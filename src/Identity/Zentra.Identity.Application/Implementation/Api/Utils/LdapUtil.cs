using System.DirectoryServices.Protocols;
using System.Net;
using Zentra.Domain;
using Zentra.Domain.Configurations.Api;
using Zentra.Domain.Enums;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Api.Validators;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Api;

namespace Zentra.Service.Implementation.Api.Utils;

internal class LdapUtil
{
    private const string LdapGetUserQuery =
        "(|(&(objectCategory=person)(objectClass=user)(uid={0}))(&(objectClass=person)(uid={0}))(&(objectClass=user)(uid={0})))";

    private readonly IFrameworkResultService frameworkResult;
    private readonly LdapConfig ldapConfig;
    private readonly ILoggerService loggerService;
    private readonly IUserAccountService userAccountService;

    public LdapUtil(
        ILoggerService loggerService,
        LdapConfig ldapConfig,
        IFrameworkResultService frameworkResult,
        IUserAccountService userAccountService)
    {
        this.loggerService = loggerService;
        this.ldapConfig = ldapConfig;
        this.frameworkResult = frameworkResult;
        this.userAccountService = userAccountService;
    }

    internal async Task<FrameworkResult> LdapLoginAsync(string username, string password)
    {
        try
        {
            var userList = await GetUserAsync(username, null);
            if (userList.ContainsAny())
            {
                if (userList.Count > 1)
                    return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.DuplicateLDAPUserFound);

                var user = userList[0];
                loggerService.WriteTo(Log.Debug, "Entered in Login : User count : " + user.Count);
                using var ldapConnection = await CreateLdapConnection();
                var dn = user["DN"];
                var credential = new NetworkCredential(dn, password);
                if (!ldapConfig.IsSecureConnection)
                {
                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.SessionOptions.SecureSocketLayer = false;
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                }
                else
                {
                    ldapConnection.AuthType = AuthType.Digest;
                    ldapConnection.SessionOptions.SecureSocketLayer = true;
                }

                ldapConnection.Bind(credential);
                loggerService.WriteTo(Log.Debug, "Ldap user login successful for user: " + username);
                return await CreateLdapUser(username, password, user);
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidLDAPUserNameOrPassword);
        }
        catch (LdapException ldapException)
        {
            var errorMessage = ldapException.Message + " Server Error = " + ldapException.ServerErrorMessage;
            return frameworkResult.ConstructFailed(ldapException.ErrorCode.ToString(), errorMessage);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while logging ldap user.");
            throw;
        }
    }

    internal async Task<IList<Dictionary<string, string>>> GetUserAsync(string userName, string[] extraFields)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userName)) frameworkResult.Throw(ApiErrorCodes.InvalidLDAPUserName);

            loggerService.WriteTo(Log.Debug, "Fetching ldap User details from user: " + userName);
            var ldapSearchFilter = string.Format(LdapGetUserQuery, userName);
            var userList = await LdapSearch(ldapConfig.LdapDomainName, ldapSearchFilter, extraFields);
            return userList;
        }
        catch (LdapException ldapException)
        {
            var errorMessage = ldapException.Message + " Server Error = " + ldapException.ServerErrorMessage;
            frameworkResult.Throw(errorMessage);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while fetching ldap user.");
            throw;
        }

        return null;
    }

    private Task<LdapConnection> CreateLdapConnection()
    {
        if (string.IsNullOrWhiteSpace(ldapConfig.LdapHostName))
            frameworkResult.Throw(ApiErrorCodes.InvalidLDAPHostname);
        else if (ldapConfig.LdapPort <= 0) frameworkResult.Throw(ApiErrorCodes.InvalidLDAPPort);

        loggerService.WriteTo(Log.Debug, "Entered in create ldap connection" + ldapConfig.LdapHostName);
        var serverId = new LdapDirectoryIdentifier(ldapConfig.LdapHostName, ldapConfig.LdapPort);
        var ldapConnection = new LdapConnection(serverId);
        ldapConnection.SessionOptions.ProtocolVersion = 3;
        if (ldapConfig.IsSecureConnection)
        {
            ldapConnection.AuthType = AuthType.External;
            ldapConnection.SessionOptions.SecureSocketLayer = true;
        }
        else
        {
            ldapConnection.AuthType = AuthType.Basic;
            ldapConnection.SessionOptions.SecureSocketLayer = false;
        }

        return Task.FromResult(ldapConnection);
    }

    private async Task<List<Dictionary<string, string>>> LdapSearch(string distinguishedName, string ldapSearchFilter,
        string[] extraFields)
    {
        using var ldapConnection = await CreateLdapConnection();
        var searchRequest = new SearchRequest(distinguishedName, ldapSearchFilter, SearchScope.Subtree, extraFields);
        var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);
        if (searchResponse != null)
        {
            loggerService.WriteTo(Log.Debug, "Entered in Ldap search for " + distinguishedName);
            var result = new List<Dictionary<string, string>>();
            foreach (SearchResultEntry entry in searchResponse.Entries)
            {
                var tempResult = new Dictionary<string, string>
                {
                    ["DN"] = entry.DistinguishedName
                };
                if (entry.Attributes?.AttributeNames != null && entry.Attributes.AttributeNames.Count > 0)
                    foreach (string attrName in entry.Attributes.AttributeNames)
                        tempResult[attrName] = string.Join(
                            ",",
                            entry.Attributes[attrName].GetValues(typeof(string)));

                result.Add(tempResult);
            }

            return result;
        }

        return null;
    }

    private async Task<FrameworkResult> CreateLdapUser(string username, string password,
        Dictionary<string, string> user)
    {
        var userModel = await userAccountService.GetUserByNameAsync(username);
        if (userModel == null) return await CreateLdapUserAsync(username, password, user);

        return await SyncLdapUserAsync(userModel, username, password, user);
    }

    private async Task<FrameworkResult> CreateLdapUserAsync(string username, string password,
        Dictionary<string, string> user)
    {
        loggerService.WriteTo(Log.Debug, "Creating LDAP user in local for user: " + username);
        try
        {
            var commonHelper = new UserManagementValidator();
            var userModel = new UserModel();
            userModel = AssignModel(userModel, username, password, user);
            if (!commonHelper.IsValidEmailAddress(userModel.Email))
                return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.EmailRequired);

            userModel.EmailConfirmed = true;
            userModel.PhoneNumberConfirmed = true;
            userModel.TwoFactorEnabled = ldapConfig.IsTwoFactorAuthenticationRequired;
            userModel.TwoFactorType = ldapConfig.TwoFactorType;
            userModel.CreatedBy = username;
            userModel.IdentityProviderType = IdentityProvider.Ldap;
            return await userAccountService.RegisterUserAsync(userModel);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to create LDAP user.");
            throw;
        }
    }

    private async Task<FrameworkResult> SyncLdapUserAsync(UserModel userModel, string username, string password,
        Dictionary<string, string> user)
    {
        loggerService.WriteTo(Log.Debug, "Updating LDAP user in local for user: " + username);
        try
        {
            userModel = AssignModel(userModel, username, password, user);
            return await userAccountService.UpdateUserAsync(userModel);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to sync LDAP user.");
            throw;
        }
    }

    private UserModel AssignModel(UserModel userModel, string username, string password,
        Dictionary<string, string> user)
    {
        userModel.UserName = username;
        userModel.Password = password;

        if (user.ContainsKey("mail"))
            userModel.Email = user["mail"];
        else if (user.ContainsKey("email"))
            userModel.Email = user["email"];
        else if (user.ContainsKey("emailaddress"))
            userModel.Email = user["emailaddress"];
        else
            userModel.Email = string.Empty;

        if (user.ContainsKey("mobile"))
            userModel.PhoneNumber = user["mobile"];
        else if (user.ContainsKey("mobileTelephoneNumber")) userModel.PhoneNumber = user["mobileTelephoneNumber"];

        if (user.ContainsKey("gn"))
            userModel.FirstName = user["gn"];
        else if (user.ContainsKey("givenname"))
            userModel.FirstName = user["givenname"];
        else if (user.ContainsKey("cn")) userModel.FirstName = user["cn"];

        if (user.ContainsKey("sn"))
            userModel.LastName = user["sn"];
        else if (user.ContainsKey("surname"))
            userModel.LastName = user["surname"];
        else
            userModel.LastName = string.Empty;

        return userModel;
    }
}

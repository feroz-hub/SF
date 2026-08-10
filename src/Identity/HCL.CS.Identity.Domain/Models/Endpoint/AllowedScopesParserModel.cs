/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Models.Endpoint;

public class AllowedScopesParserModel
{
    public List<string> ParsedIdentityResources { get; set; }

    public List<string> ParsedApiResources { get; set; }

    public List<string> ParsedApiScopes { get; set; }

    public List<string> ParsedTransactionScopes { get; set; }

    public bool AllowOfflineAccess { get; set; }

    public bool CreateIdentityToken { get; set; }

    public List<string> InvalidScopes { get; set; }

    public TokenDetailsModel TokenDetails { get; set; }

    public List<string> ParsedAllowedScopes
    {
        get
        {
            var parsedAllowedScopes = new List<string>();
            if (ParsedIdentityResources != null) parsedAllowedScopes.AddRange(ParsedIdentityResources);

            if (ParsedApiScopes != null) parsedAllowedScopes.AddRange(ParsedApiScopes);

            // TODO Refactor this property into a method.
            return parsedAllowedScopes.Distinct().ToList();
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Zentra.DemoClientWpfApp.Components;
using Zentra.DemoClientWpfApp.Constants;
using Zentra.DemoClientWpfApp.DomainModel;
using Zentra.DemoClientWpfApp.Interface;
using Zentra.DemoClientWpfApp.Services;
using static Zentra.DemoClientWpfApp.DomainModel.LogoutMessageModel;
using UserModel = Zentra.DemoClientWpfApp.DomainModel.UserModel;

namespace Zentra.DemoClientWpfApp.ViewModel
{
    internal class TwofactorAuthenticatorappViewModel : BaseViewModel, IPageViewModel
    {
        ApplicationParameters ApplicationConstants = new ApplicationParameters();
        public RelayCommand HomeCommand { get; set; }
        public RelayCommand UserHomeCommand { get; set; }
        public RelayCommand VerifyauthenticatorCommand { get; set; }
        public RelayCommand GotoEmailCommand { get; set; }
        public RelayCommand GotoSmsCommand { get; set; }

        public TwofactorAuthenticatorappViewModel()
        {
            GotoEmailCommand = new RelayCommand(param => GotoTwoFactorEmailScreen());
            GotoSmsCommand = new RelayCommand(param => GotoTwoFactorSMSScreenScreen());
            VerifyauthenticatorCommand =new RelayCommand(param => VerifyTwoFactor());
        }
        private string authenticatorVerificationCode;
        public string AuthenticatorVerificationCode
        {
            get
            {
                return authenticatorVerificationCode;
            }
            set
            {
                authenticatorVerificationCode = value;
                OnPropertyChanged("AuthenticatorVerificationCode");
            }
        }
        private void GotoTwoFactorEmailScreen()
        {
            Mediator.Notify("TwoFactorEmailScreen", "");
        }
        private void GotoTwoFactorSMSScreenScreen()
        {
            Mediator.Notify("TwoFactorSMSScreen", "");
        }
        public async Task VerifyTwoFactor()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var userName = Global.PropertyUserName;

                GlobalConfiguration.IsSmsConfigurationValid = true;
                var generateUserToken_data = new
                {
                    user_name = userName,
                    sms_token = authenticatorVerificationCode,
                };

                var verifyauthenticatorurl = ApplicationConstants.BaseUrl + ApiRoutePathConstants.TwoFactorAuthenticatorAppSignIn;
                var verifyauthenticatorresponse = Http.Client.PostAsync(verifyauthenticatorurl, new StringContent(JsonConvert.SerializeObject(authenticatorVerificationCode), Encoding.UTF8, "application/json")).Result;
                var verifyauthenticatorDetails = verifyauthenticatorresponse.Content.ReadAsStringAsync().Result;
                var verifyauthenticatorDetailsResult = JsonConvert.DeserializeObject<LogoutMessageModel.SignInResponseModel>(verifyauthenticatorDetails);

                if (verifyauthenticatorDetailsResult.Succeeded)
                {
                    AuthenticatorVerificationCode = string.Empty;
                    MessageBox.Show("Authenticator code verified successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    var tokenResponse =  GetTokenResponse().Result;
                    Global.AccessToken = tokenResponse.access_token;
                    Global.IdToken = tokenResponse.id_token;
                    Mediator.Notify("DashBoardScreen", "");
                }
                else
                {
                    MessageBox.Show(verifyauthenticatorDetailsResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
           catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = null;
        }
        public async Task<TokenResponseResultModel> GetTokenResponse()
        {
            TokenResponseResultModel tokenResponseResultModel = new TokenResponseResultModel();
            try
            {
                HttpService httpService = new HttpService();
                if (Global.AccessToken == null || Global.AccessToken == string.Empty)
                {
                    tokenResponseResultModel = httpService.AuthCodeFlow().Result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return tokenResponseResultModel;
        }
    }
}



using System;
using System.Configuration;
using System.Windows;

namespace HCL.CS.DemoClientWpfApp.Constants
{
    public class ApplicationParameters
    {
        public string BaseUrl;
        public string ClientId ;
        public string ClientSecret;
        public string AuthorizeEndpoint;
        public string TokenEndpoint ;
        public string UserInfoEndpoint ;
        public string IntrospectionEndpoint ;
        public string AuthResponseType;
        public string AuthScope ;
        public string AuthResponseMode;
        public string Prompt ;
        public string RedirectUri;
        public string ClientScope;
        public string HybridScope ;
        public string HybridResponseMode ;
        public string HybridResponseType;
        public string UserInfoScope ;
        public string UserInfoResponseMode ;
        public string UserInfoResponseType;
        public string JwtScope;
        public string JwtResponseMode;
        public string JwtResponseType;
        public string ROPScope;
        public string RefreshTokenScope;
        public string RefreshTokenMode;
        public string RefreshTokenType;

        public ApplicationParameters()
        {
            SetIntialParameters();
        }

        private void SetIntialParameters()
        {
            try
            {
                ClientId = ConfigurationManager.AppSettings["ClientId"];
                ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];
                BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
                AuthorizeEndpoint = BaseUrl + ConfigurationManager.AppSettings["AuthorizeEndpoint"];
                TokenEndpoint = BaseUrl + ConfigurationManager.AppSettings["TokenEndpoint"];
                UserInfoEndpoint = BaseUrl + ConfigurationManager.AppSettings["UserInfoEndpoint"];
                IntrospectionEndpoint = BaseUrl + ConfigurationManager.AppSettings["IntrospectionEndpoint"];
                AuthResponseType = ConfigurationManager.AppSettings["AuthResponseType"];
                AuthScope = ConfigurationManager.AppSettings["AuthScope"];
                AuthResponseMode = ConfigurationManager.AppSettings["AuthResponseMode"];
                Prompt = ConfigurationManager.AppSettings["Prompt"];
                RedirectUri = ConfigurationManager.AppSettings["RedirectUri"];
                ClientScope = ConfigurationManager.AppSettings["ClientScope"];
                HybridScope = ConfigurationManager.AppSettings["HybridScope"];
                HybridResponseMode = ConfigurationManager.AppSettings["HybridResponseMode"];
                HybridResponseType = ConfigurationManager.AppSettings["HybridResponseType"];
                UserInfoScope = ConfigurationManager.AppSettings["UserInfoScope"];
                UserInfoResponseMode = ConfigurationManager.AppSettings["UserInfoResponseMode"];
                UserInfoResponseType = ConfigurationManager.AppSettings["UserInfoResponseType"];
                JwtScope = ConfigurationManager.AppSettings["JwtScope"];
                JwtResponseMode = ConfigurationManager.AppSettings["JwtResponseMode"];
                JwtResponseType = ConfigurationManager.AppSettings["JwtResponseType"];
                ROPScope = ConfigurationManager.AppSettings["ROPScope"];
                RefreshTokenScope = ConfigurationManager.AppSettings["RefreshTokenScope"];
                RefreshTokenMode = ConfigurationManager.AppSettings["RefreshTokenMode"];
                RefreshTokenType = ConfigurationManager.AppSettings["RefreshTokenType"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error-While loading configuration.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}



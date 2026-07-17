namespace HCL.CS.DemoClientMvc.Interface;

public interface IHttpService
{
    Task<T> PostAsync<T>(string url, object Value);
    Task<T> PostSecureAsync<T>(string url, object Value);
    Task<T> PostSecureResourceServerAsync<T>(string url, object Value);
}

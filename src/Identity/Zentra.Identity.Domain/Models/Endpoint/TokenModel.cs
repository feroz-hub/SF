namespace Zentra.Domain.Models.Endpoint;

public class TokenModel
{
    public string ClientId { get; set; }

    public string ClientName { get; set; }

    public Guid UserId { get; set; }

    public virtual string UserName { get; set; }

    public virtual DateTime? LoginDateTime { get; set; }

    public virtual string Token { get; set; }

    public virtual string TokenTypeHint { get; set; }
}

namespace Zentra.Domain.Models.Api;

public class PagingModel
{
    public int TotalItems { get; set; }

    public int ItemsPerPage { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);

    public int TotalDisplayPages { get; set; }
}

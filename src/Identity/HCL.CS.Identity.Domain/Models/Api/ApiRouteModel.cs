namespace HCL.CS.Domain.Models.Api;

public class ApiRouteModel
{
    public virtual string Name { get; set; }

    public virtual string Path { get; set; }

    public virtual List<string> Permissions { get; set; }
}

//public class ApiPermissionModel
//{
//    /// <summary>
//    /// Gets or sets the Permission for Api.
//    /// </summary>
//    public virtual string Permission { get; set; }
//}

namespace HCL.CS.DemoClientMvc.Models;

public class ManageRoleModel
{
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }
    public bool IsChecked { get; set; }
}

public class ManageRoleList
{
    public List<ManageRoleModel> ManageRoles { get; set; }
}

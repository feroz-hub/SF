namespace HCL.CS.DemoClientMvc.Models;

public class UserRoleViewModel
{
    public int Id { get; set; }
    public List<ManageUsers> ManageUsers { get; set; }
}

public abstract class ManageUsers
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string LockStatus { get; set; }
}

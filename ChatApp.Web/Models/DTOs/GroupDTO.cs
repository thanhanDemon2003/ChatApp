using ChatApp.Web.Models.DTOs;

public class GroupDTO
{
    public string GroupName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public List<UserDTO> UserGroups { get; set; }
}

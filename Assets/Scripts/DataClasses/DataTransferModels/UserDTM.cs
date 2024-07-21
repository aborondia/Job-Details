using System;

[Serializable]
public class UserDTM
{
    public string objectId { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public bool verified { get; set; }
    public string sessionToken { get; set; }
    public string roleId { get; set; }
    // public RoleRelation role { get; set; }
    // public DateTime createdAt { get; set; }
    // public DateTime updatedAt { get; set; }
    // public bool emailVerified { get; set; }
}
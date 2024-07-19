using System;
using System.Collections.Generic;

public class UserDTM
{
    public string objectId { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public RoleRelation role { get; set; }
    // public DateTime createdAt { get; set; }
    // public DateTime updatedAt { get; set; }
    // public bool emailVerified { get; set; }
    public string sessionToken { get; set; }
}
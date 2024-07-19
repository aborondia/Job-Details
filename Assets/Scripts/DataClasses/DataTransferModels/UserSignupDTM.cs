using System;
using System.Collections.Generic;

public class UserSignupDTM
{
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public Dictionary<string, Permissions> ACL;
    public RoleRelation role;

    public UserSignupDTM(string user, string email, string password, string defaultRoleId)
    {
        this.username = user;
        this.email = email;
        this.password = password;
        this.role = new RoleRelation("LHFdjhi6lH");

        this.ACL = new Dictionary<string, Permissions>();

        this.role = new RoleRelation(defaultRoleId);
        this.ACL[$"role:Owner"] = new Permissions(true, true);
        this.ACL[$"role:Admin"] = new Permissions(true, true);
    }
}

[Serializable]
public class ACL
{
    public Dictionary<string, Permissions> acl;

    public ACL()
    {
        acl = new Dictionary<string, Permissions>();
        SetRolePermissions("owner", true, true);
    }

    public void SetRolePermissions(string roleName, bool read, bool write)
    {
        acl[$"role:{roleName}"] = new Permissions(read, write);
    }

}

[Serializable]
public class Permissions
{
    public bool read;
    public bool write;

    public Permissions(bool read, bool write)
    {
        this.read = read;
        this.write = write;
    }
}
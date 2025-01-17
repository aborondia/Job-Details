using System;

[Serializable]
public class RoleUpdateDTM
{
    public string userId { get; set; }
    public string newRoleId { get; set; }
    public string oldRoleId { get; set; }

    public RoleUpdateDTM(string userId, string newRoleId, string oldRoleId)
    {
        this.userId = userId;
        this.newRoleId = newRoleId;
        this.oldRoleId = oldRoleId;
    }
}

using System;

[Serializable]
public class RolePointer
{
    public string __type = "Pointer";
    public string className = "_Role";
    public string objectId;

    public RolePointer(string roleId)
    {
        this.objectId = roleId;
    }
}

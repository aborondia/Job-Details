using System;
using System.Collections.Generic;

[Serializable]
public class RoleRelation
{
    public string __op = "AddRelation";
    public List<RolePointer> objects;

    public RoleRelation(string roleId)
    {
        this.objects = new List<RolePointer>
            {
                new RolePointer(roleId)
            };
    }
}

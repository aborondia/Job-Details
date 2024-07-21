using System;

public class RoleRelationQuery
{
    public RelationPointer role;

    public RoleRelationQuery(string roleId)
    {
        this.role = new RelationPointer(roleId);
    }

    [Serializable]
    public class RelationPointer
    {
        public string __type = "Pointer";
        public string className = "_Role";
        public string objectId;

        public RelationPointer(string roleId)
        {
            this.objectId = roleId;
        }
    }
}

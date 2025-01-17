using System;
using System.Collections.Generic;

[Serializable]
public class RoleDTM
{
    public string objectId { get; set; }
    public string name { get; set; }
    public RelationOperation users { get; set; }

    public RoleDTM(string name, string objectId)
    {
        this.objectId = objectId;
        this.name = name;
    }
}

[Serializable]
public class RelationOperation
{
    public enum RelationOperations
    {
        Add,
        Remove,
    }

    public string __op;
    public List<Pointer> objects;

    public RelationOperation(List<string> userIds, RelationOperations operation)
    {
        this.objects = new List<Pointer>();

        foreach (var userId in userIds)
        {
            this.objects.Add(new Pointer(userId));
        }

        switch (operation)
        {
            case RelationOperations.Add:
                this.__op = "AddRelation";
                break;
            case RelationOperations.Remove:
                this.__op = "RemoveRelation";
                break;
        }
    }

    [Serializable]
    public class Pointer
    {
        public string __type = "Pointer";
        public string className = "_User";
        public string objectId;

        public Pointer(string userId)
        {
            this.objectId = userId;
        }
    }
}
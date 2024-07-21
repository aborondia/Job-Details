using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleDTM
{
    public string objectId { get; set; }
    public string name { get; set; }
    public RelationOperation users { get; set; }
}

[Serializable]
public class RelationOperation
{
    public string __op = "AddRelation";
    public List<Pointer> objects;

    public RelationOperation(List<string> userIds)
    {
        this.objects = new List<Pointer>();

        foreach (var userId in userIds)
        {
            this.objects.Add(new Pointer(userId));
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
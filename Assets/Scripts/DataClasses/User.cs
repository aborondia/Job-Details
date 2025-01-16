using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    private RoleDTM roleDTM;
    public RoleDTM RoleDTM => roleDTM;
    private UserDTM dtm;
    public UserDTM DTM => dtm;

    public User(UserDTM dtm, RoleDTM roleDTM)
    {
        this.dtm = dtm;
        this.roleDTM = roleDTM;
    }

    public void UpdateRole(RoleDTM roleDTM)
    {
        this.roleDTM = roleDTM;

        if (!ReferenceEquals(this.roleDTM, null))
        {
            this.dtm.roleId = this.roleDTM.objectId;
        }
    }

    public void PopulateRole(RoleDTM roleDTM)
    {
        this.roleDTM = roleDTM;
    }
}

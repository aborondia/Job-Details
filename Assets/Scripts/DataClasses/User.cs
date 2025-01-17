using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    private RoleDTM roleDTM;
    public RoleDTM RoleDTM => roleDTM;
    private UserDTM dtm;
    public UserDTM DTM => dtm;
    private int roleInHierarchy = -1;
    public int RoleInHierarchy => roleInHierarchy;

    public User(UserDTM dtm, RoleDTM roleDTM)
    {
        this.dtm = dtm;
        UpdateRole(roleDTM);
    }

    public void UpdateRole(RoleDTM roleDTM)
    {
        this.roleDTM = roleDTM;

        if (!ReferenceEquals(this.roleDTM, null))
        {
            this.dtm.roleId = this.roleDTM.objectId;
            UpdateRoleHierarchy();
        }
    }

    private void UpdateRoleHierarchy()
    {
        this.roleInHierarchy = (int)AppController.Active.UserDataHandler.GetRoleEnum(this.roleDTM.name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumLibrary
{
    public enum MainView
    {
        None = 0,
        Login = 1,
        JobDetails = 2,
    }
    public enum Subview
    {
        Default = 0,
        Login_EnterCredentials = 1,
        Login_Register = 2,
        Login_ForgotPassword = 3,
        JobDetails_ReportList = 4,
        JobDetails_Details = 5,
    }
}

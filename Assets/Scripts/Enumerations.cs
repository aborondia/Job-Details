using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enumerations
{
    public enum MainView
    {
        None = 0,
        Login = 1,
        JobDetails = 2,
        DetailsReports = 3,
        Users = 4,
    }

    public enum Subview
    {
        Default = 0,
        Login_EnterCredentials = 1,
        Login_Register = 2,
        Login_ForgotUsername = 3,
        Login_ForgotPassword = 4,
        Login_RegistrationValidation = 5,
        Login_UnregisteredUserLogin = 6,
        Login_ResetPassword = 7,
    }

    public enum JobTypeEnum
    {
        JobType = 0,
        BiWeekly = 1,
        FirstTime = 2,
        Monthly = 3,
        MoveIn = 4,
        MoveOut = 5,
        Weekly = 6,
    }

    public enum PaymentTypeEnum
    {
        Cash = 0,
        Cheque = 1,
        NoPayment = 2,
        Premium = 3,
    }

    public enum TimeOfDayEnum
    {
        AM = 0,
        PM = 1
    }

    public enum UserRoleEnum
    {
        // These correspond to the server side role names and should not change
        Admin = 1,
        Owner = 2,
        RegularUser = 0,
        Developer = 3,
    }
}

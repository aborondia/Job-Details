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
        JobType,
        BiWeekly,
        FirstTime,
        Monthly,
        MoveIn,
        MoveOut,
        Weekly,
    }

    public enum PaymentTypeEnum
    {
        PaymentType,
        Cash,
        Cheque,
        NoPayment,
        Premium,
    }

    public enum TimeOfDayEnum
    {
        AM,
        PM
    }

    public enum UserRoleEnum
    {
        // These correspond to the server side role names and should not change
        Admin = 1,
        Owner = 2,
        RegularUser = 0,
    }
}

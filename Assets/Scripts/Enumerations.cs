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
    }
    
    public enum Subview
    {
        Default = 0,
        Login_EnterCredentials = 1,
        Login_Register = 2,
        Login_ForgotPassword = 3,
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
}

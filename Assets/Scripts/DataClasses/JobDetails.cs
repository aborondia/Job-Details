using System.Collections.Generic;

namespace JobDetails
{
    namespace Enumerations
    {
        public enum JobTypeEnum
        {
            Other,
            BiWeekly,
            FirstTime,
            Monthly,
            MoveIn,
            MoveOut,
            Weekly,
        }
        public enum PaymentTypeEnum
        {
            Other,
            Cash,
            Cheque,
            NoPayment,
            Premium,
        }
        public enum TimeOfDayEnum{
            AM,
            PM
        }
    }
}

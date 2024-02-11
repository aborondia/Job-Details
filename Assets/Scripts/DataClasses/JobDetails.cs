using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JobDetails
{
    public class DetailsReport
    {
        private List<JobDetail> details = new List<JobDetail>();
        public List<JobDetail> Details => details;

        public DetailsReport()
        {
            this.details.Add(new JobDetail());
        }
    }

    namespace Enumerations
    {
        public enum JobTypeEnum
        {
            BiWeekly,
            FirstTime,
            Monthly,
            MoveIn,
            MoveOut,
            Weekly,
        }
        public enum PaymentTypeEnum
        {
            Cash,
            Cheque,
            NoPayment,
            Premium,
        }
    }
}

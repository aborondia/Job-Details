using System.Collections.Generic;

namespace JobDetails
{
    public class DetailsReport
    {
        private string userId;
        public string UserId => userId;
        private string objectId = "";
        public string ObjectId => objectId;
        private List<JobDetail> details = new List<JobDetail>();
        public List<JobDetail> Details => details;

        public DetailsReport(string userId)
        {
            this.userId = userId;

            this.details.Add(new JobDetail());
            this.details.Add(new JobDetail());
            this.details.Add(new JobDetail());
        }

        public DetailsReport(string userId, string objectId)
        {
            this.userId = userId;
            this.objectId = objectId;

            this.details.Add(new JobDetail());
            this.details.Add(new JobDetail());
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

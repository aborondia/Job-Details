const { v4: uuidv4 } = require("uuid");

Parse.Cloud.define("retrieveDetailReports", async (request) => {
  const user = request.user;

  if (!user) {
    throw new Parse.Error(
      Parse.Error.SESSION_MISSING,
      "User must be logged in to perform this action."
    );
  }

  try {
    const createdBy = request.params.createdBy;
    const DetailsReport = Parse.Object.extend("DetailsReport");
    const detailsReportQuery = new Parse.Query(DetailsReport);

    detailsReportQuery.equalTo("createdBy", createdBy);
    detailsReportQuery.include("jobDetails");

    const detailsReports = await detailsReportQuery.find({
      useMasterKey: true,
    });
    const results = [];

    for (const detailsReport of detailsReports) {
      const relation = detailsReport.relation("jobDetails");
      const jobDetailsQuery = relation.query();

      const jobDetails = await jobDetailsQuery.find({ useMasterKey: true });

      results.push({
        detailsReport: {
          objectId: detailsReport.id,
          createdBy: detailsReport.get("createdBy"),
        },
        jobDetails: jobDetails.map((jobDetail) => ({
          objectId: jobDetail.id,
          createdBy: jobDetail.get("createdBy"),
          content: jobDetail.get("content"),
          detailsReportId: jobDetail.get("detailsReportId"),
        })),
      });
    }
    return results;
  } catch (error) {
    console.error("Error retrieving DetailsReports:", error);
    throw new Parse.Error(500, "Error retrieving DetailsReports");
  }
});

Parse.Cloud.define("deleteDetailReport", async (request) => {
  const user = request.user;

  if (!user) {
    throw new Parse.Error(
      Parse.Error.SESSION_MISSING,
      "User must be logged in to perform this action."
    );
  }

  try {
    const { objectId } = request.params;
    const DetailsReport = Parse.Object.extend("DetailsReport");
    const detailsReportQuery = new Parse.Query(DetailsReport);
    detailsReportQuery.include("jobDetails");
    const reportToDelete = await detailsReportQuery.get(objectId, {
      useMasterKey: true,
    });
    if (!reportToDelete) {
      throw new Parse.Error(500, "DetailsReport not found!");
    }

    const results = [];
    const relation = reportToDelete.relation("jobDetails");
    const jobDetailsQuery = relation.query();
    const jobDetails = await jobDetailsQuery.find({ useMasterKey: true });

    if (jobDetails.length) {
      for (const jobDetail of jobDetails) {
        await jobDetail.destroy();
      }
    }

    await reportToDelete.destroy({ useMasterKey: true });

    return { Message: "DetailsReport deleted" };
  } catch (error) {
    console.error("Error deleting DetailsReport:", error);
    throw new Parse.Error(500, "Error deleting DetailsReport");
  }
});

Parse.Cloud.define("createDetailsReport", async (request) => {
  const user = request.user;
  const { createdBy } = request.params;

  if (!user) {
    throw new Parse.Error(
      Parse.Error.SESSION_MISSING,
      "User must be logged in to perform this action."
    );
  }

  try {
    const DetailsReport = Parse.Object.extend("DetailsReport");
    const detailsReport = new DetailsReport();

    detailsReport.set("createdBy", createdBy);

    await detailsReport.save(null, { useMasterKey: true });

    return detailsReport;
  } catch (error) {
    throw new Parse.Error(
      error.code || 500,
      error.message || "An error occurred while creating DetailsReport."
    );
  }
});

Parse.Cloud.define("createJobDetail", async (request) => {
  const { createdBy, content, detailsReportId } = request.params;
  const user = request.user;

  if (!user) {
    throw new Parse.Error(
      Parse.Error.SESSION_MISSING,
      "User must be logged in to perform this action."
    );
  }

  if (!createdBy || !content || !detailsReportId) {
    throw new Parse.Error(400, "Missing required parameters.");
  }

  try {
    const DetailsReport = Parse.Object.extend("DetailsReport");
    const query = new Parse.Query(DetailsReport);
    const detailsReport = await query.get(detailsReportId, {
      useMasterKey: true,
    });

    if (!detailsReport) {
      throw new Parse.Error(404, "DetailsReport not found.");
    }

    const JobDetail = Parse.Object.extend("JobDetail");
    const jobDetail = new JobDetail();

    jobDetail.set("createdBy", createdBy);
    jobDetail.set("content", content);
    jobDetail.set("detailsReportId", detailsReportId);

    await jobDetail.save(null, { useMasterKey: true });

    const relation = detailsReport.relation("jobDetails");
    relation.add(jobDetail);

    await detailsReport.save(null, { useMasterKey: true });

    return jobDetail;
  } catch (error) {
    throw new Parse.Error(
      error.code || 500,
      error.message || "An error occurred while creating JobDetail."
    );
  }
});

Parse.Cloud.define("updateJobDetail", async (request) => {
  try {
    const JobDetail = Parse.Object.extend("JobDetail");
    const jobDetail = ParseClass.fromJSON(request.params);

    const result = await jobDetail.save(null, { useMasterKey: true });

    return {
      result: result,
    };
  } catch (error) {
    console.error("Error updating JobDetail:", error);
    throw new Parse.Error(500, "Error updating JobDetail");
  }
});

Parse.Cloud.define("retrieveJobDetails", async (request) => {
  try {
    const userId = request.params.userId;

    const JobDetail = Parse.Object.extend("JobDetail");
    const query = new Parse.Query(JobDetail);

    query.equalTo("userId", userId);

    const results = await query.find({
      useMasterKey: true,
    });
    const jobDetails = results.map((result) => {
      return {
        objectId: result.id,
        userId: result.get("userId"),
        content: result.get("content"),
      };
    });

    return jobDetails;
  } catch (error) {
    console.error("Error retrieving JobDetails:", error);
    throw new Parse.Error(500, "Error retrieving JobDetails");
  }
});

Parse.Cloud.define("deleteJobDetail", async (request) => {
  try {
    const objectId = request.params.objectId;

    const JobDetail = Parse.Object.extend("JobDetail");
    const query = new Parse.Query(JobDetail);

    const jobDetail = await query.get(objectId, {
      useMasterKey: true,
    });

    await jobDetail.destroy({
      useMasterKey: true,
    });

    return { result: "JobDetail deleted successfully" };
  } catch (error) {
    console.error("Error deleting JobDetail:", error);
    throw new Parse.Error(500, "Error deleting JobDetail");
  }
});

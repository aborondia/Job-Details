const { v4: uuidv4 } = require("uuid");

Parse.Cloud.define("createJobDetail", async (request) => {
  const user = request.user;
  if (!user) {
    throw new Parse.Error(
      401,
      "User must be signed in to perform this action."
    );
  }

  try {
    const data = request.params;
    const FileObject = Parse.Object.extend("JobDetail");
    const newJsonFile = new Parse.File("stuff.json", {
      base64: Buffer.from(data.content).toString("base64"),
    });
    await newJsonFile.save();

    const newFileObject = new FileObject();
    newFileObject.set("jsonFile", newJsonFile);
    newFileObject.set("content", data.content);
    newFileObject.set("userId", data.userId);

    const result = await newFileObject.save();

    return {
      objectId: result.id,
      createdAt: result.createdAt,
    };
  } catch (error) {
    console.error("Error creating JobDetail:", error);
    throw new Parse.Error(500, "Error creating JobDetail");
  }
});

Parse.Cloud.define("updateJobDetail", async (request) => {
  const user = request.user;
  if (!user) {
    throw new Parse.Error(
      401,
      "User must be signed in to perform this action."
    );
  }

  try {
    const objectId = request.params.objectId;
    const data = request.params;

    const JobDetail = Parse.Object.extend("JobDetail");
    const existingJobDetail = await new Parse.Query(JobDetail).get(objectId);

    existingJobDetail.set("content", data.content);
    existingJobDetail.set("userId", data.userId);

    const contentBuffer = Buffer.from(data.content, "utf-8");
    const base64Content = contentBuffer.toString("base64");

    existingJobDetail.set(
      "jsonFile",
      new Parse.File("resume.txt", { base64: base64Content })
    );
    existingJobDetail.set("userId", data.userId);

    const result = await existingJobDetail.save();

    return {
      objectId: result.id,
      updatedAt: result.updatedAt,
    };
  } catch (error) {
    console.error("Error updating JobDetail:", error);
    throw new Parse.Error(500, "Error updating JobDetail");
  }
});

Parse.Cloud.define("retrieveJobDetails", async (request) => {
  const user = request.user;
  if (!user) {
    throw new Parse.Error(
      401,
      "User must be signed in to perform this action."
    );
  }

  try {
    const userId = request.params.userId;

    const JobDetail = Parse.Object.extend("JobDetail");
    const query = new Parse.Query(JobDetail);

    query.equalTo("userId", userId);

    const results = await query.find();
    const jobDetails = results.map((result) => {
      return {
        objectId: result.id,
        jsonFile: result.get("jsonFile"),
        userId: result.get("userId"),
        content: result.get("content"),
      };
    });

    return { jobDetails };
  } catch (error) {
    console.error("Error retrieving JobDetails:", error);
    throw new Parse.Error(500, "Error retrieving JobDetails");
  }
});

Parse.Cloud.define("deleteJobDetail", async (request) => {
  const user = request.user;
  if (!user) {
    throw new Parse.Error(
      401,
      "User must be signed in to perform this action."
    );
  }

  try {
    const objectId = request.params.objectId;

    const JobDetail = Parse.Object.extend("JobDetail");
    const query = new Parse.Query(JobDetail);

    const jobDetail = await query.get(objectId);

    await jobDetail.destroy();

    return { result: "JobDetail deleted successfully" };
  } catch (error) {
    console.error("Error deleting JobDetail:", error);
    throw new Parse.Error(500, "Error deleting JobDetail");
  }
});

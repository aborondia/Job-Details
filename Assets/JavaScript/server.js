const sgMail = require("@sendgrid/mail");
const { v4: uuidv4 } = require("uuid");

sgMail.setApiKey(process.env.SENDGRID_API_KEY);

Parse.Cloud.define("getUsernames", async (request) => {
    try {
        const query = new Parse.Query(Parse.User);
        query.select("username"); // Specify the column you want to retrieve
        
        const results = await query.find({ useMasterKey: true });
        
        // Extract usernames from results
        const usernames = results.map(user => user.get("username"));
        
        return usernames;
    } catch (error) {
        throw new Error("Error retrieving usernames: " + error.message);
    }
});

Parse.Cloud.define("sendEmail", async (request) => {
  try {
    const emailData = request.params;

    const msg = {
      to: { email: emailData.To },
      from: { email: emailData.From },
      subject: emailData.Subject,
      html: emailData.Body,
      attachments: [
        {
          content: emailData.Content,
          filename: emailData.FileName,
          type: emailData.Type,
          disposition: emailData.Disposition,
        },
      ],
    };

    await sgMail.send(msg);

    return { result: "Email sent successfully" };
  } catch (error) {
    console.error("Error sending email:", error);

    if (error.response && error.response.body && error.response.body.errors) {
      console.error("SendGrid API Errors:", error.response.body.errors);
    }
    throw new Parse.Error(500, "Error sending email");
  }
});

Parse.Cloud.define("createJobDetail", async (request) => {
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


Parse.Cloud.define("getRolesWithUsers", async (request) => {
  const Role = Parse.Object.extend("_Role");
  const query = new Parse.Query(Role);

  try {
    const roles = await query.find({ useMasterKey: true });
    const roleWithUsersPromises = roles.map(async (role) => {
      const usersRelation = role.relation("users");
      const usersQuery = usersRelation.query();
      const users = await usersQuery.find({ useMasterKey: true });

      return {
        role: {
          objectId: role.id,
          name: role.get("name"),
        },
        users: users.map(user => ({
          objectId: user.id,
          username: user.get("username"),
          email: user.get("email"),
          verified: user.get("verified"),
        })),
      };
    });

    const rolesWithUsers = await Promise.all(roleWithUsersPromises);
    return rolesWithUsers;
  } catch (error) {
    console.error("Error in getRolesWithUsers function:", error); // Log the error for debugging
    throw new Parse.Error(Parse.Error.INTERNAL_SERVER_ERROR, error.message);
  }
});

Parse.Cloud.define("deleteJobDetail", async (request) => {
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

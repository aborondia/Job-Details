require("./details.js");
require("./users.js");

const sgMail = require("@sendgrid/mail");
const { v4: uuidv4 } = require("uuid");

sgMail.setApiKey(process.env.SENDGRID_API_KEY);

Parse.Cloud.define("sendEmail", async (request) => {
  const user = request.user;
  if (!user) {
    throw new Parse.Error(
      401,
      "User must be signed in to perform this action."
    );
  }

  const from = process.env.SENDGRID_EMAIL;

  try {
    const emailData = request.params;

    const msg = {
      to: { email: emailData.To },
      from: { email: from },
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

    await sgMail.send(msg, { useMasterKey: true });

    return { result: "Email sent successfully" };
  } catch (error) {
    console.error("Error sending email:", error);

    if (error.response && error.response.body && error.response.body.errors) {
      console.error("SendGrid API Errors:", error.response.body.errors);
    }
    throw new Parse.Error(500, "Error sending email");
  }
});

Parse.Cloud.job("cleanDatabase", async (request) => {
  const sessionQuery = new Parse.Query("_Session");
  const registrationQuery = new Parse.Query("PendingUser");

  const maxSessionIdleTime = 3600 * 1000;
  const maxRegistrationIdleTime = 3600 * 1000 * 24;

  const currentTime = new Date();
  const sessionExpirationThreshold = new Date(
    currentTime.getTime() - maxSessionIdleTime
  );
  const registrationExpirationThreshold = new Date(
    currentTime.getTime() - maxRegistrationIdleTime
  );

  sessionQuery.lessThan("updatedAt", sessionExpirationThreshold);
  registrationQuery.lessThan("createdAt", registrationExpirationThreshold);

  try {
    const expiredRegistrations = await registrationQuery.find({
      useMasterKey: true,
    });
    const expiredSessions = await sessionQuery.find({ useMasterKey: true });

    await Parse.Object.destroyAll(expiredRegistrations, { useMasterKey: true });
    await Parse.Object.destroyAll(expiredSessions, { useMasterKey: true });

    return `Cleanup complete`;
  } catch (error) {
    throw error;
  }
});

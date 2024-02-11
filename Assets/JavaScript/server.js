const sgMail = require("@sendgrid/mail");

sgMail.setApiKey(process.env.SENDGRID_API_KEY);

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

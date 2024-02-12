const sgMail = require("@sendgrid/mail");
const { v4: uuidv4 } = require("uuid");

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

Parse.Cloud.define("uploadJobDetail", async (request) => {
  try {
    const data = request.params;
    if (!data.CustomId) {
      throw new Parse.Error(400, "Id is required");
    }
    const FileObject = Parse.Object.extend("JobDetail");
    const query = new Parse.Query(FileObject);
    query.equalTo("CustomId", data.CustomId);
    const existingFileObject = await query.first();
    if (existingFileObject) {
      const existingJsonFile = existingFileObject.get("jsonFile");
      existingJsonFile.setData(data.Content, { base64: true });
      await existingJsonFile.save();
      return {
        result: "JSON file replaced successfully",
        objectId: existingFileObject.id,
      };
    } else {
      const newJsonFile = new Parse.File("uploaded_data.json", {
        base64: Buffer.from(data.Content).toString("base64"),
      });
      await newJsonFile.save();
      const newFileObject = new FileObject();
      newFileObject.set("jsonFile", newJsonFile);
      newFileObject.set("CustomId", data.CustomId);
      newFileObject.set("Content", data.Content);
      await newFileObject.save();
      return {
        result: data,
      };
    }
  } catch (error) {
    console.error("Error uploading/replacing JSON file:", error);
    throw new Parse.Error(500, "Error uploading/replacing JSON file");
  }
});

Parse.Cloud.define("getUniqueId", async () => {
  try {
    const uniqueId = uuidv4();
    return { objectId: uniqueId };
  } catch (error) {
    console.error("Error generating unique Id:", error);
    throw new Parse.Error(500, "Error generating unique Id");
  }
});

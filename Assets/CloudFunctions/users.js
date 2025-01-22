const { v4: uuidv4 } = require("uuid");
const sgMail = require("@sendgrid/mail");

Parse.Cloud.define("userLogin", async (request) => {
  const { username, password } = request.params;

  if (!username || !password) {
    throw new Error("Username and password are required.");
  }

  try {
    const user = await Parse.User.logIn(username, password);

    if (user.verified) {
      return {
        sessionToken: user.getSessionToken(),
        user: user.toJSON(),
      };
    } else {
      await Parse.User.logOut();

      return {
        user: {
          verified: user.get("verified"),
        },
      };
    }
  } catch (error) {
    throw new Error(`Login failed: ${error.message}`);
  }
});

Parse.Cloud.define("checkRegistrationCredentials", async (request) => {
  const { email, username } = request.params;

  if (!email && !username) {
    throw new Parse.Error(400, "Both email and username are missing.");
  }

  try {
    const emailQuery = new Parse.Query(Parse.User);
    if (email) {
      emailQuery.equalTo("email", email.toLowerCase());
    }

    const pendingEmailQuery = new Parse.Query("PendingUser");
    if (email) {
      pendingEmailQuery.equalTo("email", email.toLowerCase());
    }

    const usernameQuery = new Parse.Query(Parse.User);
    if (username) {
      usernameQuery.equalTo("username", username);
    }

    const pendingUsernameQuery = new Parse.Query("PendingUser");
    if (username) {
      pendingUsernameQuery.equalTo("username", username);
    }

    let combinedUserQuery;
    if (email && username) {
      combinedUserQuery = Parse.Query.or(emailQuery, usernameQuery);
    } else if (email) {
      combinedUserQuery = Parse.Query(emailQuery);
    } else {
      combinedUserQuery = Parse.Query(usernameQuery);
    }

    let combinedPendingUserQuery;
    if (email && username) {
      combinedPendingUserQuery = Parse.Query.or(
        pendingEmailQuery,
        pendingUsernameQuery
      );
    } else if (email) {
      combinedPendingUserQuery = Parse.Query(pendingEmailQuery);
    } else {
      combinedPendingUserQuery = Parse.Query(pendingUsernameQuery);
    }

    const results = await combinedUserQuery.find({ useMasterKey: true });
    const pendingResults = await combinedPendingUserQuery.find({
      useMasterKey: true,
    });

    const result = {
      emailExists: false,
      usernameExists: false,
    };

    results.forEach((user) => {
      if (!result.emailExists && user.get("email") === email.toLowerCase()) {
        result.emailExists = true;
      }
      if (!result.usernameExists && user.get("username") === username) {
        result.usernameExists = true;
      }
    });

    pendingResults.forEach((user) => {
      if (!result.emailExists && user.get("email") === email.toLowerCase()) {
        result.emailExists = true;
      }
      if (!result.usernameExists && user.get("username") === username) {
        result.usernameExists = true;
      }
    });

    return result;
  } catch (error) {
    throw new Parse.Error(
      error.code || 500,
      error.message || "An error occurred while checking email and username."
    );
  }
});

Parse.Cloud.define("getUsernames", async (request) => {
  const user = request.user;
  if (!user) {
    throw new Parse.Error(
      401,
      "User must be signed in to perform this action."
    );
  }

  try {
    const query = new Parse.Query(Parse.User);
    query.select("username");

    const results = await query.find({ useMasterKey: true });
    const usernames = results.map((user) => user.get("username"));

    return usernames;
  } catch (error) {
    throw new Error("Error retrieving usernames: " + error.message);
  }
});

Parse.Cloud.define("verifyUser", async (request) => {
  const user = request.user;

  if (!user) {
    throw new Parse.Error(
      401,
      "User must be signed in to perform this action."
    );
  }

  const { userId } = request.params;
  const roleQuery = new Parse.Query(Parse.Role);

  try {
    const User = Parse.Object.extend("_User");
    const existingUserQuery = new Parse.Query(Parse.User);
    const existingUser = await existingUserQuery.get(userId, {
      useMasterKey: true,
    });

    if (existingUser) {
      roleQuery.equalTo("name", "RegularUser");
      const role = await roleQuery.first({ useMasterKey: true });
      if (!role) {
        throw new Parse.Error(Parse.Error.OBJECT_NOT_FOUND, "Role not found.");
      }

      const roleRelation = role.relation("users");
      roleRelation.add(existingUser);
      await role.save(null, { useMasterKey: true });

      existingUser.set("verified", true);
      await existingUser.save(null, { useMasterKey: true });

      return "User verified!";
    }
  } catch (error) {
    console.error("Error verifying:", error);
    throw new Parse.Error(500, "Error verifying");
  }
});

Parse.Cloud.define("getRole", async (request) => {
  const user = request.user;
  if (!user) {
    throw new Parse.Error(
      401,
      "User must be signed in to perform this action."
    );
  }

  try {
    const roleObjectId = request.params.objectId;
    const Role = Parse.Object.extend("_Role");
    const existingRole = await new Parse.Query(Role).get(roleObjectId);

    return { existingRole };
  } catch (error) {
    console.error("Error getting role:", error);
    throw new Parse.Error(500, "Error getting role");
  }
});

Parse.Cloud.define("getUsersForAdmin", async (request) => {
  const user = request.user;
  if (!user) {
    throw new Parse.Error(
      401,
      "User must be signed in to perform this action."
    );
  }

  try {
    const query = new Parse.Query(Parse.User);
    const users = await query.find({ useMasterKey: true });

    const sanitizedUsers = users.map((user) => ({
      objectId: user.id,
      username: user.get("username"),
      email: user.get("email"),
      verified: user.get("verified"),
      roleId: user.get("roleId"),
    }));

    return sanitizedUsers;
  } catch (error) {
    throw new Error("Error retrieving usernames: " + error.message);
  }
});

Parse.Cloud.define("getUsersForRegularUser", async (request) => {
  const user = request.user;
  if (!user) {
    throw new Parse.Error(
      401,
      "User must be signed in to perform this action."
    );
  }

  try {
    const query = new Parse.Query(Parse.User);
    const users = await query.find({ useMasterKey: true });

    const sanitizedUsers = users.map((user) => ({
      username: user.get("username"),
      email: user.get("email"),
    }));

    return sanitizedUsers;
  } catch (error) {
    throw new Error("Error retrieving usernames: " + error.message);
  }
});

Parse.Cloud.define("updateUserRole", async (request) => {
  const { userId, newRoleId, oldRoleId } = request.params;

  if (!userId || !newRoleId || !oldRoleId) {
    throw "Missing required parameters: userId, newRoleId, or oldRoleId.";
  }

  const userQuery = new Parse.Query(Parse.User);
  const roleQuery = new Parse.Query(Parse.Role);

  try {
    const user = await userQuery.get(userId, { useMasterKey: true });
    if (!user) {
      throw "User not found.";
    }

    const oldRole = await roleQuery.get(oldRoleId, { useMasterKey: true });
    if (!oldRole) {
      throw "Old role not found.";
    }

    const newRole = await roleQuery.get(newRoleId, { useMasterKey: true });
    if (!newRole) {
      throw "New role not found.";
    }

    const oldRoleRelation = oldRole.relation("users");
    oldRoleRelation.remove(user);

    await oldRole.save(null, { useMasterKey: true });

    const newRoleRelation = newRole.relation("users");
    newRoleRelation.add(user);

    await newRole.save(null, { useMasterKey: true });

    user.set("roleId", newRoleId);
    await user.save(null, { useMasterKey: true });

    return { success: true, message: "User role updated successfully." };
  } catch (error) {
    console.error("Error updating user role:", error);
    throw new Parse.Error(
      Parse.Error.SCRIPT_FAILED,
      `Failed to update user role: ${error}`
    );
  }
});

Parse.Cloud.define("registerUser", async (request) => {
  const { username, email, password, verificationUrlBase } = request.params;

  if (!username || !email || !password || !verificationUrlBase) {
    throw new Error("Missing required parameters.");
  }

  const token = (
    Math.random().toString(36).substr(2, 9) + Date.now()
  ).toString();

  const PendingUser = Parse.Object.extend("PendingUser");
  const pendingUser = new PendingUser();
  pendingUser.set("username", username);
  pendingUser.set("email", email);
  pendingUser.set("password", password);
  pendingUser.set("token", token);

  try {
    await pendingUser.save({ useMasterKey: true });

    const msg = {
      to: email,
      from: process.env.SENDGRID_EMAIL,
      subject: "Job Details - Verify Your Email",
      html: `<p>Hello ${username},</p>
                 <p>Please verify your email by entering the code below in the registration section of the app:</p>
                 <p><b><i>${token}<b><i></p>
                 <p>If you did not register, you can safely ignore this email.</p>
                 <p>This code will expire after 24 hours.</p>`,
    };

    await sgMail.send(msg, { useMasterKey: true });

    return "Verification email sent successfully.";
  } catch (error) {
    throw new Error(`Error registering user: ${error.message}`);
  }
});

Parse.Cloud.define("verifyRegistration", async (request) => {
  const token = request.params.token;
  const result = { success: false, message: "Default" };

  if (!token) {
    result.message = "Invalid or missing token!";
  } else {
    const query = new Parse.Query("PendingUser");
    query.equalTo("token", token);
    const pendingUser = await query.first({ useMasterKey: true });

    if (!pendingUser) {
      result.message = "Invalid token!";
    } else {
      try {
        const user = new Parse.User();
        user.set("username", pendingUser.get("username"));
        user.set("email", pendingUser.get("email"));
        user.set("password", pendingUser.get("password"));
        await user.signUp(
          { roleId: process.env.DEFAULT_ROLE_ID, emailVerified: true },
          { useMasterKey: true }
        );

        await pendingUser.destroy({ useMasterKey: true });
        result.success = true;
        result.message = "Success";
      } catch (error) {
        result.message = `Error: ${error.message}`;
      }
    }
  }

  return result;
});

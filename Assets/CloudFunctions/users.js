const { v4: uuidv4 } = require("uuid");

Parse.Cloud.define("userLogin", async (request) => {
  const { username, password } = request.params;

  if (!username || !password) {
    throw new Error("Username and password are required.");
  }

  try {
    const user = await Parse.User.logIn(username, password);
    return {
      sessionToken: user.getSessionToken(),
      user: user.toJSON(),
    };
  } catch (error) {
    throw new Error(`Login failed: ${error.message}`);
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

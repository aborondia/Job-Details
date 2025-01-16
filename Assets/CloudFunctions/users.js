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

Parse.Cloud.define("getRole", async (request) => {
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
        users: users.map((user) => ({
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
    console.error("Error in getRolesWithUsers function:", error);
    throw new Parse.Error(Parse.Error.INTERNAL_SERVER_ERROR, error.message);
  }
});

Parse.Cloud.define("getUnverifiedUsers", async (request) => {
  const User = Parse.Object.extend("_User");
  const query = new Parse.Query(User);
  query.equalTo("verified", false);

  try {
    const unverifiedUsers = await query.find({ useMasterKey: true });
    const usersData = unverifiedUsers.map((user) => ({
      objectId: user.id,
      username: user.get("username"),
      email: user.get("email"),
      verified: user.get("verified"),
    }));
    return usersData;
  } catch (error) {
    console.error("Error in getUnverifiedUsers function:", error);
    throw new Parse.Error(Parse.Error.INTERNAL_SERVER_ERROR, error.message);
  }
});
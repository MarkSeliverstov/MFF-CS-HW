const express = require("express")
const { graphqlHTTP } = require("express-graphql")
const {
  GraphQLSchema,
  GraphQLObjectType,
  GraphQLString,
  GraphQLList,
  GraphQLID
} = require("graphql");

// Data

const userData = [
  {
    "id": "001",
    "name": "Petr"
  }, {
    "id": "002",
    "name": "Alice"
  }
];

const groupData = [
  {
    "id": "001",
    "homepage": "https://webik.ms.mff.cuni.cz/",
    "users": ["001"],
  }
];

// Resolvers

/**
 * Return all users.
 */
const resolveUsers = async () => {
    const users = (await axios.get(userEndpoint)).data;
    const response = [];
    for (const identifier of users) {
      const user = (await axios.get(userEndpoint + identifier)).data;
      response.push(user);
    }
    return response;
  };

  const userResolver = async (source, params) => {
    return (await axios.get(userEndpoint + params.identifier)).data;
  };

/**
 * Return user with given identifier.
 */
const userEndpoint = "http://localhost:3001/api/v1/users/";
const resolveUser = async (identifier) => {
  for (const item of userData) {
    if (item.id === identifier) {
      return item;
    }
  }
  return undefined;
}

/**
 * Return all groups.
 */
const resolveGroups = async () => {
  return groupData;
}

// Schema

const UserType = new GraphQLObjectType({
  "name": "User",
  "fields": {
    "id": { "type": GraphQLID },
    "name": { "type": GraphQLString },
  },
});

const GroupType = new GraphQLObjectType({
  "name": "Group",
  "fields": {
    "id": { "type": GraphQLID },
    "homepage": { "type": GraphQLString },    
    "users": { 
      "type": GraphQLList(UserType),
      "resolve":  async (parent, params) => {
        const result = [];
        for (const identifier of parent.users) {
          result.push(await resolveUser(identifier));
        }
        return result;
      },
    },
  }
});

const schema = new GraphQLSchema({
  "query": new GraphQLObjectType({
    "name": "Query",
    "fields": {
      "users": {
        "type": GraphQLList(UserType),
        "resolve": (parent, params) => resolveUsers(),
      },
      "user": {
        "type": UserType,
        "args": {
          "identifier": { "type": GraphQLString },
        },
        "resolve": (source, params) => resolveUser(params.identifier),
      },
      "groups": {
        "type": GraphQLList(GroupType),
        "resolve": (source, params) => resolveGroups(),
      },
    },
  })
});

const app = express()
app.use(
  "/graphql",
  graphqlHTTP({
    schema: schema,
    graphiql: true,
  })
)

app.listen(4000)
console.log("Running a GraphQL API server at http://localhost:4000/graphql");
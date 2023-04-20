var express = require("express")
var { graphqlHTTP } = require("express-graphql")
var { graphql, GraphQLSchema, GraphQLObjectType, GraphQLString, GraphQLID, GraphQLList } = require("graphql")

// Construct a schema, using GraphQL schema language\

var User = new GraphQLObjectType({
    name: "user",
    fields: {
      id: { type: GraphQLID },
      name: { type: GraphQLString },
    },
})

// 2 uloha
var Group = new GraphQLObjectType({
    name: "group",
    fields: {
        id: { type: GraphQLID },
        users: { 
            type: GraphQLList(User) ,
            resolve(parent, args){
                console.log("print"+parent);
                return [];
            },
        },
        homepage: { type: GraphQLString },
    },
});

var schema = new GraphQLSchema({
    query: new GraphQLObjectType({
      name: 'Query',
      fields: {

        users: {
          type: GraphQLList(User),
          resolve(){
            return [];
         },
        },

        // 2 uloha
        groups: {
            type: GraphQLList(Group),
            resolve(){
                return [];
            }
        },
        user: {
            type: User,
            args: {
                id: { type: GraphQLID },
            },
            resolve(parent, args){
                return;
            }
      },
    }}),
});

var app = express()
app.use('/graphql', graphqlHTTP({
    schema: schema,
    graphiql: true,
}))

app.listen(4000)
console.log("Running a GraphQL API server at http://localhost:4000/graphql")
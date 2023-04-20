const express = require("express");
const app = express();

const database = {};

(function initialize() {
  // Generated using https://generatedata.com/generator
  [
    {
      id: "001",
      name: "Neve Terrell",
    },
    {
      id: "002",
      name: "Clare Fulton",
    },
    {
      id: "003",
      name: "Kylie Montoya",
    },
    {
      id: "004",
      name: "Ivory Mcintyre",
    },
    {
      id: "005",
      name: "Dacey Parks",
    },
    {
      id: "006",
      name: "Hilda Petersen",
    },
    {
      id: "007",
      name: "Macy Valdez",
    },
    {
      id: "008",
      name: "Beverly Bates",
    },
    {
      id: "009",
      name: "Althea Reyes",
    },
    {
      id: "010",
      name: "Indira Erickson",
    },
    {
      id: "011",
      name: "Hilda Petersen",
    },
    {
      id: "012",
      name: "Macy Valdez",
    },
    {
      id: "013",
      name: "Beverly Bates",
    },
    {
      id: "014",
      name: "Althea Reyes",
    },
    {
      id: "015",
      name: "Indira Erickson",
    },
  ].forEach((item) => {
    database[item.id] = item;
  });
})();

app.get("/api/v1/users/", async (req, res) => {
  res.send(Object.keys(database));
});

app.get("/api/v1/users/:id", (req, res) => {
  if (database[req.params.id] === undefined) {
    res.status(404).send({ message: "User not found" });
  } else {
    res.send(database[req.params.id]);
  }
});

const port = 3001;
app.listen(port, () => {
  console.log(`Listening on port ${port}`);
});
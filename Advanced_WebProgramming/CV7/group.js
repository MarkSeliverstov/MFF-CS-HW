const express = require("express");
const app = express();

const database = {};

(function initialize() {
  // Generated using https://generatedata.com/generator
  [
    {
      id: "3363D7EA-BB75-824C-DE03-928B889E71E5",
      users: ["006", "013"],
      homepage: "http://cnn.com",
    },
    {
      id: "1E4FAF0C-83EC-D4E7-A2EC-73E526743A16",
      users: ["012", "002"],
      homepage: "https://yahoo.com",
    },
    {
      id: "FB6B437B-A434-1A65-B2FB-E51AAB1D5E22",
      users: ["013", "014"],
      homepage: "http://baidu.com",
    },
    {
      id: "48F13198-37E6-A4E3-6C53-7E1D4315556B",
      users: ["015", "001", "002"],
      homepage: "http://walmart.com",
    },
    {
      id: "9F1132DE-93B0-D235-E4E9-35B3CA991721",
      users: ["004", "012", "005"],
      homepage: "http://wikipedia.org",
    },
    {
      id: "9BB1A8BC-0148-C913-4175-8048B732C5E2",
      users: ["011"],
      homepage: "http://wikipedia.org",
    },
    {
      id: "C985C29D-8472-C388-B5A5-01669D4C13B2",
      users: ["002", "008", "006"],
      homepage: "https://zoom.us",
    },
    {
      id: "A7A14483-4B76-B8EC-0927-6A633E93868D",
      users: ["004", "005"],
      homepage: "http://cnn.com",
    },
    {
      id: "E04EA513-C164-8A8B-28A3-B452A9210888",
      users: ["015", "008"],
      homepage: "https://instagram.com",
    },
    {
      id: "6D9CEB61-425A-AD5E-A167-11366C69C644",
      users: ["015"],
      homepage: "http://bbc.co.uk",
    },
    {
      id: "C66DE40A-2315-87BD-45BB-B6E366E240AC",
      users: ["012", "005"],
      homepage: "http://twitter.com",
    },
    {
      id: "42E74F29-A630-C287-BCAA-CAEAC89495CD",
      users: ["014", "012"],
      homepage: "https://baidu.com",
    },
    {
      id: "FF91C126-3C80-5999-58E7-196AEAFF1C62",
      users: ["003"],
      homepage: "https://netflix.com",
    },
    {
      id: "308A7723-D52E-12F3-5811-75BDD228D663",
      users: ["012", "006"],
      homepage: "https://whatsapp.com",
    },
    {
      id: "2E0181DE-603B-4687-AE7F-82C139C1D0E5",
      users: ["006"],
      homepage: "http://twitter.com",
    },
    {
      id: "CA5237D5-266C-CE7E-7232-4652369BED49",
      users: ["001", "015", "013"],
      homepage: "http://wikipedia.org",
    },
    {
      id: "D8B19454-AED2-1FD2-B452-D2F9A8EC933A",
      users: ["011", "015"],
      homepage: "https://yahoo.com",
    },
    {
      id: "FBEF5A99-4636-574E-CC72-99BD6B9EB352",
      users: ["013", "011"],
      homepage: "http://facebook.com",
    },
    {
      id: "BB738536-A84E-AB0E-6624-7543BF6E36CB",
      users: ["005", "010"],
      homepage: "https://pinterest.com",
    },
    {
      id: "29B91845-BC71-E1C6-1ABF-BB40CECBC5D4",
      users: ["009"],
      homepage: "https://instagram.com",
    },
  ].forEach((item) => {
    database[item.id] = item;
  });
})();

app.get("/api/v1/groups/", async (req, res) => {
  res.send(Object.keys(database));
});

app.get("/api/v1/groups/:id", (req, res) => {
  if (database[req.params.id] === undefined) {
    res.status(404).send({ message: "User not found" });
  } else {
    res.send(database[req.params.id]);
  }
});

const port = 3002;
app.listen(port, () => {
  console.log(`Listening on port ${port}`);
});
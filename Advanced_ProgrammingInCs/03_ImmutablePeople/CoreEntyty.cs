using System;
using System.Collections.Generic;

namespace CoreEntities {
    public abstract class Person {
        public string FirstName { get; }
        public string LastName { get; }
        public string Password { get; }

        public Person(string firstName, string lastName, string password) {
            FirstName = firstName;
            LastName = lastName;
            Password = password;
        }

        public override string ToString() => $"{this.GetType().Name} {FirstName} {LastName} has password \"{Password}\"";

        public abstract Person WithName(string name);
        public abstract Person WithPassword(string password);
    }
}
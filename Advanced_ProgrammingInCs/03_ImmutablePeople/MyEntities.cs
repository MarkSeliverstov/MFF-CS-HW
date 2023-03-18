using System;
using CoreEntities;

namespace MyEntities {
    public class Student : Person {
        public DateOnly DateEnrolled { get; }

        public Student(string firstName, string LastName, string password, DateOnly dateEnrolled) : 
        base(firstName, LastName, password) {
            DateEnrolled = dateEnrolled;
        }

        public static Student Default => new Student("Default", "Default", "Default", default(DateOnly));

        public override Student WithPassword(string password) => new Student(FirstName, LastName,  password, DateEnrolled);
        public override Student WithName(string name){
            var names = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var FirstName = names[0];
            var LastName = names[1];
            return new Student(FirstName, LastName, Password, DateEnrolled);
        }
        public Student WithDateEnrolled(DateOnly dateEnrolled) => new Student(FirstName, LastName, Password, dateEnrolled);
    }

    public class Teacher : Person {
        public int CoursesHeld { get; }

        public Teacher(string firstName, string lastName, string password, int coursesHeld) : 
        base(firstName, lastName, password) {
            CoursesHeld = coursesHeld;
        }

        public static Teacher Default => new Teacher("Default", "Default", "Default", 0);

        public override Teacher WithPassword(string password) => new Teacher(FirstName, LastName, password, CoursesHeld);
        public override Teacher WithName(string name){
            var names = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var FirstName = names[0];
            var LastName = names[1];
            return new Teacher(FirstName, LastName, Password, CoursesHeld);
        }
        public Teacher WithCoursesHeld(int coursesHeld) => new Teacher(FirstName, LastName, Password, coursesHeld);
    }
}
using System;
using System.Collections.Generic;

namespace Entities
{
    public abstract class Person{
        public string FirstName { get; private set;} = "Default";
        public string LastName { get; private set;} = "Default";
        public string Password { get; private set;} = "Default";

        public override string ToString() => $"{this.GetType().Name} {FirstName} {LastName} has password \"{Password}\"";

        public void ChangeName(string firstName, string lastName){
            FirstName = firstName;
            LastName = lastName;
        }
        public void ChangePassword(string password) => Password = password;
    }

    public sealed class Student : Person{
        public DateOnly DateEnrolled { get; private set; } = new DateOnly( 1, 1, 1);
        public void ChangeDateEnrolled(DateOnly dateEnrolled) => DateEnrolled = dateEnrolled;
        public static Student Default => new Student();
    }

    public sealed class Teacher : Person{
        public int CoursesHeld { get; private set;} = 0;
        public void ChangeCoursesHeld(int coursesHeld) => CoursesHeld = coursesHeld;
        public static Teacher Default => new Teacher();
    }

    public static class PersonExtensions{
        public static T WithName<T>(this T person, string name) where T : Person{
            string[] names = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);   
            person.ChangeName(names[0], names[1]);
            return person;
        }

        public static T WithPassword<T>(this T person, string password) where T:Person{
            person.ChangePassword(password);
            return person;
        }

        public static void PrintAll<T>(this List<T> people) where T:Person{
            foreach(var person in people){
                Console.WriteLine(person);
            }
        }

        public static List<T> WithPasswordResetByFirstName<T> (this List<T> people, string firstName, string newPassword) where T:Person{
            List<T> updatedPeople = new List<T>();
            foreach(var person in people){
                if(person.FirstName.Equals(firstName)){
                     
                }
            }
            return updatedPeople;
        }
    }

    public static class StudentExtensions{
        public static Student WithDateEnrolled(this Student student, DateOnly dateEnrolled){
            student.ChangeDateEnrolled(dateEnrolled);
            return student;
        }
    }

    public static class TeacherExtensions{
        public static Teacher WithCoursesHeld(this Teacher teacher, int coursesHeld){
            teacher.ChangeCoursesHeld(coursesHeld);
            return teacher;
        }
    }
}
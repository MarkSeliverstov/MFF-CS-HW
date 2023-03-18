using System;
using System.Collections.Generic;

using CoreEntities;

namespace MyExtentions {
    public static class PersonExtentions{
        public static void PrintAll<T>(this List<T> people) where T:Person{
            foreach(var person in people){
                Console.WriteLine(person);
            }
        }

        public static List<T> WithPasswordResetByFirstName<T> (this List<T> people, string firstName, string newPassword) where T:Person{
            List<T> updatedPeople = new List<T>();
            foreach(var person in people){
                if(person.FirstName == firstName){
                    updatedPeople.Add((T)person.WithPassword(newPassword));
                } else {
                    updatedPeople.Add(person);
                }
            }
            return updatedPeople;
        }
    }
}
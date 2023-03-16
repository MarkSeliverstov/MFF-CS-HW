using System;
using System.Collections.Generic;

namespace ImmutablePeople {
	
	class Program {
		static void Main(string[] args) {
			var studentA = Student.Default
							.WithName("Pavel Rozek").WithPassword("lavice")
							.WithDateEnrolled(new DateOnly(2015, 9, 21));

			var studentB = Student.Default
							.WithName("Marie Pichova").WithPassword("zidle")
							.WithDateEnrolled(new DateOnly(2018, 9, 25));

			var teacher = Teacher.Default
							.WithName("Pavel Jezek").WithPassword("stul")
							.WithCoursesHeld(5);

			List<Person> people = new List<Person> { studentA, studentB, teacher };
			Console.WriteLine("+++ People:");
			people.PrintAll();
			Console.WriteLine();

			var updatedPeople = people.WithPasswordResetByFirstName(firstName: "Pavel", newPassword: "pohovka");
			Console.WriteLine("+++ Updated people:");
			updatedPeople.PrintAll();
			Console.WriteLine();

			Console.WriteLine("+++ People:");
			people.PrintAll();
			Console.WriteLine();

			Console.WriteLine("+++ Just students:");
			var justStudents = new List<Student> { studentA, studentB };
			justStudents.PrintAll();
			Console.WriteLine();

			var updatedStudents = justStudents.WithPasswordResetByFirstName(firstName: "Marie", newPassword: "letadlo");
			Console.WriteLine("+++ Updated students:");
			updatedStudents.PrintAll();
		}
	}

}
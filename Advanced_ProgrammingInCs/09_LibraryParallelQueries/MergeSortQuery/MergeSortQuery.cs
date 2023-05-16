using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.IO;
using System.Linq;

using LibraryModel;

namespace MergeSortQuery {
	class MergeSortQuery {
		public Library Library { get; set; }
		public int ThreadCount { get; set; }

		public List<Copy> ExecuteQuery() {
			if (ThreadCount == 0) throw new InvalidOperationException("Threads property not set and default value 0 is not valid.");
			
			/* 	My code here:
				We have N threads, so we need to sort the selected list of copies helps merge sort algorith with N threads.
				Steps:
				1. Filter the list of copies to get only the copies that are on loan and are on shelf ending with A - Q letters.	
				2. Sorted by:
					2.1. DueDate
					2.2. Last name
					2.3. First name
					2.4. Shelf
					2.5. Copy ID
			*/
			var selectedCopies = Library.Copies.Where(
								 c => c.OnLoan != null && 
								 Regex.IsMatch(c.Book.Shelf, @"\w+[A-Q]$")
								 ).ToList();

			return selectedCopies.ToList();
		}
	}

	class ResultVisualizer {
		public static void PrintCopy(StreamWriter writer, Copy c) {
			writer.WriteLine("{0} {1}: {2} loaned to {3}, {4}.", c.OnLoan.DueDate.ToShortDateString(), c.Book.Shelf, c.Id, c.OnLoan.Client.LastName, System.Globalization.StringInfo.GetNextTextElement(c.OnLoan.Client.FirstName));
		}
	}
}

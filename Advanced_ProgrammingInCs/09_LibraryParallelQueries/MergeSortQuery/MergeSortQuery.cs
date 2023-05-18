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
		private int SortThreadCount { get; set; }
        private int MergeThreadCount { get; set; }

        public List<Copy> ExecuteQuery() {
			if (ThreadCount == 0) throw new InvalidOperationException("Threads property not set and default value 0 is not valid.");

            /*	We have N threads, so we need to sort the selected list of copies helps merge sort algorith with N threads.
				Steps:
				1. Filter the list of copies to get only the copies that are on loan and are on shelf ending with A - Q letters.	
				2. Sorted by:
					2.1. DueDate
					2.2. Last name
					2.3. First name
					2.4. Shelf
					2.5. Copy ID
			*/
            MergeThreadCount = ThreadCount;
			SortThreadCount = ThreadCount;
			return MergeSortThread(FilterCopies(Library.Copies), ThreadCount);
		}

		// My code here =========================

		private List<Copy> MergeSortThread(List<Copy> list, int threadCount)
		{
			int count = list.Count;

            if (count <= 1)
                return list;

            int mid = count / 2;

			// Separating list by threads count
            if (threadCount > 1)
            {
				List<Copy> left = new List<Copy>(), right = new List<Copy>();
				Thread leftThread = new Thread(() => left = MergeSortThread(list.GetRange(0, mid), threadCount / 2));
                Thread rightThread = new Thread(() => right = MergeSortThread(list.GetRange(mid, count - mid), threadCount - threadCount / 2));

                leftThread.Start();
                rightThread.Start();

                leftThread.Join();
                rightThread.Join();

                return Merge(left, right);
            }
            else
            {
				// Each Thread Sorting self part
				List<Copy> left = SortCopies(list.GetRange(0, mid));
                List<Copy> right = SortCopies(list.GetRange(mid, count - mid));
                return Merge(left, right);
            }
        }

        private List<Copy> Merge(List<Copy> left, List<Copy> right)
		{
			var result = new List<Copy>();
			var il = 0;
			var ir = 0;

			while (il < left.Count && ir < right.Count)
			{
				var addingCopy = CompareCopies(left[il], right[ir]) == -1 ? left[il++] : right[ir++];
				result.Add(addingCopy);
			}
			result.AddRange(right.Skip(ir));
			result.AddRange(left.Skip(il));
			return result;
		}

		private int CompareCopies(Copy left, Copy right) =>
					(left.OnLoan.DueDate,
					left.OnLoan.Client.LastName,
					left.OnLoan.Client.FirstName,
					left.Book.Shelf,
					left.Id)
					.CompareTo((
					right.OnLoan.DueDate,
					right.OnLoan.Client.LastName,
					right.OnLoan.Client.FirstName,
					right.Book.Shelf,
					right.Id));

        private List<Copy> SortCopies(List<Copy> l)
		{
			var source = from c in l
						 let client = c.OnLoan.Client
                         orderby c.OnLoan.DueDate, client.LastName, client.FirstName, c.Book.Shelf, c.Id
                         select c;
			return source.ToList();
        }

		private List<Copy> FilterCopies(List<Copy> l)
		{
            var source = from c in Library.Copies
                         where c.State == CopyState.OnLoan &&
						 c.Book.Shelf[2] >= 'A' && c.Book.Shelf[2] <= 'Q'
						 select c;
			return source.ToList();
        }

		//=================================
	}

	class ResultVisualizer {
		public static void PrintCopy(StreamWriter writer, Copy c) {
			writer.WriteLine("{0} {1}: {2} loaned to {3}, {4}.", c.OnLoan.DueDate.ToShortDateString(), c.Book.Shelf, c.Id, c.OnLoan.Client.LastName, System.Globalization.StringInfo.GetNextTextElement(c.OnLoan.Client.FirstName));
		}
	}
}

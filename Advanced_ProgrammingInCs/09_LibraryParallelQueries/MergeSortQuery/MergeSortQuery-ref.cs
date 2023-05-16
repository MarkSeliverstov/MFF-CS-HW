using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.IO;

using LibraryModel;

namespace MergeSortQuery {

	public class Lock
	{

	}
	class MergeSortQuery {
		public Library Library { get; set; }
		public int ThreadCount { get; set; }
		List<Copy> copyList = new List<Copy>();
		public List<Thread> filteringThreadPool= new List<Thread>();
		Queue<int> filterParts = new Queue<int>();
		public int currentIndex = 0;
		public Lock filterLock = new Lock();
		public int SortThreadCount { get; set; }
		public int mergeThreadCount { get; set; }

		public void FilterThreads()
		{
			filterParts.Enqueue(Library.Copies.Count);
			SortThreadCount = ThreadCount;
			mergeThreadCount = ThreadCount;
			while (ThreadCount > 1 && filterParts.Peek() > 1)
			{
				int number = filterParts.Dequeue();
				int half = number / 2;
				int otherHalf = number - half;
				filterParts.Enqueue(half);
				filterParts.Enqueue(otherHalf);
				ThreadCount--;
			}
			while (filterParts.Count > 1)
			{
				Thread thread = new Thread(new ParameterizedThreadStart(Filter));
				thread.Start(filterParts.Dequeue());
				filteringThreadPool.Add(thread);
			}
			Filter(filterParts.Dequeue());

		}

		public void Filter(object o)
		{
			int count = (int)o;
			int number = currentIndex;
			lock (filterLock)
			{
				number = currentIndex;
				currentIndex += count;
			}
			for (int i = number; i < number + count; i++)
			{
				if (Library.Copies[i] != null && Library.Copies[i].OnLoan != null && (int)Library.Copies[i].Book.Shelf[2] >= 65 && (int)Library.Copies[i].Book.Shelf[2] <= 81)
				{
					copyList.Add(Library.Copies[i]);
				}
			}
		}

		Queue<List<Copy>> sortParts = new Queue<List<Copy>>();
		Queue<List<Copy>> mergeParts = new Queue<List<Copy>>();
		Lock mergeLock = new Lock();
		List<Thread> sortingThreadPool = new List<Thread>();
		List<Thread> mergingThreadPool = new List<Thread>();
		int running = 0;
		

		public List<Copy> SortThreads()
		{ 
			sortParts.Enqueue(copyList);
			while (SortThreadCount > 1 && sortParts.Peek().Count > 1)
			{ 
				var sortPart = sortParts.Dequeue();
				int half = sortPart.Count / 2;
				int otherHalf = sortPart.Count - half;
				sortParts.Enqueue(sortPart.GetRange(0,half));
				sortParts.Enqueue(sortPart.GetRange(half,otherHalf));
				SortThreadCount--;
			}
			while (sortParts.Count > 1)
			{
				Thread sortThread = new Thread(new ParameterizedThreadStart(Sort));
				sortThread.Start(sortParts.Dequeue());
				sortingThreadPool.Add(sortThread);
			}
			Sort(sortParts.Dequeue());
			for (int i = 0; i < mergeThreadCount / 2; i++)
			{
				Thread mergeThread = new Thread(new ThreadStart(Merge));
				mergingThreadPool.Add(mergeThread);
			}
			foreach (var thread in sortingThreadPool)
			{
				thread.Join();
			}
			lock (mergeParts)
			{
				while (mergeParts.Count > 1 || running > 0)
				{
					running = 0;
					foreach (var thread in mergingThreadPool)
					{
						if (thread.ThreadState == ThreadState.Running)
						{
							running++;
						}
						else if (mergeParts.Count > 1)
						{
							thread.Start();
						}
					}
				}
			}
			foreach (var thread in mergingThreadPool)
			{ 
				thread.Join();
			}
			return mergeParts.Dequeue();
		}

		public void Sort(object o)
		{
			List<Copy> copies = (List<Copy>)o;
			copies.Sort(Compare);
			lock (mergeParts)
			{
				mergeParts.Enqueue(copies);
			}
		}

		public void Merge()
		{
			List<Copy> copies1, copies2;
			int index1 = 0, index2 = 0;
			lock (mergeLock)
			{
				copies1 = mergeParts.Dequeue();
				copies2 = mergeParts.Dequeue();
			}
			List<Copy> copiesMerged = new List<Copy>();
			while(true)
			{
				if (Compare(copies1[index1], copies2[index2]) == -1)
				{
					copiesMerged.Add(copies1[index1]);
					index1++;
					if (index1 == copies1.Count)
					{
						for (int i = index2; i < copies2.Count; i++)
						{
							copiesMerged.Add(copies2[index2]);
						}
						break;
					}
				}
				else
				{
					copiesMerged.Add(copies2[index2]);
					index2++;
					if (index2 == copies2.Count)
					{
						for (int i = index1; i < copies1.Count; i++)
						{
							copiesMerged.Add(copies1[index1]);
						}
						break;
					}
				}
			}
			lock (mergeParts)
			{
				mergeParts.Enqueue(copiesMerged);
			}

		}

		public int Compare(Copy x, Copy y)
		{
			if (x == null || y == null) return 0;
			if (DateTime.Compare(x.OnLoan.DueDate, y.OnLoan.DueDate) == 0)
			{
				if (string.Compare(x.OnLoan.Client.LastName, y.OnLoan.Client.LastName) == 0)
				{
					if (string.Compare(x.OnLoan.Client.FirstName, y.OnLoan.Client.FirstName) == 0)
					{
						if (string.Compare(x.Book.Shelf, y.Book.Shelf) == 0)
						{
							return (string.Compare(x.Id, y.Id));
						}
						else return (string.Compare(x.Book.Shelf, y.Book.Shelf));
					}
					else return (string.Compare(x.OnLoan.Client.FirstName, y.OnLoan.Client.FirstName));
				}
				else return (string.Compare(x.OnLoan.Client.LastName, y.OnLoan.Client.LastName));
			}
			else return (DateTime.Compare(x.OnLoan.DueDate, y.OnLoan.DueDate));
			/*
			return (x?.OnLoan?.DueDate, x?.OnLoan?.Client?.LastName, x?.OnLoan?.Client?.FirstName, x?.Book?.Shelf, x?.Id)
        			.CompareTo((y?.OnLoan?.DueDate, y?.OnLoan?.Client?.LastName, y?.OnLoan?.Client?.FirstName, y?.Book?.Shelf, y?.Id));
			*/
		}

		


		public List<Copy> ExecuteQuery() 
		{
			if (ThreadCount == 0) throw new InvalidOperationException("Threads property not set and default value 0 is not valid.");
			if (ThreadCount > 1)
			{ 
				FilterThreads();
				foreach (var thread in filteringThreadPool)
				{ 
					thread.Join();
				}
				copyList = SortThreads();
			}
			else
			{
				foreach (var copy in Library.Copies)
				{ 
					if (copy.OnLoan != null && (int)copy.Book.Shelf[2] >= 65 && (int)copy.Book.Shelf[2] <= 81)
					{
						copyList.Add(copy);
					}
				}
				copyList.Sort(Compare);
			}

			return copyList;
		}
	}

	class ResultVisualizer {
		public static void PrintCopy(StreamWriter writer, Copy c) {
			writer.WriteLine("{0} {1}: {2} loaned to {3}, {4}.", c.OnLoan.DueDate.ToShortDateString(), c.Book.Shelf, c.Id, c.OnLoan.Client.LastName, System.Globalization.StringInfo.GetNextTextElement(c.OnLoan.Client.FirstName));
		}
	}
}

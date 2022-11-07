using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace NezarkaBookstore
{
	//
	// Model
	//

	class ModelStore {
		private List<Book> books = new List<Book>();
		private List<Customer> customers = new List<Customer>();

		public IList<Book> GetBooks() {
			return books;
		}

		public Book GetBook(int id) {
			return books.Find(b => b.Id == id);
		}

		public Customer GetCustomer(int id) {
			return customers.Find(c => c.Id == id);
		}

		public static ModelStore LoadFrom(TextReader reader) {
			var store = new ModelStore();

			try {
				if (reader.ReadLine() != "DATA-BEGIN") {
					return null;
				}
				while (true) {
					string line = reader.ReadLine();
					if (line == null) {
						return null;
					} else if (line == "DATA-END") {
						break;
					}

					string[] tokens = line.Split(';');
					switch (tokens[0]) {
						case "BOOK":
							store.books.Add(new Book {
								Id = int.Parse(tokens[1]), Title = tokens[2], Author = tokens[3], Price = decimal.Parse(tokens[4])
							});
							break;
						case "CUSTOMER":
							store.customers.Add(new Customer {
								Id = int.Parse(tokens[1]), FirstName = tokens[2], LastName = tokens[3]
							});
							break;
						case "CART-ITEM":
							var customer = store.GetCustomer(int.Parse(tokens[1]));
							if (customer == null) {
								return null;
							}
							customer.ShoppingCart.Items.Add(new ShoppingCartItem {
								BookId = int.Parse(tokens[2]), Count = int.Parse(tokens[3])
							});
							break;
						default:
							return null;
					}
				}
			} catch (Exception ex) {
				if (ex is FormatException || ex is IndexOutOfRangeException) {
					return null;
				}
				throw;
			}

			return store;
		}
	}

	class Book {
		public int Id { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public decimal Price { get; set; }
	}

	class Customer {
		private ShoppingCart shoppingCart;

		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public ShoppingCart ShoppingCart {
			get {
				if (shoppingCart == null) {
					shoppingCart = new ShoppingCart(){CustomerId = Id};
				}
				return shoppingCart;
			}
			set {
				shoppingCart = value;
			}
		}
	}

	class ShoppingCartItem {
		public int BookId { get; set; }
		public int Count { get; set; }
	}

	class ShoppingCart {
		public int CustomerId { get; set; }
		public List<ShoppingCartItem> Items = new List<ShoppingCartItem>();
	}

	//
	// Program
	//

	public class Program{
		public static void Main(string[] args){
			TextReader tr = new StreamReader("NezarkaTest.in");
			TextWriter tw = new StreamWriter("ex.out");
			// TextReader tr = Console.In;
			// TextWriter tw = Console.Out;
			ModelStore bookStore = ModelStore.LoadFrom(tr);
			if (bookStore == null){
				tw.WriteLine("Data error.");
				return;
			}

			WriterHTML_tmp writer = new WriterHTML_tmp(tw);
			Controler.RunCmds(ref bookStore, tr);
			tw.Close();
		}
	}

	class Command{
		public string name { get; set; }
		public int custId { get; set; }
		public int bookId { get; set; }
	}

	class Controler{
		public static void RunCmds(ref ModelStore store, TextReader reader){
			
            string line = reader.ReadLine();

			while (line != null){

				Command cmd = ParserCmd.PraseLineToCmd(line);
                WriterHTML_tmp.Head();

                try{
					Customer c = store.GetCustomer(cmd.custId);
                    string Fname = c.FirstName;
					var cart = c.ShoppingCart;
                    int  countCart = cart.Items.Count;

                    switch (cmd.name) {
						case "/Books":
							WriterHTML_tmp.StyleAndMenu(countCart, Fname);
							WriterHTML_tmp.BooksListing(store.GetBooks());
							break;
						case "/Books/Detail":
							if (!CheckBook(store, cmd.bookId))
								WriterHTML_tmp.InvalidRequest();
							else{
								WriterHTML_tmp.StyleAndMenu(countCart, Fname);
								WriterHTML_tmp.BookInfo(store.GetBook(cmd.bookId));
							}
							break;	
						case "/ShoppingCart":
							WriteShopCart(store, cmd.custId);
							break;
						case "/ShoppingCart/Add":
							if (!AddBookToCart(ref store, cmd.bookId, cmd.custId))
								WriterHTML_tmp.InvalidRequest();
							else
								WriteShopCart(store, cmd.custId);
							break;
						case "/ShoppingCart/Remove":
							if (!RemoveBookToCart(ref store, cmd.bookId, cmd.custId))
								WriterHTML_tmp.InvalidRequest();
							else
								WriteShopCart(store, cmd.custId);
							break;
						case "/InvalidRequest":
							WriterHTML_tmp.InvalidRequest();
							break;
				    }
                }
                catch(Exception){
                    WriterHTML_tmp.InvalidRequest();
                }
				WriterHTML_tmp.EndOfHTML();
				line = reader.ReadLine();
			}
		}

		private static bool CheckBook(ModelStore store, int id){
			Book bb = store.GetBook(id);
			if (bb == null){
				return false;
			}
			return true;
		}

		private static bool AddBookToCart(ref ModelStore store, int bookId, int custId){
			bool inList = false;
			var cart = store.GetCustomer(custId).ShoppingCart;
			foreach (ShoppingCartItem item in cart.Items){
				if (item.BookId == bookId){
					item.Count++;
					inList = true;
				}
			}
			if (inList == false){
				if (!CheckBook(store, bookId))
					return false;
				cart.Items.Add(new ShoppingCartItem { BookId = bookId, Count = 1 });
				return true;
			}
			else
				return true;
		}

		private static bool RemoveBookToCart(ref ModelStore store, int bookId, int custId){
			bool inListR = false;
			var cart = store.GetCustomer(custId).ShoppingCart;
			foreach (ShoppingCartItem item in cart.Items){
				if (item.BookId == bookId){
					inListR = true;
					item.Count--;
					if (item.Count == 0){
						cart.Items.Remove(item);
					}
					break;
				}
			}
			if (inListR)
				return true;
			else
				return false;
		}

		private static void WriteShopCart(ModelStore store, int id){
			string Fname = store.GetCustomer(id).FirstName;
			int countCart = store.GetCustomer(id).ShoppingCart.Items.Count;
			
			WriterHTML_tmp.StyleAndMenu(countCart, Fname);
			if (countCart == 0)
				WriterHTML_tmp.EmptyCart();
			else
				WriterHTML_tmp.CartContents(store, id);
		}
	}

	class ParserCmd{
		private static bool Check_line(string[] partsOfLine, string[] partsOfCmd){
			if(partsOfLine.Length != 3
			   || !partsOfLine[0].Equals("GET")
			   || !(partsOfLine[2].Substring(0, 22)).Equals("http://www.nezarka.net")
			   || partsOfCmd.Length < 3
			   || partsOfCmd.Length > 5
			   || partsOfCmd.Length == 4)
			   {
				return false;
			   }
			return true;
		}

		public static Command PraseLineToCmd(string line){
			Command cmd = new Command();
			cmd.name = "/InvalidRequest";
			try{
				string[] partsOfLine = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				string[] partsOfCmd = partsOfLine[2].Split('/', StringSplitOptions.RemoveEmptyEntries);

				if (!Check_line(partsOfLine, partsOfCmd))
					return cmd;

				cmd.custId = int.Parse(partsOfLine[1]);

				if(partsOfCmd[2].Equals("Books")){ // http://www.nezarka.net/Books ...
					if (partsOfCmd.Length == 3){
						cmd.name = "/Books";
					}
					else if (partsOfCmd.Length == 5){ // http://www.nezarka.net/Books/Detail/<int> ...
						if (partsOfCmd[3].Equals("Detail")){
							cmd.bookId = int.Parse(partsOfCmd[4]);
							cmd.name = "/Books/Detail";
						}
					}
				}
				else if(partsOfCmd[2].Equals("ShoppingCart")){ // http://www.nezarka.net/ShoppingCart ...
					if (partsOfCmd.Length == 3){
						cmd.name = "/ShoppingCart";
					}
					else if (partsOfCmd.Length == 5){ // http://www.nezarka.net/Books/Detail/<int> ...
						if (partsOfCmd[3].Equals("Add")){
							cmd.bookId = int.Parse(partsOfCmd[4]);
							cmd.name = "/ShoppingCart/Add"; 
						}
						else if(partsOfCmd[3].Equals("Remove")){
							cmd.bookId = int.Parse(partsOfCmd[4]);
							cmd.name = "/ShoppingCart/Remove"; 
						}
					}
				}
			}
			catch (Exception){
				cmd.name = "/InvalidRequest";
			}
			return cmd;
		}
	}

	class WriterHTML_tmp{
		private static TextWriter writer;
		
		public WriterHTML_tmp(TextWriter wr){
			writer = wr;
		}

        public static void Head(){
            writer.WriteLine("<!DOCTYPE html>");
			writer.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
			writer.WriteLine("<head>");
			writer.WriteLine("    <meta charset=\"utf-8\" />");
			writer.WriteLine("    <title>Nezarka.net: Online Shopping for Books</title>");
			writer.WriteLine("</head>");
        }
        
        public static void StyleAndMenu(int cartCount, string name){
            writer.WriteLine("<body>");
			writer.WriteLine("    <style type=\"text/css\">");
            writer.WriteLine("        table, th, td {");
            writer.WriteLine("            border: 1px solid black;");
            writer.WriteLine("            border-collapse: collapse;");
            writer.WriteLine("        }");
            writer.WriteLine("        table {");
            writer.WriteLine("            margin-bottom: 10px;");
            writer.WriteLine("        }");
            writer.WriteLine("        pre {");
            writer.WriteLine("            line-height: 70%;");
            writer.WriteLine("        }");
            writer.WriteLine("    </style>");
            writer.WriteLine("    <h1><pre>  v,<br />Nezarka.NET: Online Shopping for Books</pre></h1>");
            writer.WriteLine("    {0}, here is your menu:", name);
            writer.WriteLine("    <table>");
            writer.WriteLine("        <tr>");
            writer.WriteLine("            <td><a href=\"/Books\">Books</a></td>");
            writer.WriteLine("            <td><a href=\"/ShoppingCart\">Cart ({0})</a></td>", cartCount);
            writer.WriteLine("        </tr>");
            writer.WriteLine("    </table>");
        }

        public static void EmptyCart(){
            writer.WriteLine("    Your shopping cart is EMPTY.");
            WriteHTML_End();
        }
        
        public static void WriteHTML_End(){
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }

        public static void BooksListing(IList<Book> books){
			int count = books.Count;
			int booksInLine = 3;
			int countOfRow = books.Count/booksInLine;
			int currentBook = 0;

            writer.WriteLine("    Our books for you:");
            writer.WriteLine("    <table>");

			for (int i = 0; i < count/booksInLine; i++){
				writer.WriteLine("        <tr>");

				for (int j = 0; j < booksInLine; j++){
					WriteTableCell(books[currentBook]);
					currentBook++;
				}

				writer.WriteLine("        </tr>");
			}
			if (count%booksInLine != 0){
				writer.WriteLine("        <tr>");

				for (int j = 0; j < count%booksInLine; j++){
					WriteTableCell(books[currentBook]);
					currentBook++;
				}

				writer.WriteLine("        </tr>");
			}

			writer.WriteLine("    </table>");
			WriteHTML_End();
        }

		private static void WriteTableCell(Book book){
			writer.WriteLine("            <td style=\"padding: 10px;\">");
			writer.WriteLine("                <a href=\"/Books/Detail/{0}\">{1}</a><br />", 
																book.Id, book.Title);
			writer.WriteLine("                Author: {0}<br />", book.Author);
			writer.WriteLine("                Price: {0} EUR &lt;<a href=\"/ShoppingCart/Add/{1}\">Buy</a>&gt;", 
																book.Price, book.Id);
			writer.WriteLine("            </td>");
		}

        public static void BookInfo(Book book){
			writer.WriteLine("    Book details:");
			writer.WriteLine("    <h2>{0}</h2>", book.Title);
			writer.WriteLine("    <p style=\"margin-left: 20px\">");
			writer.WriteLine("    Author: {0}<br />", book.Author);
			writer.WriteLine("    Price: {0} EUR<br />", book.Price);
			writer.WriteLine("    </p>");
			writer.WriteLine("    <h3>&lt;<a href=\"/ShoppingCart/Add/{0}\">Buy this book</a>&gt;</h3>", book.Id);
			WriteHTML_End();
        }

        public static void CartContents(ModelStore store, int custId){
			ShoppingCart cart = store.GetCustomer(custId).ShoppingCart;
			int count = cart.Items.Count;
			decimal oneBooksPrice = 0;
			decimal totalPrice = 0;

			writer.WriteLine("    Your shopping cart:");
			writer.WriteLine("    <table>");
			writer.WriteLine("        <tr>");
			writer.WriteLine("            <th>Title</th>");
			writer.WriteLine("            <th>Count</th>");
			writer.WriteLine("            <th>Price</th>");
			writer.WriteLine("            <th>Actions</th>");
			writer.WriteLine("        </tr>");

			foreach (ShoppingCartItem item in cart.Items){
				
				oneBooksPrice= item.Count * store.GetBook(item.BookId).Price;
				totalPrice += oneBooksPrice;

				writer.WriteLine("        <tr>");
				writer.WriteLine("            <td><a href=\"/Books/Detail/{0}\">{1}</a></td>",
									item.BookId, store.GetBook(item.BookId).Title);
				writer.WriteLine("            <td>{0}</td>", item.Count);

				if (item.Count > 1)
					writer.WriteLine("            <td>{0} * {1} = {2} EUR</td>", 
						item.Count, store.GetBook(item.BookId).Price, oneBooksPrice);
				else
					writer.WriteLine("            <td>{0} EUR</td>", oneBooksPrice);

				writer.WriteLine("            <td>&lt;<a href=\"/ShoppingCart/Remove/{0}\">Remove</a>&gt;</td>", item.BookId);
				writer.WriteLine("        </tr>");
			}

			writer.WriteLine("    </table>");
			writer.WriteLine("    Total price of all items: {0} EUR", totalPrice);
			WriteHTML_End();
        }

        public static void InvalidRequest(){
            writer.WriteLine("<body>");
            writer.WriteLine("<p>Invalid request.</p>");
            WriteHTML_End();
        }

		public static void EndOfHTML(){
			writer.WriteLine("====");
		}
	}
}

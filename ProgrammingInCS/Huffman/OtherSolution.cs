using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace HuffmanOne2014 {

	public class HuffmanTree {
		public Node RootNode { get; private set; }

		private HuffmanTree() { }

		public static HuffmanTree CreateFromSymbolCounts(long[] symbolCounts) {
			if (symbolCounts.Length != 256) throw new ArgumentException("Count for all symbols (all 256 possible byte values) must be always provided.", "symbolCounts");

			var tree = new HuffmanTree();

			var treeBuilder = new TreeBuilder(symbolCounts);
			tree.RootNode = treeBuilder.BuildTree();

			return tree;
		}

		public void PrintInPreorder(TextWriter writer) {
			Stack<Node> stack = new Stack<Node>();
			stack.Push(RootNode);

			while (stack.Count > 0) {
				Node currentNode = stack.Pop();

				if (currentNode.Left == null /* || currentNode.Right == null, which is implied */) {
					writer.Write('*');
					writer.Write(currentNode.Symbol);
					writer.Write(':');
				} else {
					stack.Push(currentNode.Right);
					stack.Push(currentNode.Left);
				}

				writer.Write(currentNode.Weight);

				if (stack.Count > 0) {
					writer.Write(' ');
				}
			}
		}

		private class TreeBuilder {
			private Queue<Node> leafQueue;
			private Queue<Node> innerNodeQueue;

			public TreeBuilder(long[] symbolCounts) {
				CreateLeafQueue(symbolCounts);
				innerNodeQueue = new Queue<Node>();
			}

			private void CreateLeafQueue(long[] symbolCounts) {
				var leafNodes = new List<Node>();

				for (int i = 0; i < symbolCounts.Length; i++) {
					if (symbolCounts[i] > 0) {
						leafNodes.Add(Node.CreateLeaf(symbolCounts[i], (byte) i));
					}
				}

				leafNodes.Sort();

				leafQueue = new Queue<Node>(leafNodes);
			}

			/// <summary>Generate new Huffman tree based on symbol counts passed to contructor.</summary>
			/// <returns>Root node of the newly created Huffman tree.</returns>
			public Node BuildTree() {
				while (leafQueue.Count > 0 || innerNodeQueue.Count > 1) {
					Node min1 = GetNextMinimalNode();
					Node min2 = GetNextMinimalNode();

					Node node = Node.CreateIntermediate(
						weight: min1.Weight + min2.Weight,
						left: min1,
						right: min2
					);

					innerNodeQueue.Enqueue(node);
				}

				return innerNodeQueue.Dequeue();
			}

			private Node GetNextMinimalNode() {
				if (innerNodeQueue.Count == 0 || (leafQueue.Count > 0 && leafQueue.Peek().Weight <= innerNodeQueue.Peek().Weight)) {
					return leafQueue.Dequeue();
				} else {
					return innerNodeQueue.Dequeue();
				}
			}
		}

		public class Node : IComparable<Node> {
			/// <summary>Reference to left child node. Is always <code>null</code> for leaf nodes.</summary>
			public Node Left { get; private set; }

			/// <summary>Reference to right child node. Is always <code>null</code> for leaf nodes.</summary>
			public Node Right { get; private set; }
			public long Weight { get; private set; }

			/// <summary>A byte value this node represents. This property has a valid value only for for leaf nodes and has no meaning in intermediate nodes.</summary>
			public byte Symbol { get; private set; }

			private Node() { }

			private static void ThrowIfWeightInvalid(long weight) {
				if (weight <= 0) throw new ArgumentOutOfRangeException("weight", "Weight of any node must be positive non-zero number.");
			}

			public static Node CreateIntermediate(long weight, Node left, Node right) {
				if (left == null) throw new ArgumentNullException("left");
				if (right == null) throw new ArgumentNullException("right");
				ThrowIfWeightInvalid(weight);
	
				return new Node {
					Weight = weight,
					Symbol = 0,
					Left = left,
					Right = right
				};
			}
			
			public static Node CreateLeaf(long weight, byte symbol) {
				ThrowIfWeightInvalid(weight);

				return new Node {
					Weight = weight,
					Symbol = symbol,
					Left = null,
					Right = null
				};
			}

			/// <summary>Compare leaf nodes acording to Huffman rules. Warning: This method defines a valid relationship only between two leaf nodes.</summary>
			/// <exception cref="System.InvalidOperationException">Thrown when <code>this</code> or <code>node</code> argument represent intermediate nodes.</exception>
			public int CompareTo(Node node) {
				if (this.Left != null || node.Left != null) {	// Note: || this.Right != null || node.Right != null is implied!
					throw new InvalidOperationException("CompareTo method can be only called on and only with leaf nodes.");
				}

				if (this.Weight == node.Weight) {
					return this.Symbol.CompareTo(node.Symbol);
				} else {
					return this.Weight.CompareTo(node.Weight);
				}
			}
		}

	}

	public class Huffman {
		private static long[] CountSymbolsInStream(Stream s) {
			long[] counts = new long[256];

			int symbol;
			while ((symbol = s.ReadByte()) != -1) {
				counts[symbol]++;
			}

			return counts;
		}

		public static void Encode(Stream inStream /*, Stream outStream */) {
			if (!inStream.CanRead) throw new ArgumentException("Stream does not support reading.", "inStream");
			if (!inStream.CanSeek) throw new ArgumentException("Stream does not support seeking.", "inStream");

			/*
			if (!outStream.CanWrite) throw new ArgumentException("Stream does not support writing.", "outStream");
			*/

			long[] counts = CountSymbolsInStream(inStream);

			var huffmanTree = HuffmanTree.CreateFromSymbolCounts(counts);

			huffmanTree.PrintInPreorder(Console.Out);
		}
	}

	class Program {
		static void Main(string[] args) {
			if (args.Length != 1 || args[0] == "") {
				Console.WriteLine("Argument Error");
				return;
			}

			FileStream fin = null;
			FileStream fout = null;

			try {
				try {
					fin = new FileStream(args[0], FileMode.Open);
				} catch (ArgumentException argEx) {
					throw new FileNotFoundException("Invalid file name passed in args[0].", argEx);
				}

				Huffman.Encode(fin);

			} catch (Exception ex) {
				if (ex is FileNotFoundException ||
					ex is IOException ||
					ex is UnauthorizedAccessException ||
					ex is System.Security.SecurityException
				) {
					Console.WriteLine("File Error");
				} else {
					throw;
				}
			} finally {
				if (fout != null) fout.Close();
				if (fin != null) fin.Close();
			}
		}
	}

}
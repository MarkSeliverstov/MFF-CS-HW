using Microsoft.VisualBasic;
using System.Reflection.Emit;
using System.Text;

namespace MinimalisticUIFramework {
	public abstract class Control {}

	public sealed class Image : Control {
		public string Url { get; private set; } = "";
		public float ZoomFactor { get; private set; } = 1.0f;

		public Image WithUrl(string url) { Url = url; return this; }
		public Image WithZoomFactor(float zoomFactor) { ZoomFactor = zoomFactor; return this; }

		public override string ToString() => $"Image {{ Url = \"{Url}\", ZoomFactor = {ZoomFactor} }}";
	}

	public sealed class Label : Control {
		public string Text { get; private set; } = "";

		public Label WithText(string text) { Text = text; return this; }

		public override string ToString() => $"Label {{ Text = \"{Text}\" }}";
	}

	public class Point{
		public int X { get; set; }
		public int Y { get; set; }

		public override string ToString() => $"{X}, {Y}";
	}

	public sealed class StackPanel : Control {
		private List<Control> childs = new List<Control>();
		public void AddChild(Control child) => childs.Add(child);

		public override string ToString() {
			var sb = new StringBuilder();
			sb.AppendLine("StackPanel {");
			foreach (var child in childs) {
				sb.AppendLine($"{child}");
			}
			sb.Append("}");
			return sb.ToString();
		}
	}

	public sealed class Canvas{
		private List<Control> childs = new List<Control>();
		private List<Point> positions = new List<Point>();
		
		public void AddChild(Control child, Point position) {
			childs.Add(child);
			positions.Add(position);
		}

		public override string ToString() {
			var sb = new StringBuilder();
			sb.AppendLine("Canvas {");
			for (int i = 0; i < childs.Count; i++) {
				sb.AppendLine($"{childs[i]} at {positions[i]}");
			}
			sb.Append("}");
			return sb.ToString();
		}
	}

	public static class Placer{
		// For StackPanel
		public static T PlacedIn<T>(this T control, StackPanel stackPanel) where T : Control {
			switch (control) {
				case Image image:
					stackPanel.AddChild(image);
					return control;
				case Label label:
					stackPanel.AddChild(label);
					return control;
				case StackPanel stackPanel2:
					stackPanel.AddChild(stackPanel2);
					return control;
				default:
					throw new Exception("This type of control is not supported");
			}
		}

		// For Canvas
		public static Tuple<Canvas, T> PlacedIn<T>(this T control, Canvas canvas) where T : Control {
			switch (control) {
				case Image image: return new Tuple<Canvas, T>(canvas, control);
				case Label label: return new Tuple<Canvas, T>(canvas, control);
				case StackPanel stackPanel: return new Tuple<Canvas, T>(canvas, control);
				default: throw new Exception("This type of control is not supported");
			}
		}

		public static T At<T>(this Tuple<Canvas, T> tuple, int x, int y) where T : Control {
			var position = new Point { X = x, Y = y };

			switch (tuple.Item2) {
				case Image image:
					tuple.Item1.AddChild(image, position);
					return tuple.Item2;
				case Label label:
					tuple.Item1.AddChild(label, position);
					return tuple.Item2;
				case StackPanel stackPanel:
					tuple.Item1.AddChild(stackPanel, position);
					return tuple.Item2;
				default:
					throw new Exception("This type of control is not supported");
			}
		}

	}
}
namespace MinimalisticUIFramework {
	public abstract class Control {
	}

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
}
using System;

namespace ChromaSDK.Stream
{
	public class Default
	{
		private static string GetDefaultString(uint length)
		{
			string text = string.Empty;
			for (uint num = 0U; num < length; num += 1U)
			{
				text += " ";
			}
			return text;
		}

		private const uint LENGTH_SHORTCODE = 6U;

		private const uint LENGTH_STREAM_ID = 48U;

		private const uint LENGTH_STREAM_KEY = 48U;

		private const uint LENGTH_STREAM_FOCUS = 48U;

		public static readonly string Shortcode = Default.GetDefaultString(6U);

		public static readonly string StreamId = Default.GetDefaultString(48U);

		public static readonly string StreamKey = Default.GetDefaultString(48U);

		public static readonly string StreamFocus = Default.GetDefaultString(48U);
	}
}

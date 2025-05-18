using System;

internal static class Tags
{
	public static readonly Tags.Open S1 = new Tags.Open
	{
		Opening = "<shake a=0.1>"
	};

	public static readonly Tags.Open S2 = new Tags.Open
	{
		Opening = "<shake a=0.2>"
	};

	public static readonly Tags.Open S3 = new Tags.Open
	{
		Opening = "<shake a=0.3>"
	};

	public class OpenClose
	{
		public string Opening;

		public string Closing;
	}

	public class Open
	{
		public string Opening;
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Yarn.Markup;

public static class Markup
{
	public static string Parse(MarkupParseResult markup)
	{
		Markup.sb.Clear().Append(markup.Text);
		List<MarkupAttribute> attributes = markup.Attributes;
		for (int i = attributes.Count - 1; i >= 0; i--)
		{
			MarkupAttribute markupAttribute = attributes[i];
			KeyValue keyValue;
			if (Enum.TryParse<KeyValue>(markupAttribute.Name, true, out keyValue))
			{
				switch (keyValue)
				{
				case KeyValue.C0:
					Markup.sb.Insert(markupAttribute.Position + markupAttribute.Length, "</color>").Insert(markupAttribute.Position, "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.EMPHASIS) + ">");
					break;
				case KeyValue.C1:
					Markup.sb.Insert(markupAttribute.Position + markupAttribute.Length, "</color>").Insert(markupAttribute.Position, "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE) + ">");
					break;
				case KeyValue.C2:
					Markup.sb.Insert(markupAttribute.Position + markupAttribute.Length, "</color>").Insert(markupAttribute.Position, "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE) + ">");
					break;
				case KeyValue.C3:
					Markup.sb.Insert(markupAttribute.Position + markupAttribute.Length, "</color>").Insert(markupAttribute.Position, "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL) + ">");
					break;
				case KeyValue.S1:
					Markup.sb.Insert(markupAttribute.Position, Tags.S1.Opening);
					break;
				case KeyValue.S2:
					Markup.sb.Insert(markupAttribute.Position, Tags.S2.Opening);
					break;
				case KeyValue.S3:
					Markup.sb.Insert(markupAttribute.Position, Tags.S3.Opening);
					break;
				}
			}
		}
		return Markup.sb.ToString();
	}

	private static readonly StringBuilder sb = new StringBuilder(128);
}

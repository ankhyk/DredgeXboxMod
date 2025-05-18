using System;
using AeLa.EasyFeedback;

public class FakedFeedbackFormValues : FormElement
{
	protected override void FormClosed()
	{
	}

	protected override void FormOpened()
	{
	}

	protected override void FormSubmitted()
	{
		this.Form.CurrentReport.List.id = this.Form.Config.Board.CategoryIds[0];
		this.Form.CurrentReport.List.name = this.Form.Config.Board.CategoryNames[0];
	}
}

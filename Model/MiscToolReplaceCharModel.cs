using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace SDPromptTool.Model
{
	public class MiscToolReplaceCharModel
	{
		public string ReplaceFrom { get; set; }
		public string ReplaceTo { get; set; }
		public string TextBefore { get; set; }
		public TextBox TextAfter { get; set; }
	}
}

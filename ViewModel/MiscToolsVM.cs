using CommunityToolkit.Mvvm.Input;
using SDPromptTool.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SDPromptTool.ViewModel
{
	public partial class MiscToolsVM
	{
		[RelayCommand]
		public void ReplaceChar(MiscToolReplaceCharModel input)
		{
			char[] caFrom = input.ReplaceFrom.ToCharArray();
			char[] caTo = input.ReplaceTo.ToCharArray();
			if (caFrom.Length != caTo.Length)
			{
				MessageBox.Show("Two values must have the same number of characters!");
				return;
			}
			string TextB4 = input.TextBefore;
			for (byte i = 0; i < caFrom.Length; i++)
				TextB4 = TextB4.Replace(caFrom[i], caTo[i]);
			input.TextAfter.Text = TextB4;
		}
	}
}

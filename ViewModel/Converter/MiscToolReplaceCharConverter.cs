using SDPromptTool.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SDPromptTool.ViewModel.Converter
{
	public class MiscToolReplaceCharConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => new MiscToolReplaceCharModel
		{
			ReplaceFrom = (string)values[0],
			ReplaceTo = (string)values[1],
			TextBefore = (string)values[2],
			TextAfter = (System.Windows.Controls.TextBox)values[3]
		};

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

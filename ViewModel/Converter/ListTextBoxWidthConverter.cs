using System;
using System.Globalization;
using System.Windows.Data;

namespace SDPromptTool.ViewModel.Converter
{
	public class ListTextBoxWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value - 40;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

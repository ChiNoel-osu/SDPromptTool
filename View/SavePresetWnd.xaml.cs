using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SDPromptTool.View
{
	/// <summary>
	/// SavePresetWnd.xaml 的交互逻辑
	/// </summary>
	public partial class SavePresetWnd : Window
	{
		public SavePresetWnd(bool IsNegative)
		{
			Tag = IsNegative;
			InitializeComponent();
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			SnapsToDevicePixels = true;	//STDP acts as a IsSaving flag.
			Close();
		}

		private void TB_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (((TextBox)sender).Text != string.Empty)
				Save.IsEnabled = true;
			else
				Save.IsEnabled = false;
		}
	}
}

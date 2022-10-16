using SDPromptTool.ViewModel;
using System.Collections;
using System.Windows;
using System.Windows.Input;

namespace SDPromptTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MainViewModel MainVM = new MainViewModel();
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if(Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				switch(e.Key)
				{
					case Key.D1:
						MainTab.SelectedIndex = 0;
						break;
					case Key.D2:
						MainTab.SelectedIndex = 1;
						break;
					case Key.D3:
						MainTab.SelectedIndex = 2;
						break;
					default:
						break;
				}
			}
		}
	}
}

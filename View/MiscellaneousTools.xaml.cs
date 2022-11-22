using System.Windows.Controls;

namespace SDPromptTool.View
{
	/// <summary>
	/// MiscellaneousTools.xaml 的交互逻辑
	/// </summary>
	public partial class MiscellaneousTools : UserControl
	{
		public MiscellaneousTools()
		{
			InitializeComponent();
			DataContext = MainWindow.MainVM;
		}
	}
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static SDPromptTool.ViewModel.SharedPresetVM;
//TODO: Ctrl+S in Prompts and Notes to save.
namespace SDPromptTool.View
{
	/// <summary>
	/// PresetManager.xaml 的交互逻辑
	/// </summary>
	public partial class PresetManager : UserControl
	{
		DispatcherTimer PSavedTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
		DispatcherTimer NSavedTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
		public PresetManager()
		{
			InitializeComponent();
			DataContext = MainWindow.MainVM;
		}
		#region Buttons
		private void PDelBtn_Click(object sender, RoutedEventArgs e)
		{   //Deletes the first occurrence of a preset that matches the name.
			switch (MessageBox.Show("Are you sure you want to delete [" + (string)((Button)sender).Tag + "]?\nThis cannot be undone.", "Delete preset.", MessageBoxButton.YesNo))
			{
				case MessageBoxResult.Yes:
					MainWindow.MainVM.Presets.DeletePreset((string)((Button)sender).Tag, false);
					break;
				case MessageBoxResult.No:
					break;
				default:
					break;
			}
		}
		private void NDelBtn_Click(object sender, RoutedEventArgs e)
		{
			switch (MessageBox.Show("Are you sure you want to delete [" + (string)((Button)sender).Tag + "]?\nThis cannot be undone.", "Delete preset.", MessageBoxButton.YesNo))
			{
				case MessageBoxResult.Yes:
					MainWindow.MainVM.Presets.DeletePreset((string)((Button)sender).Tag, true);
					break;
				case MessageBoxResult.No:
					break;
				default:
					break;
			}
		}
		private void PNewBtn_Click(object sender, RoutedEventArgs e)
		{
			SavePresetWnd wnd = new SavePresetWnd(false);
			wnd.Closed += SavePresetWndClosed;
			wnd.Show();
			PNewBtn.IsEnabled = NNewBtn.IsEnabled = false;
		}
		private void NNewBtn_Click(object sender, RoutedEventArgs e)
		{
			SavePresetWnd wnd = new SavePresetWnd(true);
			wnd.Closed += SavePresetWndClosed;
			wnd.Show();
			PNewBtn.IsEnabled = NNewBtn.IsEnabled = false;
		}
		private void PCopyBtn_Click(object sender, RoutedEventArgs e)
		{
			if (PPromptsBox.Text != string.Empty)
			{
				try
				{   //Rapidly clicking the copy button can cause catastrophic consequences.
					Clipboard.SetText(PPromptsBox.Text.Remove(PPromptsBox.Text.LastIndexOf(',')));
				}
				catch (ArgumentOutOfRangeException)
				{
					Clipboard.SetText(PPromptsBox.Text);
				}
			}
		}
		private void PClrBtn_Click(object sender, RoutedEventArgs e)
		{
			PPromptsBox.Clear();
			PNotes.Clear();
		}
		private void NCopyBtn_Click(object sender, RoutedEventArgs e)
		{
			if (NPromptsBox.Text != string.Empty)
				Clipboard.SetText(NPromptsBox.Text);
		}
		private void NClrBtn_Click(object sender, RoutedEventArgs e)
		{
			NPromptsBox.Clear();
			NNotes.Clear();
		}
		#region Save Button Logics
		private async void PSaveBtn_Click(object sender, RoutedEventArgs e)
		{
			Preset something = (Preset)PList.SelectedItem;
			int selectedIndex = PList.SelectedIndex;
			if (something is not null)
			{
				await MainWindow.MainVM.Presets.UpdatePreset(something.Name, false, PPromptsBox.Text, PNotes.Text);
				PSavedTimer.Tick += PSavedTimer_Tick;
				PSavedTimer.Start();    //Wait 1 sec (see start of this class) to play animation.
				PList.SelectedIndex = selectedIndex;    //TODO: This stopped working after implementing async.
				PSaveBtn.Foreground = new SolidColorBrush(Colors.Lime); //Set the text green.
			}
		}
		private void PSavedTimer_Tick(object sender, EventArgs e)
		{
			PSavedTimer.Stop();
			Storyboard storyboard = new Storyboard();
			//Slowly make it white again.
			storyboard.Children.Add(InitColorAnimation(PSaveBtn, Colors.Lime, Colors.White, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 2), "Foreground.(SolidColorBrush.Color)"));
			storyboard.Begin();
		}
		private async void NSaveBtn_Click(object sender, RoutedEventArgs e)
		{
			Preset something = (Preset)NList.SelectedItem;
			int selectedIndex = NList.SelectedIndex;
			if (something is not null)
			{
				await MainWindow.MainVM.Presets.UpdatePreset(something.Name, true, NPromptsBox.Text, NNotes.Text);
				NSavedTimer.Tick += NSavedTimer_Tick;
				NSavedTimer.Start();    //Wait 1 sec to play animation.
				NList.SelectedIndex = selectedIndex;    //TODO: This stopped working after implementing async.
				NSaveBtn.Foreground = new SolidColorBrush(Colors.Lime); //Set the text green.
			}
		}
		private void NSavedTimer_Tick(object sender, EventArgs e)
		{
			NSavedTimer.Stop();
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(InitColorAnimation(NSaveBtn, Colors.Lime, Colors.White, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 2), "Foreground.(SolidColorBrush.Color)"));
			storyboard.Begin();
		}
		#endregion
		#endregion
		private async void SavePresetWndClosed(object sender, EventArgs e)  //Async yay
		{
			if (((SavePresetWnd)sender).SnapsToDevicePixels)    //Saving?
			{
				Preset preset = new Preset();
				preset.Name = ((Border)((SavePresetWnd)sender).Content).Tag as string;
				if ((bool)((SavePresetWnd)sender).Tag)  //Is prompt Negative?
				{   //Negative prompts saving
					preset.Prompts = NPromptsBox.Text;
					preset.Notes = NNotes.Text;
					await MainWindow.MainVM.Presets.AddNewPreset(preset, MainWindow.MainVM.Presets.NPresets, true);
				}
				else
				{   //Positive prompts saving
					preset.Prompts = PPromptsBox.Text;
					preset.Notes = PNotes.Text;
					await MainWindow.MainVM.Presets.AddNewPreset(preset, MainWindow.MainVM.Presets.PPresets, false);
				}
			}
			PNewBtn.IsEnabled = NNewBtn.IsEnabled = true;
			return;
		}
		private void PListChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				Preset preset = e.AddedItems[0] as Preset;
				PPromptsBox.Text = preset.Prompts;
				PNotes.Text = preset.Notes;
			}
			catch (IndexOutOfRangeException)    //When nothing's selected, the index will be -1
			{ }
		}
		private void NListChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				Preset preset = e.AddedItems[0] as Preset;
				NPromptsBox.Text = preset.Prompts;
				NNotes.Text = preset.Notes;
			}
			catch (IndexOutOfRangeException)
			{ }
		}

		private ColorAnimation InitColorAnimation(UIElement uIElement, Color from, Color to, TimeSpan beginTime, TimeSpan duration, string propertyPath)
		{
			ColorAnimation colorAnimation = new ColorAnimation { From = from, To = to, BeginTime = beginTime, Duration = duration };
			Storyboard.SetTarget(colorAnimation, uIElement);
			Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(propertyPath));
			return colorAnimation;
		}

		private void PList_MouseUp(object sender, MouseButtonEventArgs e)
		{   //Index of 2 returns PList
			//User can now use RMB to cancel selection.
			if (e.ChangedButton == MouseButton.Right)
				((ListBox)((Grid)Content).Children[2]).SelectedIndex = -1;
		}
		private void NList_MouseUp(object sender, MouseButtonEventArgs e)
		{   //Index of 3 returns NList
			if (e.ChangedButton == MouseButton.Right)
				((ListBox)((Grid)Content).Children[3]).SelectedIndex = -1;
		}

		private void UserControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.LeftCtrl)) ;
		}
	}
}

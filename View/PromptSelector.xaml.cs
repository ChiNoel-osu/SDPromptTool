using SDPromptTool.ViewModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static SDPromptTool.ViewModel.PromptSelectorVM;
using static SDPromptTool.ViewModel.SharedPresetVM;

namespace SDPromptTool.View
{
	/// <summary>
	/// PromptSelector.xaml 的交互逻辑
	/// </summary>
	public partial class PromptSelector : UserControl
	{
		List<string> selectedTags = new();
		public PromptSelector()
		{
			DataContext = MainWindow.MainVM;
			InitializeComponent();
		}

		private void SearchBtn_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			((Button)sender).Content = ((Button)sender).IsEnabled ? "Search!" : "Searching..";
		}

		private void Label_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			//Check if tag is selected
			selectedTags.Clear();
			foreach (PromptSelectorVM.CustomListBoxItem item in TagList.SelectedItems)
				selectedTags.Add(item.Tag);
			if (selectedTags.Contains(((Label)sender).Tag.ToString()))
				return;

			int weight = e.Delta > 0 ? sbyte.Parse(((Label)sender).Content.ToString()) + 1 : sbyte.Parse(((Label)sender).Content.ToString()) - 1;
			if (weight > 8)
				weight--;
			else if (weight < -8)
				weight++;
			((Label)sender).Content = weight;
			e.Handled = true;   //Handle the scroll so it won't be passed to the ListBox.
		}

		private void Label_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			//Check if tag is selected
			selectedTags.Clear();
			foreach (PromptSelectorVM.CustomListBoxItem item in TagList.SelectedItems)
				selectedTags.Add(item.Tag);
			if (selectedTags.Contains(((Label)sender).Tag.ToString()))
				return;

			if (e.ChangedButton == System.Windows.Input.MouseButton.Middle)
				((Label)sender).Content = 0;
		}

		#region Button events
		private void PullTagButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			PPromptsBox.AppendText(MainWindow.MainVM.PSVM.PullTag(TagList.SelectedItems));
			TagList.SelectedItem = null;
		}
		private void CopyBtn_Click(object sender, System.Windows.RoutedEventArgs e)
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
		private void ClrBtn_Click(object sender, RoutedEventArgs e)
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
		#region InsertButton
		private void PInsertBtn_Click(object sender, RoutedEventArgs e)
		{
			if (PPresetsCB.SelectedIndex > -1)
			{
				Preset sth = PPresetsCB.SelectedItem as Preset;
				PPromptsBox.AppendText(sth.Prompts);
				PNotes.Text = sth.Notes;
			}
		}
		private void NInsertBtn_Click(object sender, RoutedEventArgs e)
		{
			if (NPresetsCB.SelectedIndex > -1)
			{
				Preset sth = NPresetsCB.SelectedItem as Preset;
				NPromptsBox.AppendText(sth.Prompts);
				NNotes.Text = sth.Notes;
			}
		}
		#endregion

		#endregion

		private void PSaveBtn_Click(object sender, RoutedEventArgs e)
		{
			SavePresetWnd wnd = new SavePresetWnd(false);
			wnd.Closed += SavePresetWndClosed;
			wnd.Show();
			PSaveBtn.IsEnabled = NSaveBtn.IsEnabled = false;
		}
		private void NSaveBtn_Click(object sender, RoutedEventArgs e)
		{
			SavePresetWnd wnd = new SavePresetWnd(true);
			wnd.Closed += SavePresetWndClosed;
			wnd.Show();
			PSaveBtn.IsEnabled = NSaveBtn.IsEnabled = false;
		}
		private void SavePresetWndClosed(object sender, EventArgs e)
		{
			if (((SavePresetWnd)sender).SnapsToDevicePixels)    //Saving?
			{
				Preset preset = new Preset();
				preset.Name = ((Border)((SavePresetWnd)sender).Content).Tag as string;
				if ((bool)((SavePresetWnd)sender).Tag)  //Is prompt Negative?
				{   //Negative prompts saving
					preset.Prompts = NPromptsBox.Text;
					preset.Notes = NNotes.Text;
					MainWindow.MainVM.Presets.AddNewPreset(preset, MainWindow.MainVM.Presets.NPresets, true);
				}
				else
				{   //Positive prompts saving
					preset.Prompts = PPromptsBox.Text;
					preset.Notes = PNotes.Text;
					MainWindow.MainVM.Presets.AddNewPreset(preset, MainWindow.MainVM.Presets.PPresets, false);
				}
			}
			PSaveBtn.IsEnabled = NSaveBtn.IsEnabled = true;
			return;
		}
	}
}
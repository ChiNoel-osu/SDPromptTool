using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace SDPromptTool.ViewModel
{
	public partial class PromptSelectorVM : ObservableObject
	{
		[ObservableProperty]
		bool _IsNotSearching = true;    //Change the button's IsEnabled

		[ObservableProperty]
		string _TextBoxText;

		public class CustomListBoxItem
		{
			public string Tag { get; set; }
			public int Weight { get; set; } = 0;
			public bool ReadOnly { get; set; } = true;
		}

		[ObservableProperty]
		ObservableCollection<CustomListBoxItem> _TagList = new ObservableCollection<CustomListBoxItem>();

		[RelayCommand]
		public async void RefreshTag()
		{
			TagList.Clear();
			IsNotSearching = false;
			HttpClient httpClient = new HttpClient();
			HttpResponseMessage tagResponse;
			try
			{   //Danbooru API
				tagResponse = await httpClient.GetAsync(new Uri(
"https://danbooru.donmai.us/tags.json?search[name_matches]=*" + _TextBoxText + "*&search[order]=name&search[hide_empty]=1&search[post_count]=%3E100&limit=1000"));
			}
			catch (Exception e)
			{
				MessageBox.Show("Use a VPN or try again:\n" + e.Message, "Cannot get list from Danbooru API.");
				TagList.Add(new CustomListBoxItem() { Tag = string.Empty, ReadOnly = false });  //Custom tag.
				IsNotSearching = true;
				return;
			}
			if (tagResponse.StatusCode == System.Net.HttpStatusCode.Forbidden)
			{
				MessageBox.Show("Status 403 Forbidden.\nYou can access the API, but sth is wrong with ur VPN or network and the API started to check ur connection.\nI've yet to find a solution for this....\nRequest Uri: " + tagResponse.RequestMessage.RequestUri.AbsoluteUri, "Connecton error.");
				TagList.Add(new CustomListBoxItem() { Tag = string.Empty, ReadOnly = false });
				IsNotSearching = true;
				return;
			}
			JsonDocument jsonDocument = JsonDocument.Parse(await tagResponse.Content.ReadAsStringAsync());
			JsonElement jsonRoot = jsonDocument.RootElement;
			int arrLength = jsonRoot.GetArrayLength();
			JsonElement.ArrayEnumerator arrEnu = jsonRoot.EnumerateArray();
			for (int i = 0; i < arrLength; i++)
			{
				arrEnu.MoveNext();
				CustomListBoxItem CLBI = new CustomListBoxItem();
				CLBI.Tag = arrEnu.Current.GetProperty("name").GetString();
				TagList.Add(CLBI);
			}
			TagList.Add(new CustomListBoxItem() { Tag = string.Empty, ReadOnly = false });  //Custom tag.
			IsNotSearching = true;
		}

		public string PullTag(IList TagList)
		{
			string prompts = string.Empty;
			foreach (CustomListBoxItem item in TagList)
			{
				if (item.Weight == 0)
					prompts += item.Tag.ToString() + ", ";
				else if (item.Weight > 0)
				{
					string weightedTag = item.Tag.ToString();
					for (sbyte b = 0; b < item.Weight; b++)
						weightedTag = '(' + weightedTag + ')';
					prompts += weightedTag + ", ";
				}
				else if (item.Weight < 0)
				{
					string weightedTag = item.Tag.ToString();
					for (sbyte b = 0; b > item.Weight; b--)
						weightedTag = '[' + weightedTag + ']';
					prompts += weightedTag + ", ";
				}
				else
					MessageBox.Show("WTF Mate");
			}
			return prompts.Replace('_', ' ');
		}

		public PromptSelectorVM()
		{   //Add one custom tag first the window's loaded.
			TagList.Add(new CustomListBoxItem() { Tag = string.Empty, ReadOnly = false });
		}
	}
}

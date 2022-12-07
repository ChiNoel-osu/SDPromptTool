using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using SDPromptTool.Model;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SDPromptTool.ViewModel
{
	public partial class MiscToolsVM : ObservableObject
	{
		readonly string embJsonPath = Directory.GetCurrentDirectory() + "\\UserPresets\\Embeddings.json";

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
		[RelayCommand]
		public void UpdateEmbJson()
		{
			Dictionary<string, string> nameDescPairs = new Dictionary<string, string>();
			foreach (EmbeddingsListModel emb in _PEmbList.Concat(_NEmbList))
				nameDescPairs.Add(emb.EmbName, emb.EmbTag);
			allEmb.Clear();
			foreach (KeyValuePair<string, string> entry in nameDescPairs.ToImmutableSortedDictionary(NSC))
				allEmb.Add(new EmbeddingsListModel { EmbName = entry.Key, EmbTag = entry.Value });
			File.WriteAllText(embJsonPath, JsonConvert.SerializeObject(allEmb, Formatting.Indented));
		}

		[ObservableProperty]
		ObservableCollection<EmbeddingsListModel> _PEmbList = new ObservableCollection<EmbeddingsListModel>();
		[ObservableProperty]
		ObservableCollection<EmbeddingsListModel> _NEmbList = new ObservableCollection<EmbeddingsListModel>();

		List<EmbeddingsListModel> allEmb = new List<EmbeddingsListModel>();
		string _embDir = string.Empty;
		public string EmbDir
		{
			get
			{
				return _embDir;
			}
			set
			{
				_embDir = value;
				Task.Run(() =>
				{
					PEmbList.Clear(); NEmbList.Clear(); allEmb.Clear();
					if (Directory.Exists(_embDir))
					{
						List<string> EmbFiles = Directory.GetFiles(_embDir, "*.pt").ToList();
						if (EmbFiles.Count == 0) return;
						#region Check if json file exists
						if (!File.Exists(embJsonPath))
						{
							File.Create(embJsonPath).Close();
							File.WriteAllText(embJsonPath, JsonConvert.SerializeObject(new List<EmbeddingsListModel>()));
						}
						string embJson = File.ReadAllText(embJsonPath);
						#endregion
						List<EmbeddingsListModel> embFromJson = JsonConvert.DeserializeObject<List<EmbeddingsListModel>>(embJson);
						#region Get current embs from folder
						for (ushort i = 0; i < EmbFiles.Count; i++)
						{
							string embName = EmbFiles[i].Remove(EmbFiles[i].LastIndexOf('.'));
							EmbFiles[i] = embName[(embName.LastIndexOf('\\') + 1)..];
						}
						foreach (string emb in EmbFiles)
							allEmb.Add(new EmbeddingsListModel { EmbName = emb, EmbTag = "<Description>" });
						#endregion
						#region Add(or delete) missing embs to json from folder, create&sort final dictionary.
						foreach (EmbeddingsListModel fromFolder in allEmb)
							if (!embFromJson.Contains(fromFolder, CEC))  //If the actual emb does not exist in json file
								embFromJson.Add(fromFolder);
						//Enumerate from last to first. Making sure index won't go out of bound if sth is deleted.
						for (int jsonEmbIndex = embFromJson.Count - 1; jsonEmbIndex >= 0; jsonEmbIndex--)
							if (!allEmb.Contains(embFromJson[jsonEmbIndex], CEC)) //If the json file has sth that is not there anymore.
								embFromJson.Remove(embFromJson[jsonEmbIndex]);
						Dictionary<string, string> nameDescPairs = new Dictionary<string, string>();
						foreach (EmbeddingsListModel fromJson in embFromJson)
							nameDescPairs.Add(fromJson.EmbName, fromJson.EmbTag);
						ImmutableSortedDictionary<string, string> sortedDict = nameDescPairs.ToImmutableSortedDictionary(NSC);
						#endregion
						allEmb.Clear();
						#region Update(data in view, json file).
						foreach (KeyValuePair<string, string> entry in sortedDict)
						{
							EmbeddingsListModel embToAdd = new EmbeddingsListModel { EmbName = entry.Key, EmbTag = entry.Value };
							if (entry.Key.EndsWith("-neg"))
							{
								NEmbList.Add(embToAdd);
							}
							else
							{
								PEmbList.Add(embToAdd);
							}
							allEmb.Add(embToAdd);
						}
						File.WriteAllText(embJsonPath, JsonConvert.SerializeObject(allEmb, Formatting.Indented));
						#endregion
					}
					else return;
				});
			}
		}
		class NaturalStringComparer : IComparer<string>
		{
			public int Compare(string left, string right)
			{
				int max = new[] { left, right }
					.SelectMany(x => Regex.Matches(x, @"\d+").Cast<Match>().Select(y => (int?)y.Value.Length))
					.Max() ?? 0;
				var leftPadded = Regex.Replace(left, @"\d+", m => m.Value.PadLeft(max, '0'));
				var rightPadded = Regex.Replace(right, @"\d+", m => m.Value.PadLeft(max, '0'));
				return string.Compare(leftPadded, rightPadded);
			}
		}
		class CustomEquComparer : IEqualityComparer<EmbeddingsListModel>
		{
			public bool Equals([AllowNull] EmbeddingsListModel x, [AllowNull] EmbeddingsListModel y)
			{
				if (x.EmbName == y.EmbName) return true;
				else return false;
			}

			public int GetHashCode([DisallowNull] EmbeddingsListModel obj)
			{
				throw new System.NotImplementedException();
			}
		}
		readonly NaturalStringComparer NSC = new NaturalStringComparer();
		readonly CustomEquComparer CEC = new CustomEquComparer();

		public MiscToolsVM()
		{
			BindingOperations.EnableCollectionSynchronization(PEmbList, new object());
			BindingOperations.EnableCollectionSynchronization(NEmbList, new object());
			EmbDir = "<Your stable-diffusion-webui folder path here>\\embeddings";
		}
	}
}

using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SDPromptTool.ViewModel
{
	public partial class SharedPresetVM : ObservableObject
	{
		readonly string PPresetPath = Directory.GetCurrentDirectory() + "\\UserPresets\\Positive.json";
		readonly string NPresetPath = Directory.GetCurrentDirectory() + "\\UserPresets\\Negative.json";
		public class Preset
		{
			public string Name { get; set; }
			public string Prompts { get; set; }
			public string Notes { get; set; }
		}

		[ObservableProperty]
		ObservableCollection<Preset> _PPresets = new ObservableCollection<Preset>();
		[ObservableProperty]
		ObservableCollection<Preset> _NPresets = new ObservableCollection<Preset>();

		private void LoadPresets(bool IsNegative)
		{
			//Read json file and create it if it doesn't exist.
			Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\UserPresets");
			string path, PresetsJson;
			if (IsNegative)
				path = NPresetPath;
			else
				path = PPresetPath;
			try
			{
				PresetsJson = File.ReadAllText(path);
			}
			catch (FileNotFoundException)
			{
				File.Create(path).Close();
				File.WriteAllText(path, JsonConvert.SerializeObject(PPresets));
				PresetsJson = File.ReadAllText(path);
			}
			if (IsNegative)
				NPresets = JsonConvert.DeserializeObject<ObservableCollection<Preset>>(PresetsJson);
			else
				PPresets = JsonConvert.DeserializeObject<ObservableCollection<Preset>>(PresetsJson);
		}

		public Task AddNewPreset(Preset ps, ObservableCollection<Preset> presets, bool IsNegative)
		{
			Task task = new Task(() =>
			{
				//Thread.Sleep(2500);	//Test
				presets.Add(ps);
				string str = JsonConvert.SerializeObject(presets, Formatting.Indented);
				if (IsNegative)
					File.WriteAllText(NPresetPath, str);
				else
					File.WriteAllText(PPresetPath, str);
			});
			task.Start();
			return task;
		}
		/// <summary>
		/// Delete first occurrence of preset that matches the given name.
		/// </summary>
		/// <param name="presetName">The preset name. If duplicates exists, this will delete the first one.</param>
		/// <param name="IsNegative">Weather the preset is negative prompt or not.</param>
		public void DeletePreset(string presetName, bool IsNegative)
		{
			System.Collections.Generic.IEnumerator<Preset> pstEnu;
			if (IsNegative)
				pstEnu = NPresets.GetEnumerator();
			else
				pstEnu = PPresets.GetEnumerator();
			do
				pstEnu.MoveNext();
			while (pstEnu.Current.Name != presetName);
			if (IsNegative)
				NPresets.Remove(pstEnu.Current);
			else
				PPresets.Remove(pstEnu.Current);
			if (IsNegative)
				File.WriteAllText(NPresetPath, JsonConvert.SerializeObject(NPresets, Formatting.Indented));
			else
				File.WriteAllText(PPresetPath, JsonConvert.SerializeObject(PPresets, Formatting.Indented));
		}
		public Task UpdatePreset(string presetName, bool IsNegative, string updatedPrompts, string updatedNotes)
		{
			Task task = new Task(() =>
			{
				//Thread.Sleep(2500);	//Test
				System.Collections.Generic.IEnumerator<Preset> pstEnu;
				if (IsNegative)
					pstEnu = NPresets.GetEnumerator();
				else
					pstEnu = PPresets.GetEnumerator();
				ushort index = 0;
				do
				{
					pstEnu.MoveNext();
					index++;
				}
				while (pstEnu.Current.Name != presetName);
				index--;    //List index starts from 0.
				pstEnu.Current.Prompts = updatedPrompts;
				pstEnu.Current.Notes = updatedNotes;
				if (IsNegative)
				{
					NPresets[index] = pstEnu.Current;
					File.WriteAllText(NPresetPath, JsonConvert.SerializeObject(NPresets, Formatting.Indented));
				}
				else
				{
					PPresets[index] = pstEnu.Current;
					File.WriteAllText(PPresetPath, JsonConvert.SerializeObject(PPresets, Formatting.Indented));
				}
			});
			task.Start();
			return task;
		}

		public SharedPresetVM()
		{
			LoadPresets(false);
			LoadPresets(true);
			//Do this so the ObservableCollection can be shared between threads
			BindingOperations.EnableCollectionSynchronization(PPresets, new object());
			BindingOperations.EnableCollectionSynchronization(NPresets, new object());
		}
	}
}

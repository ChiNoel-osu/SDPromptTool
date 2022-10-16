using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Printing;
using System.Security.AccessControl;

namespace SDPromptTool.ViewModel
{
	public partial class SharedPresetVM : ObservableObject
	{
		private readonly string PPresetPath = Directory.GetCurrentDirectory() + "\\UserPresets\\Positive.json";
		private readonly string NPresetPath = Directory.GetCurrentDirectory() + "\\UserPresets\\Negative.json";
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

		public void AddNewPreset(Preset ps, ObservableCollection<Preset> presets, bool IsNegative)
		{
			presets.Add(ps);
			string str = JsonConvert.SerializeObject(presets, Formatting.Indented);
			if (IsNegative)
				File.WriteAllText(NPresetPath, str);
			else
				File.WriteAllText(PPresetPath, str);
		}

		public SharedPresetVM()
		{
			LoadPresets(false);
			LoadPresets(true);
		}
	}
}

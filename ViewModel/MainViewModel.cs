namespace SDPromptTool.ViewModel
{
	public class MainViewModel
	{
		public PromptSelectorVM PSVM { get; set; }
		public SharedPresetVM Presets { get; set; }
		public MiscToolsVM MiscTools { get; set; }
		public MainViewModel()
		{
			PSVM = new PromptSelectorVM();
			Presets = new SharedPresetVM();
			MiscTools = new MiscToolsVM();
		}
	}
}
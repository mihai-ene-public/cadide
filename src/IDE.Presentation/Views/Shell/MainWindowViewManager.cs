namespace IDE.App.Views.Shell
{
	/// <summary>
	/// This class manages global settings such as a
	/// 
	/// 1> MainMenu control,
	/// </summary>
	public static class MainWindowViewManager
	{
        #region fields

        static MainMenu mainMenu = null;
		static StatusBar mainWindowStatusBar = null;
		
        #endregion fields

		#region constructor
		/// <summary>
		/// Staic class constructor
		/// </summary>
		static MainWindowViewManager()
		{
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get the currently available main menu to be displayed in the main menu.
		/// </summary>
		public static MainMenu MainWindowMenu
		{
			get
			{
				if (mainMenu == null)
					mainMenu = new MainMenu();

				return mainMenu;
			}
		}

		public static StatusBar MainWindowStatusBar
		{
			get
			{
				if (mainWindowStatusBar == null)
					mainWindowStatusBar = new StatusBar();

				return mainWindowStatusBar;
			}
		}
		#endregion properties
	}
}

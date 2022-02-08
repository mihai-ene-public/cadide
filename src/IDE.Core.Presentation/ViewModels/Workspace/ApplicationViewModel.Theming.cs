namespace IDE.Core.ViewModels
{
    using System;
    using IDE.Core.Interfaces;

    public partial class ApplicationViewModel
	{
		/// <summary>
		/// Change WPF theme.
		/// 
		/// This method can be called when the theme is to be reseted by all means
		/// (eg.: when powering application up).
		/// 
		/// !!! Use the CurrentTheme property to change !!!
		/// !!! the theme when App is running           !!!
		/// </summary>
		public void ResetTheme()
		{
            SwitchToSelectedTheme();
		}

		/// <summary>
		/// Attempt to switch to the theme stated in <paramref name="nextThemeToSwitchTo"/>.
		/// The given name must map into the <seealso cref="Themes.ThemesVM.EnTheme"/> enumeration.
		/// </summary>
		private void SwitchToSelectedTheme()
		{
            var resManager = ServiceProvider.Resolve<IResourceLocator>();
            resManager.SwitchToSelectedTheme();

			//try
			//{
			//	// Get WPF Theme definition from Themes Assembly
			//	var theme = themesManager.SelectedTheme;

			//	if (theme != null)
			//	{
			//		Application.Current.Resources.MergedDictionaries.Clear();

			//		foreach (var item in theme.Resources)
			//		{
			//			try
			//			{
			//				var Res = new Uri(item, UriKind.Relative);

			//				var dictionary = Application.LoadComponent(Res) as ResourceDictionary;

			//				if (dictionary != null)
			//					Application.Current.Resources.MergedDictionaries.Add(dictionary);
			//			}
			//			catch (Exception ex)
			//			{
   //                         MessageDialog.Show(ex.Message);
   //                     }
			//		}
			//	}
			//}
			//catch (Exception ex)
			//{
   //             MessageDialog.Show(ex.Message);
			//}
		}
	}
}

namespace IDE.Core.ViewModels
{
    using System;
    using IDE.Core.Interfaces;

    public partial class ApplicationViewModel
	{
		public void ResetTheme()
		{
            SwitchToSelectedTheme();
		}

		private void SwitchToSelectedTheme()
		{
            var resManager = ServiceProvider.Resolve<IResourceLocator>();
            resManager.SwitchToSelectedTheme();
		}
	}
}

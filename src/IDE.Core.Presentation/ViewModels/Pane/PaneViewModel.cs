namespace IDE.Core.ViewModels
{
   
    using System;

    /// <summary>
    /// AvalonDock base class viewm-model to support document pane views
    /// </summary>
    public class PaneViewModel : BaseViewModel
	{
		public PaneViewModel()
		{ }

		#region Title

		private string title = null;
		public virtual string Title
		{
			get { return title; }
			set
			{
				if (title != value)
				{
					title = value;
					OnPropertyChanged("Title");
				}
			}
		}
		#endregion

		//public virtual Uri IconSource
		//{
		//	get;

		//	protected set;
		//}

		#region ContentId

		private string contentId = null;
		public string ContentId
		{
			get { return contentId; }
			set
			{
				if (contentId != value)
				{
					contentId = value;
					OnPropertyChanged("ContentId");
				}
			}
		}
		#endregion

		#region IsSelected

		private bool isSelected = false;
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				if (isSelected != value)
				{
					isSelected = value;
					OnPropertyChanged("IsSelected");
				}
			}
		}

		#endregion

		#region IsActive

		private bool isActive = false;
		public bool IsActive
		{
			get { return isActive; }
			set
			{
				if (isActive != value)
				{
					isActive = value;
					OnPropertyChanged("IsActive");
				}
			}
		}

		#endregion
	}
}

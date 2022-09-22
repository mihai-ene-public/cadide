namespace IDE.Documents.Views
{
    using System;
    using System.Collections.ObjectModel;
    using Core;
    using Core.Units;
    using IDE.Core.Interfaces;

    //   public class TextViewModel : BaseViewModel
    //{
    //	#region fields
    //	private TextDocument mDocument;

    //	private object mLockThis = new object();

    //	private bool mIsReadOnly = false;
    //	private int mLine = 0;
    //	private int mColumn = 0;

    //	private TextBoxController mTxtControl = null;

    //	// These properties are used to save and restore the editor state when CTRL+TABing between documents
    //	private int mTextEditorCaretOffset = 0;
    //	private int mTextEditorSelectionStart = 0;
    //	private int mTextEditorSelectionLength = 0;
    //	private bool mTextEditorIsRectangularSelection = false;
    //	private double mTextEditorScrollOffsetX = 0;
    //	private double mTextEditorScrollOffsetY = 0;

    //	private bool mWordWrap = false;            // Toggle state command
    //	private bool mShowLineNumbers = true;     // Toggle state command

    //	private ICSharpCode.AvalonEdit.TextEditorOptions mTextOptions
    //					= new ICSharpCode.AvalonEdit.TextEditorOptions() { IndentationSize = 2, ConvertTabsToSpaces = true };

    //	private bool mIsDirty = false;
    //	#endregion fields

    //	#region constructor
    //	/// <summary>
    //	/// Constructor
    //	/// </summary>
    //	/// <param name="documentViewModel"></param>
    //	public TextViewModel()
    //	{
    //           dispatcherHelper = ServiceProvider.Resolve<IDispatcherHelper>();

    //		SizeUnitLabel = new UnitViewModel(GenerateScreenUnitList(), 0);

    //		TxtControl = new TextBoxController();

    //		mDocument = new TextDocument();

    //		TextEditorSelectionStart = 0;
    //		TextEditorSelectionLength = 0;

    //		// Set XML Highlighting for XML split view part of the UML document viewer
    //		mHighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(".txt");
    //	}
    //       #endregion constructor

    //       IDispatcherHelper dispatcherHelper;

    //       #region properties
    //       #region AvalonEdit properties
    //       #region IsReadOnly
    //       public bool IsReadOnly
    //	{
    //		get
    //		{
    //			return mIsReadOnly;
    //		}

    //		protected set
    //		{
    //			if (mIsReadOnly != value)
    //			{
    //				mIsReadOnly = value;
    //				OnPropertyChanged(nameof(IsReadOnly));
    //			}
    //		}
    //	}
    //	#endregion IsReadOnly

    //	#region TextContent
    //	/// <summary>
    //	/// This property wraps the document class provided by AvalonEdit. The actual text is inside
    //	/// the document and can be accessed at save, load or other processing times.
    //	/// 
    //	/// The Text property itself cannot be bound in AvalonEdit since binding this would mResult
    //	/// in updating the text (via binding) each time a user enters a key on the keyboard
    //	/// (which would be a design error resulting in huge performance problems)
    //	/// </summary>
    //	public TextDocument Document
    //	{
    //		get
    //		{
    //			return mDocument;
    //		}

    //		set
    //		{
    //			if (mDocument != value)
    //			{
    //				mDocument = value;
    //				OnPropertyChanged(nameof(Document));
    //			}
    //		}
    //	}
    //	#endregion

    //	#region ScaleView
    //	/// <summary>
    //	/// Scale view of text in percentage of font size
    //	/// </summary>
    //	public UnitViewModel SizeUnitLabel { get; set; }
    //	#endregion ScaleView

    //	#region CaretPosition
    //	// These properties are used to display the current column/line
    //	// of the cursor in the user interface
    //	public int Line
    //	{
    //		get
    //		{
    //			return mLine;
    //		}

    //		set
    //		{
    //			if (mLine != value)
    //			{
    //				mLine = value;
    //				OnPropertyChanged(nameof(Line));
    //			}
    //		}
    //	}

    //	public int Column
    //	{
    //		get
    //		{
    //			return mColumn;
    //		}

    //		set
    //		{
    //			if (mColumn != value)
    //			{
    //				mColumn = value;
    //				OnPropertyChanged(nameof(Column));
    //			}
    //		}
    //	}
    //	#endregion CaretPosition

    //	#region TxtControl
    //	public TextBoxController TxtControl
    //	{
    //		get
    //		{
    //			return mTxtControl;
    //		}

    //		private set
    //		{
    //			if (mTxtControl != value)
    //			{
    //				mTxtControl = value;
    //				OnPropertyChanged(nameof(TxtControl));
    //			}
    //		}
    //	}
    //	#endregion TxtControl

    //	#region EditorStateProperties
    //	/// <summary>
    //	/// Get/set editor carret position
    //	/// for CTRL-TAB Support 
    //	/// </summary>
    //	public int TextEditorCaretOffset
    //	{
    //		get
    //		{
    //			return mTextEditorCaretOffset;
    //		}

    //		set
    //		{
    //			if (mTextEditorCaretOffset != value)
    //			{
    //				mTextEditorCaretOffset = value;
    //				OnPropertyChanged(nameof(TextEditorCaretOffset));
    //			}
    //		}
    //	}

    //	/// <summary>
    //	/// Get/set editor start of selection
    //	/// for CTRL-TAB Support 
    //	/// </summary>
    //	public int TextEditorSelectionStart
    //	{
    //		get
    //		{
    //			return mTextEditorSelectionStart;
    //		}

    //		set
    //		{
    //			if (mTextEditorSelectionStart != value)
    //			{
    //				mTextEditorSelectionStart = value;
    //				OnPropertyChanged(nameof(TextEditorSelectionStart));
    //			}
    //		}
    //	}

    //	/// <summary>
    //	/// Get/set editor length of selection
    //	/// for CTRL-TAB Support
    //	/// </summary>
    //	public int TextEditorSelectionLength
    //	{
    //		get
    //		{
    //			return mTextEditorSelectionLength;
    //		}

    //		set
    //		{
    //			if (mTextEditorSelectionLength != value)
    //			{
    //				mTextEditorSelectionLength = value;
    //				OnPropertyChanged(nameof(TextEditorSelectionLength));
    //			}
    //		}
    //	}

    //	public bool TextEditorIsRectangularSelection
    //	{
    //		get
    //		{
    //			return mTextEditorIsRectangularSelection;
    //		}

    //		set
    //		{
    //			if (mTextEditorIsRectangularSelection != value)
    //			{
    //				mTextEditorIsRectangularSelection = value;
    //				OnPropertyChanged(nameof(TextEditorIsRectangularSelection));
    //			}
    //		}
    //	}

    //	#region EditorScrollOffsetXY
    //	/// <summary>
    //	/// Current editor view scroll X position
    //	/// </summary>
    //	public double TextEditorScrollOffsetX
    //	{
    //		get
    //		{
    //			return mTextEditorScrollOffsetX;
    //		}

    //		set
    //		{
    //			if (mTextEditorScrollOffsetX != value)
    //			{
    //				mTextEditorScrollOffsetX = value;
    //				OnPropertyChanged(nameof(TextEditorScrollOffsetX));
    //			}
    //		}
    //	}

    //	/// <summary>
    //	/// Current editor view scroll Y position
    //	/// </summary>
    //	public double TextEditorScrollOffsetY
    //	{
    //		get
    //		{
    //			return mTextEditorScrollOffsetY;
    //		}

    //		set
    //		{
    //			if (mTextEditorScrollOffsetY != value)
    //			{
    //				mTextEditorScrollOffsetY = value;
    //				OnPropertyChanged(nameof(TextEditorScrollOffsetY));
    //			}
    //		}
    //	}
    //	#endregion EditorScrollOffsetXY
    //	#endregion EditorStateProperties

    //	/// <summary>
    //	/// AvalonEdit exposes a Highlighting property that controls whether keywords,
    //	/// comments and other interesting text parts are colored or highlighted in any
    //	/// other visual way. This property exposes the highlighting information for the
    //	/// text file managed in this viewmodel class.
    //	/// </summary>
    //	public IHighlightingDefinition HighlightingDefinition
    //	{
    //		get
    //		{
    //			lock (mLockThis)
    //			{
    //				return mHighlightingDefinition;
    //			}
    //		}

    //		set
    //		{
    //			lock (mLockThis)
    //			{
    //				if (mHighlightingDefinition != value)
    //				{
    //					mHighlightingDefinition = value;

    //					OnPropertyChanged(nameof(HighlightingDefinition));
    //				}
    //			}
    //		}
    //	}

    //	public bool WordWrap
    //	{
    //		get
    //		{
    //			return mWordWrap;
    //		}

    //		set
    //		{
    //			if (mWordWrap != value)
    //			{
    //				mWordWrap = value;
    //				OnPropertyChanged(nameof(WordWrap));
    //			}
    //		}
    //	}

    //	public bool ShowLineNumbers
    //	{
    //		get
    //		{
    //			return mShowLineNumbers;
    //		}

    //		set
    //		{
    //			if (mShowLineNumbers != value)
    //			{
    //				mShowLineNumbers = value;
    //				OnPropertyChanged(nameof(ShowLineNumbers));
    //			}
    //		}
    //	}

    //	public bool ShowEndOfLine               // Toggle state command
    //	{
    //		get
    //		{
    //			return TextOptions.ShowEndOfLine;
    //		}

    //		set
    //		{
    //			if (TextOptions.ShowEndOfLine != value)
    //			{
    //				TextOptions.ShowEndOfLine = value;
    //				OnPropertyChanged(nameof(ShowEndOfLine));
    //			}
    //		}
    //	}

    //	public bool ShowSpaces               // Toggle state command
    //	{
    //		get
    //		{
    //			return TextOptions.ShowSpaces;
    //		}

    //		set
    //		{
    //			if (TextOptions.ShowSpaces != value)
    //			{
    //				TextOptions.ShowSpaces = value;
    //				OnPropertyChanged(nameof(ShowSpaces));
    //			}
    //		}
    //	}

    //	public bool ShowTabs               // Toggle state command
    //	{
    //		get
    //		{
    //			return TextOptions.ShowTabs;
    //		}

    //		set
    //		{
    //			if (TextOptions.ShowTabs != value)
    //			{
    //				TextOptions.ShowTabs = value;
    //				OnPropertyChanged(nameof(ShowTabs));
    //			}
    //		}
    //	}

    //	public ICSharpCode.AvalonEdit.TextEditorOptions TextOptions
    //	{
    //		get
    //		{
    //			return mTextOptions;
    //		}

    //		set
    //		{
    //			if (mTextOptions != value)
    //			{
    //				mTextOptions = value;
    //				OnPropertyChanged(nameof(TextOptions));
    //			}
    //		}
    //	}
    //	#endregion AvalonEdit properties

    //	#region IsDirty
    //	/// <summary>
    //	/// Get whether the current information was edit and needs to be saved or not.
    //	/// </summary>
    //	public bool IsDirty
    //	{
    //		get
    //		{
    //			return mIsDirty;
    //		}

    //		set
    //		{
    //			if (mIsDirty != value)
    //			{
    //				mIsDirty = value;
    //				OnPropertyChanged(nameof(IsDirty));
    //			}
    //		}
    //	}
    //	#endregion IsDirty
    //	#endregion properties

    //	#region methods
    //	/// <summary>
    //	/// Method removes all text from the current viewmodel.
    //	/// </summary>
    //	public void Clear()
    //	{
    //           //Application.Current.Dispatcher.BeginInvoke(
    //           //new Action(
    //           //delegate
    //           //{
    //           //	mDocument.Text = string.Empty;
    //           //}
    //           //));

    //           dispatcherHelper.RunOnDispatcher(() => mDocument.Text = string.Empty);
    //	}

    //	/// <summary>
    //	/// Method appends a new line of text into the current output.
    //	/// </summary>
    //	public void AppendLine(string text)
    //	{
    //		Append(text + Environment.NewLine);
    //	}

    //	/// <summary>
    //	/// Method appends text into the current output.
    //	/// </summary>
    //	public void Append(string text)
    //	{
    //           //Application.Current.Dispatcher.BeginInvoke(
    //           //new Action(
    //           //		delegate
    //           //		{
    //           //			if (text != null)
    //           //				mDocument.Insert(mDocument.TextLength, text);
    //           //		}
    //           //));

    //           dispatcherHelper.RunOnDispatcher(() =>
    //           {
    //               if (text != null)
    //                   mDocument.Insert(mDocument.TextLength, text);
    //           });

    //       }

    //       /// <summary>
    //       /// Initialize scale view of content to indicated value and unit.
    //       /// </summary>
    //       /// <param name="unit"></param>
    //       /// <param name="defaultValue"></param>
    //       public void InitScaleView(int unit, double defaultValue)
    //	{
    //		SizeUnitLabel = new UnitViewModel(GenerateScreenUnitList(), defaultValue);
    //	}

    //       ObservableCollection<AbstractUnit> GenerateScreenUnitList()
    //       {
    //           var unitList = new ObservableCollection<AbstractUnit>();

    //           var percentDefaults = new ObservableCollection<double>() { 25, 50, 75, 100, 125, 150, 175, 200, 300, 400, 500 };
    //           var pointsDefaults = new ObservableCollection<double>() { 3, 6, 8, 9, 10, 12, 14, 16, 18, 20, 24, 26, 32, 48, 60 };

    //           unitList.Add(new ScreenPercentUnit(0)
    //           {
    //               //DefaultValues = percentDefaults,
    //               DisplayNameLong = "percent",
    //               DisplayNameShort = "%",
    //               MinValue = 0,
    //               MaxValue = 400

    //           });
    //           unitList.Add(new ScreenFontPointsUnit(0)
    //           {
    //              // DefaultValues = pointsDefaults,
    //               DisplayNameLong = "point",
    //               DisplayNameShort = "pt",
    //               MinValue = 2,
    //               MaxValue = 399
    //           });

    //           return unitList;
    //       }



    //	#endregion methods
    //}

   
}

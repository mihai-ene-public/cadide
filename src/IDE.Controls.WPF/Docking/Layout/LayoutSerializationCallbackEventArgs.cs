using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Controls.WPF.Docking.Layout;
public class LayoutSerializationCallbackEventArgs : CancelEventArgs
{
    #region constructor

    public LayoutSerializationCallbackEventArgs(LayoutContent model, object previousContent)
    {
        Cancel = false;
        Model = model;
        Content = previousContent;
    }

    #endregion

    #region Properties

    public LayoutContent Model
    {
        get; private set;
    }

    public object Content
    {
        get; set;
    }

    #endregion
}

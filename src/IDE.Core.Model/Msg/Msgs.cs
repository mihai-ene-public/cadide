namespace IDE.Core.Utilities
{
    using System.Collections.Generic;

    ///// <summary>
    ///// This class hosts a collection of (error) message objects
    ///// to enable summary reports.
    ///// </summary>
    //public class Msgs
    //{
    //    #region Fields
    //    private readonly object syncRoot = new object();

    //    private List<Msg> msgs = new List<Msg>();
    //    #endregion Fields

    //    #region Constructors
    //    /// <summary>
    //    /// constructor
    //    /// </summary>
    //    public Msgs()
    //    {
    //        msgs = new List<Msg>();
    //    }

    //    /// <summary>
    //    /// Copy constructor of the <seealso cref="ToolMsgs"/> class
    //    /// </summary>
    //    /// <param name="inTMs"></param>
    //    public Msgs(Msgs inTMs)
    //    {
    //        msgs = new List<Msg>(inTMs.msgs);
    //    }

      
    //    #endregion Constructors

    //    #region properties
    //    /// <summary>
    //    /// Count number of messages retrieved
    //    /// </summary>
    //    public int Count
    //    {
    //        get
    //        {
    //            lock (syncRoot)
    //            {
    //                if (msgs == null)
    //                    return 0;

    //                return msgs.Count;
    //            }
    //        }
    //    }

    //    public List<Msg> Messages
    //    {
    //        get
    //        {
    //            lock (syncRoot)
    //            {
    //                if (msgs == null)
    //                    return new List<Msg>();

    //                return new List<Msg>(this.msgs);
    //            }
    //        }
    //    }
    //    #endregion properties

    //    #region Methods
    //    public void Add(Msg tm)
    //    {
    //        lock (syncRoot)
    //        {
    //            if (tm == null)
    //                return;

    //            if (msgs == null)
    //                msgs = new List<Msg>();

    //            msgs.Add(tm);
    //        }
    //    }

    //    public void Add(string Msg, Msg.MsgCategory cat)
    //    {
    //        lock (syncRoot)
    //        {
    //            if (msgs == null)
    //                msgs = new List<Msg>();

    //            msgs.Add(new Msg(Msg, cat));
    //        }
    //    }

    //    public void Add(Msgs msgColl)
    //    {
    //        lock (syncRoot)
    //        {
    //            if (msgColl == null)
    //                return;

    //            if (msgs == null)
    //                msgs = new List<Msg>();

    //            foreach (var item in msgColl.msgs)
    //            {
    //                msgs.Add(new Msg(item));
    //            }
    //        }
    //    }
    //    #endregion Methods
    //}
}

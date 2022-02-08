using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.HashCodeTree
{

    //b3
    //  b2
    //    b1
    //        data
    public class HashCodeTree<TKey, TData>
    {

        FolderNode[] b3Level = new FolderNode[256];

        public void Add(TKey key, TData data)
        {
            var hashCode = key.GetHashCode();

            var ib3 = GetLevelByte(hashCode, FolderLevel.B3);
            var ib2 = GetLevelByte(hashCode, FolderLevel.B2);
            var ib1 = GetLevelByte(hashCode, FolderLevel.B1);

            if (b3Level[ib3] == null)
            {
                b3Level[ib3] = new FolderNode();
            }

            var b3Folder = b3Level[ib3];
            var b2Folder = b3Folder.CreateFolderAt(ib2);
            var b1Folder = b2Folder.CreateDataNodeAt(ib1);

            b1Folder.Data.Add(data);
        }

        public IEnumerable<TData> Find(TKey key)
        {
            var hashCode = key.GetHashCode();

            var ib3 = GetLevelByte(hashCode, FolderLevel.B3);
            var ib2 = GetLevelByte(hashCode, FolderLevel.B2);
            var ib1 = GetLevelByte(hashCode, FolderLevel.B1);

            var b3Folder = b3Level[ib3];
            if (b3Folder != null)
            {
                var b2Folder = b3Folder.GetNodeAt(ib2);
                if (b2Folder != null)
                {
                    var b1Folder = (DataNode)b2Folder.GetNodeAt(ib1);
                    if (b1Folder != null)
                    {
                        return b1Folder.Data;
                    }
                }
            }

            return System.Linq.Enumerable.Empty<TData>();
        }

        int GetLevelByte(int hashCode, FolderLevel folderLevel)
        {
            //b3: (hashCode>>(8*3)) & 0xff
            //b2: (hashCode>>(8*2)) & 0xff
            //b1: (hashCode>>(8*1)) & 0xff

            var fl = (int)folderLevel;

            return (hashCode >> (8 * fl)) & 0xff;
        }

        #region Nodes

        enum FolderLevel
        {
            B1 = 1,
            B2,
            B3
        }

        class FolderNode
        {
            public FolderNode[] Children { get; } = new FolderNode[256];

            public FolderNode GetNodeAt(int index)
            {
                return Children[index];
            }

            public FolderNode CreateFolderAt(int index)
            {
                if (Children[index] == null)
                {
                    Children[index] = new FolderNode();
                }

                return Children[index];
            }

            public DataNode CreateDataNodeAt(int index)
            {
                if (Children[index] == null)
                {
                    Children[index] = new DataNode();
                }

                return (DataNode)Children[index];
            }
        }

        class DataNode : FolderNode
        {
            public List<TData> Data { get; set; } = new List<TData>();
        }

        #endregion
    }
}

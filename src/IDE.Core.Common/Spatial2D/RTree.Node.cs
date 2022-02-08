using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Spatial2D
{
	public partial class RTree<T>
	{
		public class Node : ISpatialData
		{
			public Node(List<ISpatialData> items, int height)
			{
				this.Height = height;
				this.Children = items;
				ResetEnvelope();
			}

			public void Add(ISpatialData node)
			{
				Children.Add(node);
				Envelope.Extend(node.Envelope);
			}

			internal void Remove(ISpatialData node)
			{
				Children.Remove(node);
				ResetEnvelope();
			}

			public void ResetEnvelope()
			{
				Envelope = GetEnclosingEnvelope(Children);
			}

			public List<ISpatialData> Children { get; set; }
			
			public int Height { get; set; }

			public bool IsLeaf => Height == 1;
			
			public Envelope Envelope { get; set; }
		}
	}
}

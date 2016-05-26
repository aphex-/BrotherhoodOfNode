using System;
using UnityEngine;

namespace Assets.Code.Bon.Graph.Custom
{
	public class SamplerNode : Node
	{

		public SamplerNode(int id) : base(id)
		{
			Sockets.Add(new Socket(this, Color.white, true));
			Sockets.Add(new Socket(this, Color.red, true));
			Sockets.Add(new Socket(this, Color.cyan, false));
			Sockets.Add(new Socket(this, Color.red, false));
		}

		public override void OnGUI()
		{

		}
	}
}



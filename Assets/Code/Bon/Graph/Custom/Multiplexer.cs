using System;
using UnityEngine;

namespace Assets.Code.Bon.Graph.Custom
{
	[Serializable]
	[GraphContextMenuItem("Standard")]
	public class Multiplexer : Node {


		public Multiplexer(int id) : base(id)
		{
			Sockets.Add(new Socket(this, Color.red, true));
			Sockets.Add(new Socket(this, Color.red, false));
			Sockets.Add(new Socket(this, Color.red, false));
			Height = 60;
		}

		public override void OnGUI()
		{
		}

		public override void OnSerialization(SerializableNode sNode)
		{

		}

		public override void OnDeserialization(SerializableNode sNode)
		{

		}
	}
}

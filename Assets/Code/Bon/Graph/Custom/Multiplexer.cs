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
			Height = 65;
		}

		public override void OnGUI()
		{
		}

		public override void ApplySerializationData(SerializableNode sNode)
		{
			sNode.data = JsonUtility.ToJson(this);
		}

	}
}

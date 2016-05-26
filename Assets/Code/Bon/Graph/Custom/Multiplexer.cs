using System;
using UnityEngine;

namespace Assets.Code.Bon.Graph.Custom
{
	[Serializable]
	public class Multiplexer : Node {


		public Multiplexer(int id) : base(id)
		{
			Sockets.Add(new Socket(this, Color.white, true));
			Sockets.Add(new Socket(this, Color.white, false));
			Height = 200;
		}

		public override void OnGUI()
		{

		}

		public override void ApplySerializationData(SerializableNode sNode)
		{
			sNode.data = JsonUtility.ToJson(this);
			sNode.type = this.GetType();
		}

	}
}

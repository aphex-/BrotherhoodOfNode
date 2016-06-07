using System;
using UnityEngine;

namespace Assets.Code.Bon.Graph.Custom
{
	[Serializable]
	[GraphContextMenuItem("Standard")]
	public class SamplerNode : Node
	{

		[SerializeField]
		public string foo = "bar";

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

		public override void ApplySerializationData(SerializableNode sNode)
		{
			sNode.data = JsonUtility.ToJson(this);
		}
	}
}



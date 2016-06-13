using System;
using UnityEngine;

namespace Assets.Code.Bon.Graph.Custom
{
	[Serializable]
	[GraphContextMenuItem("Standard", "CoolNamedNode")]
	public class RidiculouslyNamedNode : Node
	{
		public RidiculouslyNamedNode(int id) : base(id)
		{
			Sockets.Add(new Socket(this, Color.red, true));
			Height = 65;
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
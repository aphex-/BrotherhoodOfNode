using System;
using Assets.Code.Bon.Nodes.Math;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Map
{
	[Serializable]
	[GraphContextMenuItem("Map", "Display")]
	public class MapDisplayNode : Node {


		public MapDisplayNode(int id) : base(id)
		{
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Input));
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
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

		public override object GetResultOf(Socket outSocket)
		{
			return null;
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return false;
		}
	}
}

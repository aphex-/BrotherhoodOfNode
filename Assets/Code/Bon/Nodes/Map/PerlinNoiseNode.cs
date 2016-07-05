using System;
using Assets.Code.Bon.Nodes.Math;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Map
{
	[Serializable]
	[GraphContextMenuItem("Map", "PerlinNoise")]
	public class PerlinNoiseNode : Node, ISampler2D
	{

		private float _samplingX = 0;
		private float _samplingY = 0;

		public PerlinNoiseNode(int id) : base(id)
		{
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
			return Mathf.PerlinNoise(_samplingX, _samplingY);
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return true;
		}

		public void SampleFrom(float x, float y)
		{
			_samplingX = x;
			_samplingY = y;
		}
	}
}

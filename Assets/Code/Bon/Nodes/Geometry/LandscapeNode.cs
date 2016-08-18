using System;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Geometry
{
	[Serializable]
	[GraphContextMenuItem("Geometry", "Lanscape")]
	public class LandscapeNode : AbstractNumberNode, IColorSampler, IStringSampler {

		[NonSerialized] private Rect _heightValueLabel;
		[NonSerialized] private Rect _colorRangeLabel;
		[NonSerialized] private Rect _materialLabel;

		[NonSerialized] private InputSocket _heightValueSocket;
		[NonSerialized] private InputSocket _gradientSocket;
		[NonSerialized] private InputSocket _materialSocket;

		public LandscapeNode(int id, Graph parent) : base(id, parent)
		{
			_heightValueLabel = new Rect(8, 0, 75, 20);
			_colorRangeLabel = new Rect(8, 20, 75, 20);
			_materialLabel = new Rect(8, 40, 75, 20);

			_heightValueSocket = new InputSocket(this, typeof(AbstractNumberNode));
			Sockets.Add(_heightValueSocket);
			_gradientSocket = new InputSocket(this, typeof(AbstractColorNode));
			Sockets.Add(_gradientSocket);
			_materialSocket = new InputSocket(this, typeof(AbstractStringNode));
			Sockets.Add(_materialSocket);
		}

		public override void OnGUI()
		{
			GUI.Label(_heightValueLabel, "height map");
			GUI.Label(_colorRangeLabel, "vertex color");
			GUI.Label(_materialLabel, "material");
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket socket, float x, float y, float z, float seed)
		{
			return GetInputNumber(_heightValueSocket, x, y, z, seed);
		}

		public UnityEngine.Color GetColorFrom(float i)
		{
			if (!_gradientSocket.IsConnected()) return UnityEngine.Color.black;
			AbstractColorNode node = (AbstractColorNode) _gradientSocket.GetConnectedSocket().Parent;
			return node.GetColor(_gradientSocket.GetConnectedSocket(), i);
		}

		public string GetString()
		{
			if (!_materialSocket.IsConnected()) return "";
			AbstractStringNode node = (AbstractStringNode) _materialSocket.GetConnectedSocket().Parent;
			return node.GetString(_materialSocket.GetConnectedSocket());
		}

		public UnityEngine.Color GetColor(OutputSocket outSocket, float i)
		{
			return UnityEngine.Color.black;
		}

		public string GetString(OutputSocket outSocket)
		{
			return null;
		}
	}
}

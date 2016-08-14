using System;
using Assets.Code.Bon.Interface;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Geometry
{
	[Serializable]
	[GraphContextMenuItem("Geometry", "Mesh")]
	public class MeshNode : AbstractNumberNode, IUpdateable, IColorSampler1D, IStringSampler {

		[NonSerialized] private Rect _heightValueLabel;
		[NonSerialized] private Rect _colorRangeLabel;
		[NonSerialized] private Rect _materialLabel;

		[NonSerialized] private Socket _heightValueSocket;
		[NonSerialized] private Socket _gradientSocket;
		[NonSerialized] private Socket _materialSocket;

		public MeshNode(int id, Graph parent) : base(id, parent)
		{
			_heightValueLabel = new Rect(8, 0, 75, 20);
			_colorRangeLabel = new Rect(8, 20, 75, 20);
			_materialLabel = new Rect(8, 40, 75, 20);

			_heightValueSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_heightValueSocket);
			_gradientSocket = new Socket(this, typeof(AbstractColorNode), SocketDirection.Input);
			Sockets.Add(_gradientSocket);
			_materialSocket = new Socket(this, typeof(AbstractStringNode), SocketDirection.Input);
			Sockets.Add(_materialSocket);
		}

		public override void OnGUI()
		{
			GUI.Label(_heightValueLabel, "height map");
			GUI.Label(_colorRangeLabel, "vertex color");
			GUI.Label(_materialLabel, "material");
		}

		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed);
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return _heightValueSocket.GetConnectedSocket() != null;
		}


		public void Update()
		{

		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			return GetInputNumber(_heightValueSocket, x, y, seed);
		}

		public UnityEngine.Color GetColorFrom(float i)
		{
			if (!_gradientSocket.IsConnected()) return UnityEngine.Color.black;
			AbstractColorNode node = (AbstractColorNode) _gradientSocket.GetConnectedSocket().Parent;
			node.SetPosition(i);
			return (UnityEngine.Color) node.GetResultOf(_gradientSocket.GetConnectedSocket());
		}

		public string GetString()
		{
			if (!_materialSocket.IsConnected()) return "";
			AbstractStringNode node = (AbstractStringNode) _materialSocket.GetConnectedSocket().Parent;
			return node.GetString();
		}
	}
}

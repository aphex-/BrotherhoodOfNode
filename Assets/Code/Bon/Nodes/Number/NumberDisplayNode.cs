using System;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Number
{

	[Serializable]
	[GraphContextMenuItem("Number", "Display")]
	public class NumberDisplayNode : AbstractNumberNode
	{
		[NonSerialized] public float Value;
		[NonSerialized] private Rect _textFieldArea;
		[NonSerialized] private InputSocket _inSocket;

		public NumberDisplayNode(int id, Graph parent) : base(id, parent)
		{
			_textFieldArea = new Rect(10, 0, 80, BonConfig.SocketSize);
			_inSocket = new InputSocket(this, typeof(AbstractNumberNode));
			Sockets.Add(_inSocket);
			Height = 20 + BonConfig.SocketOffsetTop;
		}

		public override void OnGUI()
		{
			GUI.Label(_textFieldArea, Value + "");
		}

		public override void Update()
		{
			Value = GetInputNumber(_inSocket, 0, 0, 0, 0);
		}

		public override float GetNumber(OutputSocket outSocket, float x, float y, float z, float seed)
		{
			return GetInputNumber(_inSocket, x, y, z, seed);
		}
	}
}

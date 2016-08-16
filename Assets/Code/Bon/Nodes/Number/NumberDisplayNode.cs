using System;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Number
{

	[Serializable]
	[GraphContextMenuItem("Number", "Display")]
	public class NumberDisplayNode : AbstractNumberNode
	{
		[NonSerialized] public float Value;
		[NonSerialized] private Rect _textFieldArea;
		[NonSerialized] private Socket _inSocket;

		public NumberDisplayNode(int id, Graph parent) : base(id, parent)
		{
			_textFieldArea = new Rect(10, 0, 80, BonConfig.SocketSize);
			_inSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_inSocket);
			Height = 20 + BonConfig.SocketOffsetTop;
		}

		public override void OnGUI()
		{
			GUI.Label(_textFieldArea, Value + "");
		}


		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed);
		}

		public override void Update()
		{
			Value = GetInputNumber(_inSocket, _x, _y, _seed);
		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			return GetInputNumber(_inSocket, x, y, seed);
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Assets.Code.Bon.Nodes.Math
{
	[Serializable]
	[GraphContextMenuItem("Math", "Condition")]
	public class ConditionNode : Node
	{

		private Rect labelInput01 = new Rect(3, 0, 100, 20);
		private Rect labelInput02 = new Rect(3, 20, 100, 20);
		private Rect labelCondition = new Rect(3, 40, 100, 20);

		private Socket _inputSocket01;
		private Socket _inputSocket02;
		private Socket _conditionSocket;

		public ConditionNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket01 = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			_inputSocket02 = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			_conditionSocket = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			Sockets.Add(_inputSocket01);
			Sockets.Add(_inputSocket02);
			Sockets.Add(_conditionSocket);
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
			Height = 80;
			Width = 80;
		}

		public override void OnGUI()
		{
			GUI.Label(labelInput01, "if (x < 1)");
			GUI.Label(labelInput02, "if (x >= 1)");
			GUI.Label(labelCondition, "x");
		}

		public override object GetResultOf(Socket outSocket)
		{
			if (!AllInputSocketsConnected()) return float.NaN;
			float inputValue01 = (float) _inputSocket01.GetConnectedSocket().Parent.GetResultOf(_inputSocket01.GetConnectedSocket());
			float inputValue02 = (float) _inputSocket02.GetConnectedSocket().Parent.GetResultOf(_inputSocket02.GetConnectedSocket());
			float conditionValue02 = (float) _conditionSocket.GetConnectedSocket().Parent.GetResultOf(_conditionSocket.GetConnectedSocket());
			if (float.IsNaN(conditionValue02) || float.IsNaN(inputValue01) || float.IsNaN(inputValue02)) return float.NaN;

			if (conditionValue02 < 1f) return inputValue01;
			return inputValue02;
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return AllInputSocketsConnected();
		}
	}
}

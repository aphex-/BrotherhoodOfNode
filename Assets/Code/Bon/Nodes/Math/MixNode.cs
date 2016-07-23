using System;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Math
{
	[Serializable]
	[GraphContextMenuItem("Math", "Mix")]
	public class MixNode : Node
	{
		private Rect labelInput01 = new Rect(3, 0, 100, 20);
		private Rect labelInput02 = new Rect(3, 20, 100, 20);
		private Rect labelFactor = new Rect(3, 40, 100, 20);

		private Socket _inputSocket01;
		private Socket _inputSocket02;
		private Socket _factorSocket;

		public MixNode(int id) : base(id)
		{
			_inputSocket01 = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			_inputSocket02 = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			_factorSocket = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			Sockets.Add(_inputSocket01);
			Sockets.Add(_inputSocket02);
			Sockets.Add(_factorSocket);
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
			Height = 80;
			Width = 80;
		}

		public override void OnGUI()
		{
			GUI.Label(labelInput01, "in 1");
			GUI.Label(labelInput02, "in 2");
			GUI.Label(labelFactor, "factor (0 - 1)");
		}

		public override object GetResultOf(Socket outSocket)
		{
			if (!AllInputSocketsConnected()) return float.NaN;
			float factorValue = (float) _factorSocket.GetConnectedSocket().Parent.GetResultOf(_factorSocket.GetConnectedSocket());
			if (float.IsNaN(factorValue)) return float.NaN;

			// avoid calc of obsolete values here..
			if (factorValue <= 0)
			{
				return (float) _inputSocket01.GetConnectedSocket().Parent.GetResultOf(_inputSocket01.GetConnectedSocket());
			}
			if (factorValue >= 1)
			{
				return (float) _inputSocket02.GetConnectedSocket().Parent.GetResultOf(_inputSocket02.GetConnectedSocket());
			}

			float v1 = (float) _inputSocket01.GetConnectedSocket().Parent.GetResultOf(_inputSocket01.GetConnectedSocket());
			float v2 = (float) _inputSocket02.GetConnectedSocket().Parent.GetResultOf(_inputSocket02.GetConnectedSocket());

			v1 = Clamp(v1, 0, 1);
			v2 = Clamp(v2, 0, 1);

			if (float.IsNaN(v1) || float.IsNaN(v2)) return float.NaN;

			return (v1 * (1 - factorValue)) + (v2 * (factorValue));
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return AllInputSocketsConnected();
		}

		public static float Clamp(float value, float min, float max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
	}
}

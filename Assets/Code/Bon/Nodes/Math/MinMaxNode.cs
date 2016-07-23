using System;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Math
{
	[Serializable]
	[GraphContextMenuItem("Math", "MinMax")]
	public class MinMaxNode : Node {

		[SerializeField]
		private int _selectedMode = 0;

		[System.NonSerialized]
		private readonly Socket _inputSocket01;

		[System.NonSerialized]
		private readonly Socket _inputSocket02;

		public static readonly string[] Options = { "min", "max"};

		public MinMaxNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket01 = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			Sockets.Add(_inputSocket01);
			_inputSocket02 = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			Sockets.Add(_inputSocket02);
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
			Height = 60;
			Width = 80;
		}


		public override void OnGUI()
		{

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			int newMode = GUILayout.SelectionGrid(_selectedMode,Options,1,"toggle");
			if (newMode != _selectedMode)
			{
				_selectedMode = newMode;
				TriggerChangeEvent();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}



		public override object GetResultOf(Socket outSocket)
		{
			if (!CanGetResultOf(outSocket)) return float.NaN;

			Socket connectedSocket01 = _inputSocket01.GetConnectedSocket();
			Socket connectedSocket02 = _inputSocket02.GetConnectedSocket();
			Node connectedNode01 = connectedSocket01.Parent;
			Node connectedNode02 = connectedSocket02.Parent;

			float value01 = (float) connectedNode01.GetResultOf(connectedSocket01);
			float value02 = (float) connectedNode02.GetResultOf(connectedSocket02);

			if (!float.IsNaN(value01) && !float.IsNaN(value02))
			{
				return Calculate(value01, value02);
			}

			return float.NaN;
		}

		public float Calculate(float value01, float value02)
		{
			if (_selectedMode == 0)
			{
				return System.Math.Min(value01, value02);
			}
			if (_selectedMode == 1)
			{
				return System.Math.Max(value01, value02);
			}
			return float.NaN;
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return AllInputSocketsConnected();
		}
	}
}

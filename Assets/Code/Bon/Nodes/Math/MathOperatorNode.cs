using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Interface;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Math
{

	[Serializable]
	[GraphContextMenuItem("Math", "Operator")]
	public class MathOperatorNode : Node
	{

		[SerializeField]
		private int selectedMode = 0;

		public static readonly string[] Operations = { "add", "sub", "mul", "div" };

		private Socket inputSocket01;
		private Socket inputSocket02;

		public MathOperatorNode(int id) : base(id)
		{

			inputSocket01 = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			Sockets.Add(inputSocket01);
			inputSocket02 = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			Sockets.Add(inputSocket02);
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
			Height = 95;
			Width = 80;
		}

		public override void OnGUI()
		{

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			int newMode = GUILayout.SelectionGrid(selectedMode,Operations,1,"toggle");
			if (newMode != selectedMode)
			{
				selectedMode = newMode;
				OnChange();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		public override void OnSerialization(SerializableNode sNode)
		{

		}

		public override void OnDeserialization(SerializableNode sNode)
		{

		}

		public override object GetResultOf(Socket outSocket)
		{
			if (!CanGetResultOf(outSocket)) return float.NaN;

			Socket connectedSocket01 = inputSocket01.GetConnectedSocket();
			Socket connectedSocket02 = inputSocket02.GetConnectedSocket();
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

		public override bool CanGetResultOf(Socket outSocket)
		{
			return AllInputSocketsConnected();
		}

		public float Calculate(float value01, float value02)
		{
			if (selectedMode == 0)
			{
				return value01 + value02;
			}
			if (selectedMode == 1)
			{
				return value01 - value02;
			}
			if (selectedMode == 2)
			{
				return value01 * value02;
			}
			if (selectedMode == 3)
			{
				if (value02.Equals(0)) return float.NaN;
				return value01 / value02;
			}
			return float.NaN;
		}
	}
}

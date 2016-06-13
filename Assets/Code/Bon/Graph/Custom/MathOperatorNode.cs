using System;
using UnityEngine;

namespace Assets.Code.Bon.Graph.Custom
{
	[Serializable]
	[GraphContextMenuItem("Math", "Operator")]
	public class MathOperatorNode : Node, IMathNode
	{

		[SerializeField]
		private int selectedMode = 0;

		public static readonly string[] Operations = { "add", "sub", "mul", "div" };

		private Socket inputSocket01;
		private Socket inputSocket02;

		public MathOperatorNode(int id) : base(id)
		{

			inputSocket01 = new Socket(this, Color.red, SocketDirection.Input);
			Sockets.Add(inputSocket01);
			inputSocket02 = new Socket(this, Color.red, SocketDirection.Input);
			Sockets.Add(inputSocket02);
			Sockets.Add(new Socket(this, Color.red, SocketDirection.Output));
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

		public float GetNumber(Socket outSocket)
		{


			float value01;
			float value02;

			Socket connectedSocket01 = inputSocket01.GetConnectedSocket();
			Socket connectedSocket02 = inputSocket02.GetConnectedSocket();

			Debug.Log("So far 01 " + connectedSocket01);
			Debug.Log("So far 02 " + connectedSocket02);



			if (connectedSocket01 != null && connectedSocket02 != null)
			{

				Node connectedNode01 = connectedSocket01.Parent;
				Node connectedNode02 = connectedSocket02.Parent;

				Debug.Log("Sasaso far 01 " + connectedNode01);
				Debug.Log("SasaSo far 02 " + connectedNode02);

				value01 = ((IMathNode) connectedNode01).GetNumber(connectedSocket01);
				value02 = ((IMathNode) connectedNode02).GetNumber(connectedSocket02);

				if (!float.IsNaN(value01) && !float.IsNaN(value02))
				{
					return Calculate(value01, value02);
				}
			}
			return float.NaN;
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

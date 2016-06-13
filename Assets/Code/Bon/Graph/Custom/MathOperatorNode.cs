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
			return ((IMathNode) inputSocket01.Parent).GetNumber(null);
		}
	}
}

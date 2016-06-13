using System;
using UnityEngine;

namespace Assets.Code.Bon.Graph.Custom
{
	[Serializable]
	[GraphContextMenuItem("Math", "Operator")]
	public class MathOperatorNode : Node
	{

		[SerializeField]
		private int selectedMode = 0;

		public static readonly string[] Operations = { "add", "sub", "mul", "div" };

		public MathOperatorNode(int id) : base(id)
		{
			Sockets.Add(new Socket(this, Color.red, true));
			Sockets.Add(new Socket(this, Color.red, true));
			Sockets.Add(new Socket(this, Color.red, false));
			Height = 95;

		}

		public override void OnGUI()
		{

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			selectedMode = GUILayout.SelectionGrid(selectedMode,Operations,1,"toggle");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		public override void OnSerialization(SerializableNode sNode)
		{

		}

		public override void OnDeserialization(SerializableNode sNode)
		{

		}
	}
}

using System;
using UnityEngine;

namespace Assets.Code.Bon.Graph.Custom
{
	[Serializable]
	[GraphContextMenuItem("Standard", "Number")]
	public class NumberNode : Node
	{

		[SerializeField]
		public float number;

		private Rect textFieldArea = new Rect(10, 0, 80, BonConfig.SocketSize);

		public NumberNode(int id) : base(id)
		{
			Sockets.Add(new Socket(this, Color.red, false));
			Height = 20 + BonConfig.SocketOffsetTop;
		}

		public override void OnGUI()
		{
			string text = GUI.TextField(textFieldArea, number.ToString());

			float newNumber;
			if (text.Equals("") || text.Length == 0) text = "0";
			bool isNumeric = float.TryParse(text, out newNumber);
			if (isNumeric)
			{
				if (Math.Abs(newNumber - number) > 0)
				{
					// a new number is set by the user
					number = newNumber;
					OnChange();
				}
			}
		}

		public override void ApplySerializationData(SerializableNode sNode)
		{
			sNode.data = JsonUtility.ToJson(this);
		}
	}
}



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
			string textFieldValue = GUI.TextField(textFieldArea, number.ToString());
			float newNumber = GetValidNumber(textFieldValue);
			if (Math.Abs(newNumber - number) > 0)
			{
				number = newNumber;
				OnChange();
			}
		}

		private float GetValidNumber(string text)
		{
			float newNumber;
			if (text.Equals("") || text.Length == 0) text = "0";
			bool isNumeric = float.TryParse(text, out newNumber);
			if (isNumeric)
			{
				return newNumber;
			}
			return number;
		}

		public override void OnSerialization(SerializableNode sNode)
		{

		}

		public override void OnDeserialization(SerializableNode sNode)
		{

		}
	}
}



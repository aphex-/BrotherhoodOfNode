using System;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Math
{
	[Serializable]
	[GraphContextMenuItem("Math", "Number")]
	public class NumberNode : Node
	{

		public static Color FloatType = new Color(0.32f, 0.58f, 0.86f);

		[SerializeField]
		public float Number;

		private readonly Rect textFieldArea = new Rect(10, 0, 80, BonConfig.SocketSize);

		public NumberNode(int id) : base(id)
		{

			Sockets.Add(new Socket(this, FloatType, SocketDirection.Output));
			Height = 20 + BonConfig.SocketOffsetTop;
		}

		public override void OnGUI()
		{
			var textFieldValue = GUI.TextField(textFieldArea, Number.ToString());
			var newNumber = GetValidNumber(textFieldValue);
			if (System.Math.Abs(newNumber - Number) > 0)
			{
				Number = newNumber;
				OnChange();
			}
		}

		private float GetValidNumber(string text)
		{
			float newNumber;
			if (text.Equals("") || text.Length == 0) text = "0";
			var isNumeric = float.TryParse(text, out newNumber);
			if (isNumeric)
			{
				return newNumber;
			}
			return Number;
		}

		public override void OnSerialization(SerializableNode sNode)
		{

		}

		public override void OnDeserialization(SerializableNode sNode)
		{

		}

		public override object GetResultOf(Socket outSocket)
		{
			return Number;
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return true;
		}

	}
}



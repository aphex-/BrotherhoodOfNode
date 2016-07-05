using System;
using System.Globalization;
using System.Text.RegularExpressions;
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
		public string Number;

		private readonly Rect _textFieldArea = new Rect(10, 0, 80, BonConfig.SocketSize);

		public NumberNode(int id) : base(id)
		{
			Sockets.Add(new Socket(this, FloatType, SocketDirection.Output));
			Height = 20 + BonConfig.SocketOffsetTop;
		}

		public override void OnGUI()
		{
			if (NodeUtils.FloatTextField(_textFieldArea, ref Number))
			{
				TriggerChangeEvent();
			}
		}


		public override object GetResultOf(Socket outSocket)
		{
			return float.Parse(Number);
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return AllInputSocketsConnected();
		}

	}
}



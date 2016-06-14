using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Interface;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Math
{

	[Serializable]
	[GraphContextMenuItem("Math", "Display")]
	public class MathDisplay : Node
	{

		public float value;
		private string errorMessage;

		private const string NotConnectedMessage = "not connected";

		private Rect textFieldArea = new Rect(10, 0, 80, BonConfig.SocketSize);

		private Socket inSocket;

		public MathDisplay(int id) : base(id)
		{
			inSocket = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			Sockets.Add(inSocket);
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
			Height = 20 + BonConfig.SocketOffsetTop;
			//errorMessage = NotConnectedMessage;
		}

		public override void OnGUI()
		{
			if (errorMessage == null)
			{
				GUI.Label(textFieldArea, value.ToString());
			}
			else
			{
				GUI.Label(textFieldArea, errorMessage);
			}
		}

		public override void OnSerialization(SerializableNode sNode)
		{

		}

		public override void OnDeserialization(SerializableNode sNode)
		{

		}

		public override object GetResultOf(Socket outSocket)
		{
			if (CanGetResultOf(outSocket))
			{
				return UpdateValue();
			}
			else
			{
				return float.NaN;
			}
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			if (AllInputSocketsConnected())
			{
				errorMessage = null;
				return true;
			}
			else
			{
				errorMessage = NotConnectedMessage;
				return false;
			}
		}


		public float UpdateValue()
		{
			Socket connectedSocket = inSocket.GetConnectedSocket();
			Node connectedNode = connectedSocket.Parent;
			value = (float) connectedNode.GetResultOf(connectedSocket);
			return value;
		}
	}
}

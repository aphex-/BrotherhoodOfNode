using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Interface;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Math
{

	[Serializable]
	[GraphContextMenuItem("Math", "Display")]
	public class MathDisplay : Node, IUpdateable
	{

		[System.NonSerialized]
		public float Value;

		[System.NonSerialized]
		private string _errorMessage;

		private const string NotConnectedMessage = "not connected";

		[System.NonSerialized]
		private readonly Rect _textFieldArea = new Rect(10, 0, 80, BonConfig.SocketSize);

		[System.NonSerialized]
		private readonly Socket _inSocket;

		public MathDisplay(int id) : base(id)
		{
			_inSocket = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			Sockets.Add(_inSocket);
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
			Height = 20 + BonConfig.SocketOffsetTop;
		}

		public override void OnGUI()
		{
			if (_errorMessage == null)
			{
				GUI.Label(_textFieldArea, Value.ToString());
			}
			else
			{
				GUI.Label(_textFieldArea, _errorMessage);
			}
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
			return AllInputSocketsConnected();
		}

		private float UpdateValue()
		{
			Socket connectedSocket = _inSocket.GetConnectedSocket();
			Node connectedNode = connectedSocket.Parent;
			Value = (float) connectedNode.GetResultOf(connectedSocket);
			Debug.Log("UpdateValue " + Value);
			return Value;
		}

		public void Update()
		{
			if (AllInputSocketsConnected())
			{
				_errorMessage = null;
				UpdateValue();
			}
			else
			{
				_errorMessage = NotConnectedMessage;
			}
		}
	}
}

using System;
using UnityEngine;
using System.Collections;
using Assets.Code.Bon;
using Assets.Code.Bon.Graph;

[Serializable]
[GraphContextMenuItem("Math", "Display")]
public class MathDisplay : Node, IMathNode
{

	public float value;
	private string errorMessage;

	private const string NotConnectedMessage = "not connected";

	private Rect textFieldArea = new Rect(10, 0, 80, BonConfig.SocketSize);

	private Socket inSocket;

	public MathDisplay(int id) : base(id)
	{
		inSocket = new Socket(this, Color.red, SocketDirection.Input);
		Sockets.Add(inSocket);
		Sockets.Add(new Socket(this, Color.red, SocketDirection.Output));
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

	public float GetNumber(Socket outSocket)
	{
		return value;
	}

	public void UpdateValue()
	{
		if (inSocket.Edge == null)
		{
			errorMessage = NotConnectedMessage;
		}
		else
		{
			value = ((IMathNode) inSocket.Edge.GetOtherSocket(inSocket).Parent).GetNumber(null);
			errorMessage = null;
		}
	}
}

using UnityEngine;
using System.Collections;
using Assets.Code.Bon.Graph;

public class Multiplexer : Node {

	public Multiplexer(int id) : base(id)
	{
		Sockets.Add(new Socket(this, Color.white, true));
		Sockets.Add(new Socket(this, Color.white, false));
		Height = 200;
	}

	public override void OnGUI()
	{

	}
}

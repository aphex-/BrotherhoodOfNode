using System;
using Assets.Code.Bon.Nodes.Math;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Map
{
	[Serializable]
	[GraphContextMenuItem("Map", "Display")]
	public class MapDisplayNode : Node, IUpdateable {

		private readonly Texture2D _texture2D;


		public MapDisplayNode(int id) : base(id)
		{
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Input));
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
			_texture2D = new Texture2D(100, 100, TextureFormat.ARGB32, false);
		}

		public override void OnGUI()
		{
			GUI.DrawTexture(new Rect(6, 0, _texture2D.width, _texture2D.height), _texture2D);
		}


		public override object GetResultOf(Socket outSocket)
		{
			return null;
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return false;
		}

		public void Update()
		{

		}
	}
}

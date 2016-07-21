using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.Bon.Nodes.Math;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Map
{
	[Serializable]
	[GraphContextMenuItem("Map", "Display")]
	public class MapDisplayNode : Node, IUpdateable {

		private Texture2D _texture2D;
		private readonly Socket _inputSocket;
		private string _errorMessage = "";
		private bool _samplerConnected = false;

		[SerializeField]
		public int GUISize = 100;

		public MapDisplayNode(int id) : base(id)
		{
			_inputSocket = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			Sockets.Add(_inputSocket);
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
			InitTexture(GUISize);
			Update();
		}

		public override void OnDeserialization(SerializableNode sNode)
		{
			InitTexture(GUISize);
			Update();
		}

		public override void OnGUI()
		{
			if (_errorMessage == null)
			{
				GUI.Label(new Rect(0, 0, 25, 15), "size");
				if (GUI.Button(new Rect(25, 0, 18, 18), "+"))
				{
					InitTexture(GUISize + 50);
					Update();
				}
				if (GUI.Button(new Rect(43, 0, 18, 18), "-"))
				{
					InitTexture(GUISize - 50);
					Update();
				}

				//GUI.Label(new Rect(0, 20, 40, 15), "zoom");
				//GUI.Button(new Rect(40, 20, 20, 20), "+");
				//GUI.Button(new Rect(60, 20, 20, 20), "-");
				if (_texture2D != null) GUI.DrawTexture(new Rect(6, 24, _texture2D.width, _texture2D.height), _texture2D);
			}
			else
			{
				GUI.Label(new Rect(0, 0, 100, 15), _errorMessage);
			}
		}

		private void InitTexture(int size)
		{
			Debug.Log("InitTexture " + size);
			if (size <= 99) return;
			GUISize = size;
			if (_texture2D != null) Texture2D.DestroyImmediate(_texture2D);
			_texture2D = new Texture2D(GUISize, GUISize, TextureFormat.ARGB32, false);
			Width = size + 12;
			Height = size + 50;
		}

		public override object GetResultOf(Socket outSocket)
		{
			if (!CanGetResultOf(null))
			{
				return float.NaN;
			}
			else
			{
				return _inputSocket.GetConnectedSocket().Parent.GetResultOf(_inputSocket.GetConnectedSocket());
			}

	}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return AllInputSocketsConnected();
		}

		public void Update()
		{
			UpdateTexture();
		}

		public Texture2D UpdateTexture()
		{
			if (!AllInputSocketsConnected())
			{
				_errorMessage = NodeUtils.NotConnectedMessage;
				return null;
			}

			_errorMessage = null;

			IList<ISampler2D> samplers = Graph.CreateUpperNodesList(this).OfType<ISampler2D>().ToList();
			if (samplers.Count > 0 && _texture2D != null)
			{
				for (int x = 0; x < _texture2D.width; x++)
				{
					for (int y = 0; y < _texture2D.height; y++)
					{
						foreach (ISampler2D sampler in samplers)
						{
							sampler.SampleFrom(x, y);
							float value = (float) _inputSocket.GetConnectedSocket().Parent.GetResultOf(_inputSocket.GetConnectedSocket());
							_texture2D.SetPixel(x, y, NodeUtils.GetMapValueColor(value));
						}
					}
				}
			}

			if (samplers.Count == 0) _errorMessage = "no sample data";
			_samplerConnected = samplers.Count > 0;
			if (_texture2D != null) _texture2D.Apply();
			return _texture2D;
		}

		public override void OnFocus()
		{
			base.OnFocus();
			// this is a fix to reload textures that did not load during startup (if unity starts with an open EditorWindow)
			if (_texture2D == null)
			{
				InitTexture(GUISize);
				UpdateTexture();
			}
		}
	}
}


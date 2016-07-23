using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes.Math;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Map
{
	[Serializable]
	[GraphContextMenuItem("Map", "Display")]
	public class MapDisplayNode : Node, IUpdateable, ISampler2D {

		private readonly Socket _inputSocket;
		private IList<INode2D> _connectedSamplers;

		private Rect _sizeLabel = 			new Rect(0, 0, 25, 15);
		private Rect _sizePlusButton =		new Rect(25, 0, 18, 18);
		private Rect _sizeMinusButton = 	new Rect(43, 0, 18, 18);
		private Rect _textureArea = 		new Rect();
		private Rect _errorMessageLabel = 	new Rect(0, 0, 100, 15);

		private MapDisplayUpdateJob _job = new MapDisplayUpdateJob();
		private Texture2D _texture;

		private bool _isUpdating = false;
		private bool _isConnected = false;
		private bool _isSamplerConnected = false;

		[SerializeField]
		public int TextureSize = 100;

		public MapDisplayNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket = new Socket(this, NumberNode.FloatType, SocketDirection.Input);
			Sockets.Add(_inputSocket);
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
			InitTexture(TextureSize);
			UpdateConnectedSamplerList();
			Update();
		}

		public override void OnDeserialization(SerializableNode sNode)
		{
			InitTexture(TextureSize);
			Update();
		}

		private bool UpdateJob()
		{
			if (_job != null)
			{
				_job.Update();
				if (!_job.IsDone) return true;
				else
				{
					_texture = _job.Texture;
					_job.Abort();
					_job = null;
					return false;
				}
			}
			return false;
		}

		public override void OnGUI()
		{
			_isUpdating = UpdateJob();
			_isConnected = AllInputSocketsConnected();
			_isSamplerConnected = _connectedSamplers != null && _connectedSamplers.Count > 0;

			if (!_isUpdating && _isConnected && _isSamplerConnected)
			{
				GUI.Label(_sizeLabel, "size");
				if (GUI.Button(_sizePlusButton, "+"))
				{
					InitTexture(TextureSize + 50);
					Update();
				}
				if (GUI.Button(_sizeMinusButton, "-"))
				{
					InitTexture(TextureSize - 50);
					Update();
				}

				if (_texture != null)
				{
					_textureArea.Set(6, 24, _texture.width, _texture.height);
					GUI.DrawTexture(_textureArea, _texture);
				}
			}


			if (_isUpdating) GUI.Label(_errorMessageLabel, "updating data..");
			else if (!_isConnected) GUI.Label(_errorMessageLabel, NodeUtils.NotConnectedMessage);
			else if (!_isSamplerConnected) GUI.Label(_errorMessageLabel, "no sampler data");

		}

		private void InitTexture(int size)
		{
			if (size <= 99) return;
			TextureSize = size;
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
				return null;
			}
			UpdateConnectedSamplerList();
			if (_connectedSamplers.Count > 0)
			{
				if (_job != null && !_job.IsDone) _job.Abort();
				if (_job == null) _job = new MapDisplayUpdateJob();
				_job.Request(_inputSocket, _connectedSamplers, 0, 0, TextureSize, TextureSize);
				_job.Start();
			}
			return null;
		}

		public void UpdateConnectedSamplerList()
		{
			_connectedSamplers = Graph.CreateUpperNodesList(this).OfType<INode2D>().ToList();
		}

		public float GetSampleAt(float x, float y)
		{
			foreach (INode2D sampler in _connectedSamplers)
			{
				sampler.SampleFrom(x, y);
			}
			return (float) _inputSocket.GetConnectedSocket().Parent.GetResultOf(_inputSocket.GetConnectedSocket());
		}

	}
}


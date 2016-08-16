using System;
using Assets.Code.Bon.Interface;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Number.Map2D
{
	[Serializable]
	[GraphContextMenuItem("Number/Map2D", "Display")]
	public class MapDisplayNode : AbstractMap2DNode {

		[NonSerialized] private Socket _inputSocketNumber;
		[NonSerialized] private Socket _inputSocketColor;
		[NonSerialized] private Socket _inputSocketPosition;

		[NonSerialized] private Rect _sizeLabel;
		[NonSerialized] private Rect _sizePlusButton;
		[NonSerialized] private Rect _sizeMinusButton;

		[NonSerialized] private bool _isConnected;

		public MapDisplayNode(int id, Graph parent) : base(id, parent)
		{
			_sizeLabel = new Rect(3, 0, 25, 15);
			_sizePlusButton = new Rect(28, 0, 18, 18);
			_sizeMinusButton = new Rect(46, 0, 18, 18);

			_inputSocketNumber = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_inputSocketNumber);

			_inputSocketColor = new Socket(this, typeof(AbstractColorNode), SocketDirection.Input);
			Sockets.Add(_inputSocketColor);

			_inputSocketPosition = new Socket(this, typeof(AbstractPositionNode), SocketDirection.Input);
			Sockets.Add(_inputSocketPosition);

			_textures.Add(new GUIThreadedTexture()); // heightmap
			_textures.Add(new GUIThreadedTexture()); // points
		}

		public override void OnGUI()
		{
			if (!_textures[0].DoneInitialUpdate) _textures[0].StartTextureUpdateJob((int) Width -12, (int) Height - 50, GetNumberSampler(), GetColorSampler());
			if (!_textures[1].DoneInitialUpdate) _textures[1].StartTextureUpdateJob((int) Width -12, (int) Height - 50, GetNumberSampler(), GetColorSampler());

			_isConnected = CanGetResultOf(null);

			if (!IsUpdatingTexture() && _isConnected)
			{
				_sizeLabel.Set(_sizeLabel.x, Height - 45, _sizeLabel.width, _sizeLabel.height);
				_sizePlusButton.Set(_sizePlusButton.x, Height - 45, _sizePlusButton.width, _sizePlusButton.height);
				_sizeMinusButton.Set(_sizeMinusButton.x, Height - 45, _sizeMinusButton.width, _sizeMinusButton.height);

				GUI.Label(_sizeLabel, "size");
				if (GUI.Button(_sizePlusButton, "+"))
				{
					ChangeTextureSize(+50);
				}
				if (GUI.Button(_sizeMinusButton, "-"))
				{
					ChangeTextureSize(-50);
				}
			}
			DrawTextures();
			//Width = CurrentTextureSize + 12;
			//Height = CurrentTextureSize + 50;
		}

		private void ChangeTextureSize(int size)
		{
			Width += size;
			Height += size;
			Update();

		}

		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed);
		}

		public override void Update()
		{
			if (Collapsed) return;

			if (_inputSocketNumber.CanGetResult())
				_textures[0].StartTextureUpdateJob((int) Width -12, (int) Height - 50, GetNumberSampler(), GetColorSampler());
			else _textures[0].Hide();

			if (_inputSocketPosition.CanGetResult())
				_textures[1].StartTextureUpdateJob((int) Width -12, (int) Height - 50, GetPositionSampler());
			else _textures[1].Hide();
		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			return GetInputNumber(_inputSocketNumber, x, y, seed);
		}

		private IColorSampler1D GetColorSampler()
		{
			if (_inputSocketColor.CanGetResult()) return (AbstractColorNode) _inputSocketColor.GetConnectedSocket().Parent;
			return null;
		}

		private ISampler3D GetNumberSampler()
		{
			if (_inputSocketNumber.IsInDirectInputMode()) return new SingleNumberSampler(GetInputNumber(_inputSocketNumber, 0, 0, 0));
			if (_inputSocketNumber.CanGetResult()) return (AbstractNumberNode) _inputSocketNumber.GetConnectedSocket().Parent;
			return null;
		}

		private IPositionSampler GetPositionSampler()
		{
			if (_inputSocketPosition.CanGetResult()) return (AbstractPositionNode) _inputSocketPosition.GetConnectedSocket().Parent;
			return null;
		}


	}
}


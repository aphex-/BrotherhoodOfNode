using System;
using System.Collections.Generic;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes.Number;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Noise
{
	[Serializable]
	[GraphContextMenuItem("Noise", "Display")]
	public class NoiseDisplayNode : AbstractNoiseNode
	{
		[SerializeField] private int _sizeModifcator;

		[NonSerialized] private InputSocket _inputSocketNumber;
		[NonSerialized] private InputSocket _inputSocketColor;
		[NonSerialized] private InputSocket _inputSocketPosition;

		[NonSerialized] private Rect _sizeLabel;
		[NonSerialized] private Rect _sizePlusButton;
		[NonSerialized] private Rect _sizeMinusButton;

		[NonSerialized] private const int _sizeStep = 50;

		private List<UnityEngine.Vector3> _lastVectors;

		private bool _initializedSize;
		private Rect _tmpRect;

		public NoiseDisplayNode(int id, Graph parent) : base(id, parent)
		{
			_sizeLabel = new Rect(3, 0, 25, 15);
			_sizePlusButton = new Rect(28, 0, 18, 18);
			_sizeMinusButton = new Rect(46, 0, 18, 18);

			_inputSocketNumber = new InputSocket(this, typeof(AbstractNumberNode));
			Sockets.Add(_inputSocketNumber);

			_inputSocketColor = new InputSocket(this, typeof(AbstractColorNode));
			Sockets.Add(_inputSocketColor);

			_inputSocketPosition = new InputSocket(this, typeof(AbstractVector3Node));
			Sockets.Add(_inputSocketPosition);

			_textures.Add(new GUIThreadedTexture()); // heightmap
			_textures.Add(new GUIThreadedTexture()); // points

			_tmpRect = new Rect();
			Width = 160;
			Height = 220;
		}

		public override void OnGUI()
		{

			if (!_initializedSize) ChangeTextureSize(_sizeModifcator * _sizeStep);

			if (!_textures[0].DoneInitialUpdate) _textures[0].StartTextureUpdateJob((int) Width -10, (int) Height - 70, GetNumberSampler(), GetColorSampler());
			if (!_textures[1].DoneInitialUpdate) _textures[1].StartTextureUpdateJob((int) Width -10, (int) Height - 70, GetNumberSampler(), GetColorSampler());

			if (!IsUpdatingTexture())
			{
				_sizeLabel.Set(_sizeLabel.x, Height - 65, _sizeLabel.width, _sizeLabel.height);
				_sizePlusButton.Set(_sizePlusButton.x, Height - 65, _sizePlusButton.width, _sizePlusButton.height);
				_sizeMinusButton.Set(_sizeMinusButton.x, Height - 65, _sizeMinusButton.width, _sizeMinusButton.height);

				GUI.Label(_sizeLabel, "size");
				if (GUI.Button(_sizePlusButton, "+"))
				{
					ChangeTextureSize(+_sizeStep);
					_sizeModifcator++;
				}
				if (Width > 100 && GUI.Button(_sizeMinusButton, "-"))
				{
					ChangeTextureSize(-_sizeStep);
					_sizeModifcator--;
				}
			}
			DrawTextures();
			//Width = CurrentTextureSize + 12;
			//Height = CurrentTextureSize + 50;

			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, Height - 45, 100, 20);
			GUI.Label(_tmpRect, "w" + _textures[0].Width + " h" + _textures[0].Height);

			if (_lastVectors != null)
			{
				_tmpRect.Set(70, Height - 45, 100, 20);
				GUI.Label(_tmpRect, "vec" + _lastVectors.Count);
			}

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		private void ChangeTextureSize(int size)
		{
			_initializedSize = true;
			Width += size;
			Height += size;
			Update();
		}

		public override void Update()
		{
			if (Collapsed) return;

			if (_inputSocketNumber.CanGetResult())
				_textures[0].StartTextureUpdateJob((int) Width -10, (int) Height - 70, GetNumberSampler(), GetColorSampler());
			else _textures[0].Hide();


			if (_inputSocketPosition.CanGetResult())
			{
				_lastVectors = GetPositionSampler().GetVector3List(
					_inputSocketPosition.GetConnectedSocket(),
					0, 0, 0, (int) Width - 10, 0, (int) Height - 70, 0);
				_textures[1].StartTextureUpdateJob((int) Width - 10, (int) Height - 70, _lastVectors);
			}
			else
			{
				_textures[1].Hide();
				_lastVectors = null;
			}
		}

		public override float GetNumber(OutputSocket outSocket, float x, float y, float z, float seed)
		{
			return GetInputNumber(_inputSocketNumber, x, y, z, seed);
		}

		private IColorSampler GetColorSampler()
		{
			if (_inputSocketColor.CanGetResult()) return (AbstractColorNode) _inputSocketColor.GetConnectedSocket().Parent;
			return null;
		}

		private INumberSampler GetNumberSampler()
		{
			if (_inputSocketNumber.IsInDirectInputMode()) return new SingleNumberSampler(GetInputNumber(_inputSocketNumber, 0, 0, 0, 0));
			if (_inputSocketNumber.CanGetResult()) return (AbstractNumberNode) _inputSocketNumber.GetConnectedSocket().Parent;
			return null;
		}

		private IVectorSampler GetPositionSampler()
		{
			if (_inputSocketPosition.CanGetResult()) return (AbstractVector3Node) _inputSocketPosition.GetConnectedSocket().Parent;
			return null;
		}


	}
}


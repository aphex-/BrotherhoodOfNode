using System;
using Assets.Code.Bon.Interface;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Number.Map2D
{
	[Serializable]
	[GraphContextMenuItem("Number/Map2D", "Display")]
	public class MapDisplayNode : AbstractMap2DNode, IUpdateable {

		[SerializeField] public int TextureSize = 100;

		[NonSerialized] private Socket _inputSocketNumber;
		[NonSerialized] private Socket _inputSocketColor;

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
		}

		public override void OnGUI()
		{

			_isConnected = CanGetResultOf(null);

			if (!_isUpdatingTexture && _isConnected)
			{
				_sizeLabel.Set(_sizeLabel.x, Height - 45, _sizeLabel.width, _sizeLabel.height);
				_sizePlusButton.Set(_sizePlusButton.x, Height - 45, _sizePlusButton.width, _sizePlusButton.height);
				_sizeMinusButton.Set(_sizeMinusButton.x, Height - 45, _sizeMinusButton.width, _sizeMinusButton.height);

				GUI.Label(_sizeLabel, "size");
				if (GUI.Button(_sizePlusButton, "+"))
				{
					ChangeTextureSize(TextureSize + 50);
				}
				if (GUI.Button(_sizeMinusButton, "-"))
				{
					ChangeTextureSize(TextureSize - 50);
				}
			}
			DrawTexture();
			Width = CurrentTextureSize + 12;
			Height = CurrentTextureSize + 50;
		}

		private void ChangeTextureSize(int size)
		{
			if (size <= 99) return;
			TextureSize = size;
			CurrentTextureSize = TextureSize;
			CreateTexture();
			StartTextureUpdateJob();
		}

		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed);
		}

		public void Update()
		{
			StartTextureUpdateJob();
		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			return GetInputNumber(_inputSocketNumber, x, y, seed);
		}

		protected override bool CanCreatePreview()
		{
			return true;
		}

		protected override IColorSampler1D GetColorSampler()
		{
			if (_inputSocketColor.CanGetResult())
			{
				AbstractColorNode node = (AbstractColorNode) _inputSocketColor.GetConnectedSocket().Parent;
				return node;
			}
			return null;
		}
	}
}


using System;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Noise
{
	[Serializable]
	[GraphContextMenuItem("Noise", "Sine")]
	public class SineMapNode : AbstractNoiseNode {


		[NonSerialized] private Rect _textLabelScaleX;
		[NonSerialized] private Rect _textLabelScaleY;

		[NonSerialized] private Socket _socketInputX;
		[NonSerialized] private Socket _socketInputY;

		public SineMapNode(int id, Graph parent) : base(id, parent)
		{
			_textLabelScaleX = new Rect(6, 0, 65, BonConfig.SocketSize);
			_textLabelScaleY = new Rect(6, 20, 65, BonConfig.SocketSize);
			_socketInputX = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_socketInputX);
			_socketInputY = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_socketInputY);

			Height = 60;
			_textures.Add(new GUIThreadedTexture()); // heightmap
		}

		public override void OnGUI()
		{
			if (!_textures[0].DoneInitialUpdate) Update();

			_textures[0].X = 48;
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(_textLabelScaleX, "scale x");
			GUI.Label(_textLabelScaleY, "scale y");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			DrawTextures();
		}

		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed);
		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			var scaleX = GetInputNumber(_socketInputX, x, y, seed);
			var scaleY = GetInputNumber(_socketInputY, x, y, seed);

			if (float.IsNaN(scaleX) || float.IsNaN(scaleY)) return float.NaN;

			if (scaleX == 0) scaleX = 1;
			if (scaleY == 0) scaleY = 1;

			return (float) (Math.Sin(x / scaleX + seed) + Math.Sin(y / scaleY + seed)) / 2f;
		}

		/*protected override IColorSampler1D GetColorSampler()
	{
		return null;
	}*/

		public override void Update()
		{
			if (!Collapsed) _textures[0].StartTextureUpdateJob(45, 35, this, null);
		}
	}
}

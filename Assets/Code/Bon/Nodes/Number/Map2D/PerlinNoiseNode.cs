using System;
using Assets.Code.Bon.Interface;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Number.Map2D
{
	[Serializable]
	[GraphContextMenuItem("Number/Map2D", "UnityPerlinNoise")]
	public class PerlinNoiseNode : AbstractMap2DNode, IUpdateable
	{

		[NonSerialized] private Rect _labelScale;
		[NonSerialized] private Rect _labelSeed;

		[NonSerialized] private Socket _inputSocketScale;
		[NonSerialized] private Socket _inputSocketSeed;

		public PerlinNoiseNode(int id, Graph parent) : base(id, parent)
		{
			_labelScale = new Rect(6, 110, 30, BonConfig.SocketSize);
			_labelSeed = new Rect(6, 130, 30, BonConfig.SocketSize);
			_inputSocketScale = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputSocketSeed = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_inputSocketScale);
			Sockets.Add(_inputSocketSeed);

			Height = CurrentTextureSize + 70;
			SocketTopOffsetInput = 110;
		}

		public override void OnGUI()
		{
			GUI.Label(_labelScale, "scale");
			GUI.Label(_labelSeed, "seed");
			DrawTexture();
		}


		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed);
		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			var scale = GetInputNumber(_inputSocketScale, x, y, seed);
			var internalSeed = GetInputNumber(_inputSocketSeed, x, y, seed);

			if (float.IsNaN(scale) || float.IsNaN(internalSeed)) return float.NaN;

			if (scale == 0) scale = 1;

			if (Math.Abs(internalSeed) > 0.0000001) internalSeed = seed;

			float noise = Mathf.PerlinNoise(x / scale + internalSeed, y / scale + internalSeed) * 2f - 1f;
			return Math.Max(Math.Min(noise, 1), -1);
		}

		protected override bool CanCreatePreview()
		{
			return true;
		}

		protected override IColorSampler1D GetColorSampler()
		{
			return null;
		}

		public void Update()
		{
			StartTextureUpdateJob();
		}
	}
}

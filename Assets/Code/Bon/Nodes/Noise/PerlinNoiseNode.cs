using System;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Noise
{
	[Serializable]
	[GraphContextMenuItem("Noise", "UnityPerlinNoise")]
	public class PerlinNoiseNode : AbstractNoiseNode
	{

		[NonSerialized] private Rect _labelScale;
		[NonSerialized] private Rect _labelSeed;

		[NonSerialized] private Socket _inputSocketScale;
		[NonSerialized] private Socket _inputSocketSeed;

		public PerlinNoiseNode(int id, Graph parent) : base(id, parent)
		{
			_labelScale = new Rect(6, 0, 30, BonConfig.SocketSize);
			_labelSeed = new Rect(6, 20, 30, BonConfig.SocketSize);
			_inputSocketScale = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputSocketSeed = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);

			_inputSocketScale.SetDirectInputNumber(5, false);

			Sockets.Add(_inputSocketScale);
			Sockets.Add(_inputSocketSeed);

			//Height = CurrentTextureSize + 70;
			_textures.Add(new GUIThreadedTexture()); // heightmap
			Height = 60;
		}

		public override void OnGUI()
		{
			if (!_textures[0].DoneInitialUpdate) Update();

			_textures[0].X = 40;

			GUI.Label(_labelScale, "scale");
			GUI.Label(_labelSeed, "seed");
			DrawTextures();
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

		public override void Update()
		{
			if (!Collapsed) _textures[0].StartTextureUpdateJob(55, 35, this, null);
		}
	}
}

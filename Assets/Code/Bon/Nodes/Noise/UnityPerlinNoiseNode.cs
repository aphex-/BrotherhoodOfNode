using System;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Noise
{
	[Serializable]
	[GraphContextMenuItem("Noise", "UnityPerlinNoise")]
	public class UnityPerlinNoiseNode : AbstractNoiseNode
	{

		[NonSerialized] private Rect _labelScale;
		[NonSerialized] private Rect _labelSeed;

		[NonSerialized] private InputSocket _inputSocketScale;
		[NonSerialized] private InputSocket _inputSocketSeed;

		public UnityPerlinNoiseNode(int id, Graph parent) : base(id, parent)
		{
			_labelScale = new Rect(6, 0, 30, BonConfig.SocketSize);
			_labelSeed = new Rect(6, 20, 30, BonConfig.SocketSize);
			_inputSocketScale = new InputSocket(this, typeof(AbstractNumberNode));
			_inputSocketSeed = new InputSocket(this, typeof(AbstractNumberNode));

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


		public override float GetNumber(OutputSocket outSocket, float x, float y, float z, float seed)
		{
			var scale = GetInputNumber(_inputSocketScale, x, y, z, seed);
			var socketSeed = GetInputNumber(_inputSocketSeed, x, y, z, seed);

			if (float.IsNaN(scale) || float.IsNaN(socketSeed)) return float.NaN;

			if (scale == 0) scale = 1;

			var modifiedSeed = NodeUtils.ModifySeed(socketSeed, seed);

			float noise = Mathf.PerlinNoise(x / scale + modifiedSeed, z / scale + modifiedSeed) * 2f - 1f;
			return Math.Max(Math.Min(noise, 1), -1);
		}

		public override void Update()
		{
			if (!Collapsed) _textures[0].StartTextureUpdateJob(55, 35, this, null);
		}
	}
}

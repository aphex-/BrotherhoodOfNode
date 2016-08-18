using System;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Noise
{
	[Serializable]
	[GraphContextMenuItem("Noise", "Octave")]
	public class OctaveNode : AbstractNumberNode {


		[NonSerialized] private InputSocket _inputNoiseSocket;
		[NonSerialized] private InputSocket _inputIterationSocket;
		[NonSerialized] private InputSocket _inputLacunaritySocket;
		[NonSerialized] private InputSocket _inputPersistanceSocket;

		[NonSerialized] private Rect _labelNoise;
		[NonSerialized] private Rect _labelIteration;
		[NonSerialized] private Rect _labelLacunarity;
		[NonSerialized] private Rect _labelPersistance;

		public OctaveNode(int id, Graph parent) : base(id, parent)
		{
			_labelNoise = new Rect(3, 0, 100, 20);
			_labelIteration = new Rect(3, 20, 100, 20);
			_labelLacunarity = new Rect(3, 40, 100, 20);
			_labelPersistance = new Rect(3, 60, 100, 20);

			_inputNoiseSocket = new InputSocket(this, typeof(AbstractNumberNode));
			_inputIterationSocket = new InputSocket(this, typeof(AbstractNumberNode));
			_inputLacunaritySocket = new InputSocket(this, typeof(AbstractNumberNode));
			_inputPersistanceSocket = new InputSocket(this, typeof(AbstractNumberNode));

			_inputIterationSocket.SetDirectInputNumber(4, false);
			_inputLacunaritySocket.SetDirectInputNumber(3, false);
			_inputPersistanceSocket.SetDirectInputNumber(0.2f, false);

			Sockets.Add(_inputNoiseSocket);
			Sockets.Add(_inputIterationSocket);
			Sockets.Add(_inputLacunaritySocket);
			Sockets.Add(_inputPersistanceSocket);
			Width = 100;
		}

		public override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(_labelNoise, "noise");
			GUI.Label(_labelIteration, "iteration");
			GUI.Label(_labelLacunarity, "lacunarity");
			GUI.Label(_labelPersistance, "persistance");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocker, float x, float y, float z, float seed)
		{
			var iterations = GetInputNumber(_inputIterationSocket, x, y, z, seed);
			var lacunarity = GetInputNumber(_inputLacunaritySocket, x, y, z, seed);
			var persistance = GetInputNumber(_inputPersistanceSocket, x, y, z, seed);

			if (float.IsNaN(iterations) || float.IsNaN(lacunarity) || float.IsNaN(persistance)) return float.NaN;

			float noiseHeight = 0;
			var frequency = 1f;
			var amplitude = 1f;

			for (var i = 0; i < (int) iterations; i++)
			{
				var noise = GetInputNumber(_inputNoiseSocket, x * frequency, y * frequency, z * frequency, seed / (i + 1)) * 2 - 1;
				noiseHeight += noise * amplitude;
				amplitude *= persistance;
				frequency *= lacunarity;
			}
			noiseHeight = (noiseHeight + 1f) / 2f;
			return noiseHeight;
		}

	}
}

using System;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Number.Map2D
{
	[Serializable]
	[GraphContextMenuItem("Number/Map2D", "Octave")]
	public class OctaveNode : AbstractNumberNode {


		[NonSerialized] private Socket _inputNoiseSocket;
		[NonSerialized] private Socket _inputIterationSocket;
		[NonSerialized] private Socket _inputLacunaritySocket;
		[NonSerialized] private Socket _inputPersistanceSocket;

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

			_inputNoiseSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputIterationSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputLacunaritySocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputPersistanceSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_inputNoiseSocket);
			Sockets.Add(_inputIterationSocket);
			Sockets.Add(_inputLacunaritySocket);
			Sockets.Add(_inputPersistanceSocket);
			Width = 80;
		}

		public override void OnGUI()
		{
			GUI.Label(_labelNoise, "noise");
			GUI.Label(_labelIteration, "iteration");
			GUI.Label(_labelLacunarity, "lacunarity");
			GUI.Label(_labelPersistance, "persistance");
		}

		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed);
		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			var iterations = GetInputNumber(_inputIterationSocket, x, y, seed);
			var lacunarity = GetInputNumber(_inputLacunaritySocket, x, y, seed);
			var persistance = GetInputNumber(_inputPersistanceSocket, x, y, seed);

			if (float.IsNaN(iterations) || float.IsNaN(lacunarity) || float.IsNaN(persistance)) return float.NaN;

			float noiseHeight = 0;
			var frequency = 1f;
			var amplitude = 1f;

			for (var i = 0; i < (int) iterations; i++)
			{
				var noise = GetInputNumber(_inputNoiseSocket, x * frequency, y * frequency, seed / i) * 2 - 1;
				noiseHeight += noise * amplitude;
				amplitude *= persistance;
				frequency *= lacunarity;
			}
			noiseHeight = (noiseHeight + 1f) / 2f;
			return noiseHeight;
		}

	}
}

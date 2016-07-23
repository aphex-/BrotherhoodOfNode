using System;
using System.Text.RegularExpressions;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes.Math;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Map
{
	[Serializable]
	[GraphContextMenuItem("Map", "UnityPerlinNoise")]
	public class PerlinNoiseNode : Node, INode2D, IUpdateable, ISampler2D
	{

		private float _samplingX = 0;
		private float _samplingY = 0;

		private Texture2D _texture2D;

		private readonly Rect _labelScale = 	new Rect(6, 	110, 30, BonConfig.SocketSize);
		private readonly Rect _textFieldScale = new Rect(40, 	110, 65, BonConfig.SocketSize);

		private readonly Rect _labelSeed = 		new Rect(6, 	130, 30, BonConfig.SocketSize);
		private readonly Rect _textFieldSeed = 	new Rect(40, 	130, 65, BonConfig.SocketSize);

		private Rect _textureArea = new Rect();

		private static int _textureSize = 100;

		public string Scale = "10";
		public string Seed = "0";

		private string _lastUpdateScale;
		private string _lastUpdateSeed;

		public PerlinNoiseNode(int id, Graph parent) : base(id, parent)
		{
			Sockets.Add(new Socket(this, NumberNode.FloatType, SocketDirection.Output));
			_texture2D =  new Texture2D(_textureSize, _textureSize, TextureFormat.RGB24, false);
			_textureArea.Set(6, 0, _texture2D.width, _texture2D.height);
			Width = _texture2D.width + 12;
			Height = _texture2D.height + 70;
			Update();
		}

		public override void OnGUI()
		{
			if (_texture2D != null) GUI.DrawTexture(_textureArea, _texture2D);
			GUI.Label(_labelScale, "scale");
			if (NodeUtils.FloatTextField(_textFieldScale, ref Scale)) TriggerChangeEvent();
			GUI.Label(_labelSeed, "seed");
			if (NodeUtils.FloatTextField(_textFieldSeed, ref Seed)) TriggerChangeEvent();
		}


		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_samplingX, _samplingY);
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return true;
		}

		public void SampleFrom(float x, float y)
		{
			_samplingX = x;
			_samplingY = y;
		}

		public float GetSampleAt(float x, float y)
		{
			float scale = float.Parse(Scale);
			float seed = float.Parse(Seed);
			return Mathf.PerlinNoise((x / scale) + seed, (y / scale) + seed);
		}

		public void Update()
		{
			if (_lastUpdateScale == Scale && _lastUpdateSeed == Seed) return;
			for (int x = 0; x < _texture2D.width; x++)
			{
				for (int y = 0; y < _texture2D.height; y++)
				{
					float value = GetSampleAt(x, y);
					_texture2D.SetPixel(x, y, NodeUtils.GetMapValueColor(value));
				}
			}

			_lastUpdateScale = Scale;
			_lastUpdateSeed = Seed;
			_texture2D.Apply();
		}
	}
}

using System;
using System.Collections.Generic;
using Assets.Code.Bon.Interface;
using Assets.Code.Thread;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Number.Map2D
{
	public class TextureUpdateJob : ThreadedJob
	{

		private ISampler3D _sampler3D;
		private IColorSampler1D _samplerColor;
		private List<Vector3> _positions;

		private int _width;
		private int _height;
		private float[,] _values;

		public Texture2D Texture;

		public void Request(int width, int height, ISampler3D sampler, IColorSampler1D colorSampler = null)
		{
			_sampler3D = sampler;
			_samplerColor = colorSampler;
			Init(width, height);
		}

		public void Request(int width, int height, List<Vector3> positions)
		{
			_positions = positions;
			_samplerColor = new PositionDisplayColorSampler();
			Init(width, height);
		}

		private void Init(int width, int height)
		{
			_width = width;
			_height = height;
			_values = new float[width, height];
		}

		protected override void ThreadFunction()
		{
			var _xStart = 0;
			var _yStart = 0;
			for (var x = _xStart; x < _xStart + _width; x++)
			{
				for (var y = _yStart; y < _yStart + _height; y++)
				{
					_values[x - _xStart, y - _yStart] = GetValueAt(x, y);
				}
			}
		}

		private float GetValueAt(float x, float y)
		{
			if (_sampler3D != null) return _sampler3D.GetSampleAt(x, y, 0);
			if (_positions != null)
			{
				foreach (var position in _positions)
				{
					if (Math.Floor(x).Equals(Math.Floor(position.x)) && Math.Floor(y).Equals(Math.Floor(position.y))) return 1;
				}
			}
			return float.NaN;
		}

		protected override void OnFinished()
		{
			if (Texture != null) Texture2D.DestroyImmediate(Texture);
			Texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
			Texture.SetPixels(NodeUtils.ToColorMap(_values, _samplerColor));
			Texture.Apply();
		}

	}
}

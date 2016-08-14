using Assets.Code.Bon.Interface;
using Assets.Code.Thread;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Number.Map2D
{
	public class TextureUpdateJob : ThreadedJob
	{

		private ISampler3D _sampler;
		private IColorSampler1D _colorSampler;
		private int _width;
		private int _height;
		private float[,] _values;

		public Texture2D Texture;

		public void Request(ISampler3D sampler, int width, int height, IColorSampler1D colorSampler = null)
		{
			_sampler = sampler;
			_width = width;
			_height = height;
			_values = new float[width, height];
			_colorSampler = colorSampler;
		}


		protected override void ThreadFunction()
		{
			var _xStart = 0;
			var _yStart = 0;
			for (var x = _xStart; x < _xStart + _width; x++)
			{
				for (var y = _yStart; y < _yStart + _height; y++)
				{
					float value = float.NaN;
					if (_sampler != null)
					{
						value = _sampler.GetSampleAt(x, y, 0);
					}
					_values[x - _xStart, y - _yStart] = value;
				}
			}
		}

		protected override void OnFinished()
		{
			if (Texture != null) Texture2D.DestroyImmediate(Texture);
			Texture = new Texture2D(_width, _height, TextureFormat.RGB24, false);
			Texture.SetPixels(NodeUtils.ToColorMap(_values, _colorSampler));
			Texture.Apply();
		}

	}
}

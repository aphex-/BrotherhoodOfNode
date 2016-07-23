using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Code.Thread;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Map
{
	public class MapDisplayUpdateJob : ThreadedJob
	{

		private Socket _socket;
		private IList<ISampler2D> _samplers;
		private int _width;
		private int _height;
		private int _xStart;
		private int _yStart;
		private float[,] _values;

		public Texture2D Texture;

		public void Request(Socket socket, IList<ISampler2D> samplers, int x, int y, int width, int height)
		{
			_socket = socket;
			_samplers = samplers;
			_width = width;
			_height = height;
			_values = new float[width, height];
		}

		protected override void ThreadFunction()
		{
			for (var x = _xStart; x < _xStart + _width; x++)
			{
				for (var y = _yStart; y < _yStart + _height; y++)
				{
					foreach (ISampler2D sampler in _samplers)
					{
						sampler.SampleFrom(x, y);
					}
					_values[x - _xStart, y - _yStart] = (float) _socket.GetConnectedSocket().Parent.GetResultOf(_socket.GetConnectedSocket());
				}
			}
		}

		protected override void OnFinished()
		{
			if (Texture != null) Texture2D.DestroyImmediate(Texture);
			Texture = new Texture2D(_width, _height, TextureFormat.RGB24, false);
			for (var x = 0; x < _width; x++)
			{
				for (var y = 0; y < _height; y++)
				{
					Texture.SetPixel(x, y, NodeUtils.GetMapValueColor(_values[x, y]));
				}
			}
			Texture.Apply();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Color
{
	[Serializable]
	[GraphContextMenuItem("Color", "Gradient")]
	public class GradientNode : AbstractColorNode
	{

		[SerializeField] private List<float> _times = new List<float>();

		[NonSerialized] private Rect _tmpRect;
		[NonSerialized] private Rect _addColorButton;

		[NonSerialized] private  List<InputSocket> _inputSockets;
		[NonSerialized] private Gradient _gradient;
		[NonSerialized] private Texture2D _previewTexture;
		[NonSerialized] private bool _needsUpdate = true;

		public GradientNode(int id, Graph parent) : base(id, parent)
		{
			_inputSockets = new List<InputSocket>();
			_tmpRect = new Rect();
			_addColorButton = new Rect();
			_gradient = new Gradient();
			Sockets.Add(new OutputSocket(this, typeof(AbstractColorNode)));
			Width = 156;
		}

		public override void OnDeserialization(SerializableNode sNode)
		{
			var aditionalSocketsCount = _times.Count;
			while (Sockets.Count <= aditionalSocketsCount) AddInputSocket(false);
		}

		private void AddInputSocket(bool addTimes)
		{
			InputSocket s = new InputSocket(this, typeof(AbstractColorNode));
			Sockets.Add(s);
			_inputSockets.Add(s);
			if (addTimes) _times.Add(0);
			_needsUpdate = true;
		}

		private void RemoveInputSocket(int i)
		{
			Sockets.Remove(_inputSockets[i]);
			_inputSockets.Remove(_inputSockets[i]);
			_times.RemoveAt(i);
			_needsUpdate = true;
		}

		public override void OnGUI()
		{
			Height = _times.Count * 20 + 45;

			_addColorButton.Set(28, Height - 42, 100, 18);
			if (GUI.Button(_addColorButton, "add color")) AddInputSocket(true);

			var socketToRemove = -1;

			for (var i = 0; i < _inputSockets.Count; i++)
			{
				_tmpRect.Set(25, 20 * i, 100, 20);

				var value = _times[i];
				_times[i] = GUI.HorizontalSlider(_tmpRect, value, 0f, 1f);
				if (Math.Abs(_times[i] - value) > 0.0001f) UpdateColorPreview();
				_tmpRect.Set(3, (20 * i) - 2, 18, 18);
				if (!_inputSockets[i].IsConnected()) if (GUI.Button(_tmpRect, "x")) socketToRemove = i;
			}

			if (_previewTexture != null)
			{
				GUI.DrawTexture(new Rect(Width - _previewTexture.width - 6, 0, _previewTexture.width, _previewTexture.height), _previewTexture);
			}

			if (socketToRemove != -1) RemoveInputSocket(socketToRemove);

			if (_needsUpdate) UpdateColorPreview();
		}

		public void UpdateColorPreview()
		{
			_needsUpdate = false;
			UpdateGradient();
			if (_previewTexture != null) Texture2D.DestroyImmediate(_previewTexture);
			_previewTexture = new Texture2D(20, (int) Height - 24);

			for (int y = 0; y < _previewTexture.height; y++)
			{
				UnityEngine.Color c = _gradient.Evaluate(y / (float) _previewTexture.height);
				for (int x = 0; x < _previewTexture.width; x++)
				{
					_previewTexture.SetPixel(x, y, c);
				}
			}
			_previewTexture.Apply();
		}

		private void UpdateGradient()
		{
			GradientColorKey[] colorKeys = new GradientColorKey[_inputSockets.Count];
			GradientAlphaKey[] alphaKeys = new GradientAlphaKey[_inputSockets.Count];
			for (int i = 0; i < _inputSockets.Count; i++)
			{
				UnityEngine.Color c = UnityEngine.Color.black;
				if (_inputSockets[i].IsConnected())
				{
					AbstractColorNode colorNode = (AbstractColorNode) _inputSockets[i].GetConnectedSocket().Parent;
					c = colorNode.GetColor(_inputSockets[i].GetConnectedSocket(), 0);
				}
				colorKeys[i] = new GradientColorKey();
				colorKeys[i].color = c;
				colorKeys[i].time = _times[i];
				alphaKeys[i] = new GradientAlphaKey();
				alphaKeys[i].alpha = c.a;
				alphaKeys[i].time = _times[i];
			}
			_gradient.SetKeys(colorKeys, alphaKeys);
		}

		public override void Update()
		{
			if (!Collapsed)
			UpdateColorPreview();
		}

		public override UnityEngine.Color GetColor(OutputSocket outSocket, float i)
		{
			return _gradient.Evaluate(i);
		}


	}
}

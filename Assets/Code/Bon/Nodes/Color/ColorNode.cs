using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Bon.Nodes.Color
{
	[Serializable]
	[GraphContextMenuItem("Color", "Color")]
	public class ColorNode : AbstractColorNode
	{
		[SerializeField] private UnityEngine.Color _color;

		[NonSerialized] private Socket _inputSocketR;
		[NonSerialized] private Socket _inputSocketG;
		[NonSerialized] private Socket _inputSocketB;
		[NonSerialized] private Socket _inputSocketA;

		[NonSerialized] private Rect _labelR;
		[NonSerialized] private Rect _labelG;
		[NonSerialized] private Rect _labelB;
		[NonSerialized] private Rect _labelA;

		[NonSerialized] private Rect _sliderR;
		[NonSerialized] private Rect _sliderG;
		[NonSerialized] private Rect _sliderB;
		[NonSerialized] private Rect _sliderA;

		public ColorNode(int id, Graph parent) : base(id, parent)
		{
			_labelR = new Rect(3, 0, 20, 20);
			_labelG = new Rect(3, 20, 20, 20);
			_labelB = new Rect(3, 40, 20, 20);
			_labelA = new Rect(3, 60, 20, 20);

			_sliderR = new Rect(23, 0, 50, 20);
			_sliderG = new Rect(23, 20, 50, 20);
			_sliderB = new Rect(23, 40, 50, 20);
			_sliderA = new Rect(23, 60, 50, 20);

			_inputSocketR = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputSocketG = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputSocketB = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputSocketA = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);

			Sockets.Add(_inputSocketR);
			Sockets.Add(_inputSocketG);
			Sockets.Add(_inputSocketB);
			Sockets.Add(_inputSocketA);

			Sockets.Add(new Socket(this, typeof(AbstractColorNode), SocketDirection.Output));
		}

		public override void OnGUI()
		{
			var r = _color.r;
			var g = _color.g;
			var b = _color.b;
			var a = _color.a;

			GUI.Label(_labelR, "r");
			GUI.Label(_labelG, "g");
			GUI.Label(_labelB, "b");
			GUI.Label(_labelA, "a");

			r = GUI.HorizontalSlider(_sliderR, r, 0f, 1f);
			g = GUI.HorizontalSlider(_sliderG, g, 0f, 1f);
			b = GUI.HorizontalSlider(_sliderB, b, 0f, 1f);
			a = GUI.HorizontalSlider(_sliderA, a, 0f, 1f);

			var rChanged = UpdateDirectInput(_inputSocketR, _color.r, r);
			var gChanged = UpdateDirectInput(_inputSocketG, _color.g, g);
			var bChanged = UpdateDirectInput(_inputSocketB, _color.b, b);
			var aChanged = UpdateDirectInput(_inputSocketA, _color.a, a);

			if (rChanged || gChanged || bChanged || aChanged)
			{
				SetColor(r, g, b, a);
				TriggerChangeEvent();
			}

			NodeUtils.GUIDrawRect(new Rect(77, 0, 20, 20), _color);
		}

		private bool UpdateDirectInput(Socket socket, float oldValue, float newValue)
		{
			if (oldValue != newValue && socket.IsInDirectInputMode())
			{
				socket.SetDirectInputNumber(newValue, false);
				return true;
			}
			return false;
		}

		private void SetColor(float r, float g, float b, float a)
		{
			_color.r = r;
			_color.g = g;
			_color.b = b;
			_color.a = a;
		}

		public override void Update()
		{
			_color.r = AbstractNumberNode.GetInputNumber(_inputSocketR, 0, 0, 0);
			_color.g = AbstractNumberNode.GetInputNumber(_inputSocketG, 0, 0, 0);
			_color.b = AbstractNumberNode.GetInputNumber(_inputSocketB, 0, 0, 0);
			_color.a = AbstractNumberNode.GetInputNumber(_inputSocketA, 0, 0, 0);
		}

		public override object GetResultOf(Socket outSocket)
		{
			return _color;
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return true;
		}

		public override UnityEngine.Color GetColorFrom(float x)
		{
			return _color;
		}


	}
}

using System;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Limit")]
	public class LimitNode : AbstractNumberNode {

		[SerializeField] private bool _minActive;
		[SerializeField] private bool _maxActive;

		[NonSerialized] private Socket _inputSocket01;
		[NonSerialized] private Socket _inputSocketMin;
		[NonSerialized] private Socket _inputSocketMax;

		[NonSerialized] private Rect _tmpRect;

		public LimitNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket01 = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_inputSocket01);
			_inputSocketMin = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_inputSocketMin);
			_inputSocketMax = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_inputSocketMax);

			_tmpRect = new Rect();
			Height = 80;
			Width = 50;
		}


		public override void OnGUI()
		{
			_tmpRect.Set(3, 20, 50, 20);
			var currentMin = GUI.Toggle(_tmpRect, _minActive, "min");
			_tmpRect.Set(3, 40, 50, 20);
			var currentMax = GUI.Toggle(_tmpRect, _maxActive, "max");

			if (currentMin != _minActive || currentMax != _maxActive) TriggerChangeEvent();

			_maxActive = currentMax;
			_minActive = currentMin;
		}

		public override void Update()
		{

		}

		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed);
		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			var value01 = GetInputNumber(_inputSocket01, x, y, seed);

			if (float.IsNaN(value01)) return float.NaN;

			if (_minActive)
			{
				var min = GetInputNumber(_inputSocketMin, x, y, seed);
				if (float.IsNaN(min)) return float.NaN;
				value01 = Math.Min(value01, min);
			}

			if (_maxActive)
			{
				var max = GetInputNumber(_inputSocketMax, x, y, seed);
				if (float.IsNaN(max)) return float.NaN;
				value01 = Math.Max(value01, max);
			}

			return value01;
		}

	}
}

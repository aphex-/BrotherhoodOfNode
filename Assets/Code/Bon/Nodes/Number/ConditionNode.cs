using System;
using UnityEngine;

/**
	This node avoids the evaluation of the graph branch that
	where the condition is not true. This can be used to improves performance.
**/
namespace Assets.Code.Bon.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Condition")]
	public class ConditionNode : AbstractNumberNode
	{

		[NonSerialized] private Rect _labelInput01;
		[NonSerialized] private Rect _labelInput02;
		[NonSerialized] private Rect _labelCondition;

		[NonSerialized] private Socket _inputSocket01;
		[NonSerialized] private Socket _inputSocket02;
		[NonSerialized] private Socket _conditionSocket;

		public ConditionNode(int id, Graph parent) : base(id, parent)
		{
			_labelInput01 = new Rect(3, 0, 100, 20);
			_labelInput02 = new Rect(3, 20, 100, 20);
			_labelCondition = new Rect(3, 40, 100, 20);

			_inputSocket01 = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputSocket02 = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_conditionSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_inputSocket01);
			Sockets.Add(_inputSocket02);
			Sockets.Add(_conditionSocket);
			Sockets.Add(_outSocket);
			Height = 80;
			Width = 80;
		}

		public override void OnGUI()
		{
			GUI.Label(_labelInput01, "if (x < 0.0)");
			GUI.Label(_labelInput02, "if (x >= 0.0)");
			GUI.Label(_labelCondition, "x");
		}

		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed);
		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			var conditionValue02 = GetInputNumber(_conditionSocket, x, y, seed);
			if (float.IsNaN(conditionValue02)) return float.NaN;
			if (conditionValue02 < 0.0f) return GetInputNumber(_inputSocket01, x, y, seed);
			return GetInputNumber(_inputSocket02, x, y, seed);
		}
	}
}

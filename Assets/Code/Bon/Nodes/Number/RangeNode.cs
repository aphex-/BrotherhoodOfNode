using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Nodes;
using UnityEngine;

[Serializable]
[GraphContextMenuItem("Number", "Range")]
public class RangeNode : AbstractNumberNode {


	[SerializeField] private int _selectedMode;

	[NonSerialized] private Socket _inputSocket01;
	[NonSerialized] public static string[] Modes = {"[-1:1] to [0:1]", "[0:1] to [-1:1]"};

	public RangeNode(int id, Graph parent) : base(id, parent)
	{
		_inputSocket01 = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
		Sockets.Add(_inputSocket01);
		Height = 60;
		Width = 100;
	}

	public override void OnGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		int newMode = GUILayout.SelectionGrid(_selectedMode, Modes, 1, "toggle");
		if (newMode != _selectedMode)
		{
			_selectedMode = newMode;
			TriggerChangeEvent();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	public override object GetResultOf(Socket outSocket)
	{
		return GetSampleAt(_x, _y, _seed);
	}

	public override float GetSampleAt(float x, float y, float seed)
	{
		float value = GetInputNumber(_inputSocket01, x, y, seed);
		if (float.IsNaN(value)) return float.NaN;

		if (_selectedMode == 0) return (value + 1f) / 2f;
		return value * 2f - 1f;
	}
}

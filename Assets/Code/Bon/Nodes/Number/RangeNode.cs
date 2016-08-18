using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Socket;
using UnityEngine;

[Serializable]
[GraphContextMenuItem("Number", "Range")]
public class RangeNode : AbstractNumberNode {


	[SerializeField] private int _selectedMode;

	[NonSerialized] private InputSocket _inputSocket01;
	[NonSerialized] public static string[] Modes = {"[-1:1] to [0:1]", "[0:1] to [-1:1]"};

	public RangeNode(int id, Graph parent) : base(id, parent)
	{
		_inputSocket01 = new InputSocket(this, typeof(AbstractNumberNode));
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

	public override void Update()
	{

	}


	public override float GetNumber(OutputSocket outSocket, float x, float y, float z, float seed)
	{
		float value = GetInputNumber(_inputSocket01, x, y, z, seed);
		if (float.IsNaN(value)) return float.NaN;

		if (_selectedMode == 0) return (value + 1f) / 2f;
		return value * 2f - 1f;
	}
}

using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes;
using UnityEngine;

[Serializable]
[GraphContextMenuItem("Number/Map2D", "Sine")]
public class SineMapNode : AbstractMap2DNode, IUpdateable {


	[NonSerialized] private Rect _textLabelScaleX;
	[NonSerialized] private Rect _textLabelScaleY;

	[NonSerialized] private Socket _socketInputX;
	[NonSerialized] private Socket _socketInputY;

	public SineMapNode(int id, Graph parent) : base(id, parent)
	{
		_textLabelScaleX = new Rect(6, 110, 65, BonConfig.SocketSize);
		_textLabelScaleY = new Rect(6, 130, 65, BonConfig.SocketSize);
		_socketInputX = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
		Sockets.Add(_socketInputX);
		_socketInputY = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
		Sockets.Add(_socketInputY);

		SocketTopOffsetInput = 110;
	}

	public override void OnGUI()
	{
		Height = CurrentTextureSize + 70;
		DrawTexture();
		GUI.Label(_textLabelScaleX, "scale x");
		GUI.Label(_textLabelScaleY, "scale y");
	}

	public override object GetResultOf(Socket outSocket)
	{
		return GetSampleAt(_x, _y, _seed);
	}

	public override float GetSampleAt(float x, float y, float seed)
	{
		var scaleX = GetInputNumber(_socketInputX, x, y, seed);
		var scaleY = GetInputNumber(_socketInputY, x, y, seed);

		if (float.IsNaN(scaleX) || float.IsNaN(scaleY)) return float.NaN;

		if (scaleX == 0) scaleX = 1;
		if (scaleY == 0) scaleY = 1;

		return (float) (Math.Sin(x / scaleX + seed) + Math.Sin(y / scaleY + seed)) / 2f;
	}

	protected override bool CanCreatePreview()
	{
		return true;
	}

	protected override IColorSampler1D GetColorSampler()
	{
		return null;
	}

	public void Update()
	{
		StartTextureUpdateJob();
	}
}

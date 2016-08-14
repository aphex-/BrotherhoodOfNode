using System;
using Assets.Code.Bon;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Nodes.Number.Map2D;
using UnityEngine;

public abstract class AbstractMap2DNode : AbstractNumberNode {

	[NonSerialized] protected Rect _textureArea;
	[NonSerialized] protected int CurrentTextureSize = 100;
	[NonSerialized] protected bool _isUpdatingTexture;
	[NonSerialized] private TextureUpdateJob _job;
	[NonSerialized] private Texture2D _texture;
	[NonSerialized] private Rect _errorMessageLabel;
	[NonSerialized] private bool _initialUpdate;

	protected AbstractMap2DNode(int id, Graph parent) : base(id, parent)
	{
		_textureArea = new Rect();
		_errorMessageLabel = new Rect(3, 0, 100, 15);
		_job = new TextureUpdateJob();
	}


	protected void DrawTexture()
	{
		if (!_initialUpdate) StartTextureUpdateJob();

		_isUpdatingTexture = UpdateTextureJob();
		if (_texture != null && !_isUpdatingTexture) GUI.DrawTexture(_textureArea, _texture);

		if (!CanCreatePreview()) GUI.Label(_errorMessageLabel, NodeUtils.NotConnectedMessage);
		else if (_isUpdatingTexture) GUI.Label(_errorMessageLabel, "updating data..");
		Width = CurrentTextureSize + 12;
	}

	protected void StartTextureUpdateJob()
	{
		_initialUpdate = true;
		if (_texture == null) CreateTexture();
		if (_job != null && !_job.IsDone) _job.Abort();
		_job = new TextureUpdateJob();
		_job.Request(this, CurrentTextureSize, CurrentTextureSize, GetColorSampler());
		_job.Start();
	}

	private bool UpdateTextureJob()
	{
		if (_job == null) return false;
		_job.Update();

		if (!CanCreatePreview())
		{
			_job.Abort();
			_texture = null;
			_job = null;
			return false;
		}

		if (!_job.IsDone) return true;
		_texture = _job.Texture;
		_job.Abort();
		_job = null;
		return false;
	}

	protected void CreateTexture()
	{
		if (_texture != null) Texture2D.DestroyImmediate(_texture);
		_texture = new Texture2D(CurrentTextureSize, CurrentTextureSize, TextureFormat.RGB24, false);
		_textureArea.Set(6, 0, _texture.width, _texture.height);
	}

	protected abstract bool CanCreatePreview();

	protected abstract IColorSampler1D GetColorSampler();
}

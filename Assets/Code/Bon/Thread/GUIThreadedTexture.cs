using System;
using System.Collections.Generic;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes.Noise;
using UnityEngine;
using UnityEngineInternal;

public class GUIThreadedTexture {

	private Rect _textureArea;
	private bool _isUpdatingTexture;
	private TextureUpdateJob _job;
	private Texture2D _texture;
	private bool _initialUpdate;

	public bool IsUpdating
	{
		get { return _isUpdatingTexture; }
	}

	public bool DoneInitialUpdate
	{
		get { return _initialUpdate; }
	}

	public float X
	{
		get { return _textureArea.x; }
		set { _textureArea.x = value; }
	}

	public float Y
	{
		get { return _textureArea.y; }
		set { _textureArea.y = value; }
	}

	public float Width
	{
		get { return _textureArea.width; }
	}

	public float Height
	{
		get { return _textureArea.height; }
	}

	public GUIThreadedTexture()
	{
		_textureArea = new Rect(6, 0, 0, 0);
		_job = new TextureUpdateJob();
	}

	public void OnGUI()
	{
		if (!DoneInitialUpdate) return;
		_isUpdatingTexture = UpdateTextureJob();
		if (_texture != null && !_isUpdatingTexture) GUI.DrawTexture(_textureArea, _texture);
	}

	public void StartTextureUpdateJob(int width, int height, ISampler3D sampler3D, IColorSampler1D samplerColor)
	{
		InitJob(width, height);
		_job.Request(width, height, sampler3D, samplerColor);
		_job.Start();
	}

	public void StartTextureUpdateJob(int width, int height, List<Vector3> vectors)
	{
		InitJob(width, height);
		_job.Request(width, height, vectors);
		_job.Start();
	}

	private void InitJob(int width, int height)
	{
		_textureArea.Set(_textureArea.x, _textureArea.y, width, height);
		_job = new TextureUpdateJob();
		_initialUpdate = true;
		if (_texture == null) CreateTexture(width, height);
		if (_job != null && !_job.IsDone) _job.Abort();
	}

	private bool UpdateTextureJob()
	{
		if (_job == null) return false;
		_job.Update();
		/*if (!CanCreatePreview())
		{

		}*/

		if (!_job.IsDone) return true;
		_texture = _job.Texture;
		_job.Abort();
		_job = null;
		return false;
	}

	private void CreateTexture(int width, int height)
	{
		if (_texture != null) Texture2D.DestroyImmediate(_texture);
		_texture = new Texture2D(width, height, TextureFormat.RGB24, false);
		_textureArea.Set(_textureArea.x, _textureArea.y, width, height);
	}

	public void Hide()
	{
		if (_job != null) _job.Abort();
		if (_texture != null) Texture2D.DestroyImmediate(_texture);
		_job = null;
	}
}

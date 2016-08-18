using System;
using System.Collections.Generic;
using Assets.Code.Bon.Socket;
using UnityEngine;


namespace Assets.Code.Bon.Nodes.Vector3
{
	[Serializable]
	[GraphContextMenuItem("Vector", "Random Offset")]
	public class RamdomOffsetNode : AbstractVector3Node
	{

		[SerializeField] private bool _offsetX = true;
		[SerializeField] private bool _offsetY = true;
		[SerializeField] private bool _offsetZ = true;

		private InputSocket _inputSocketVec;
		private InputSocket _inputSocketOffset;
		private InputSocket _inputSocketNoise;

		private Rect _tmpRect;

		public RamdomOffsetNode(int id, Graph parent) : base(id, parent)
		{

			_inputSocketVec = new InputSocket(this, typeof(AbstractVector3Node));
			_inputSocketOffset = new InputSocket(this, typeof(AbstractNumberNode));
			_inputSocketNoise = new InputSocket(this, typeof(AbstractNumberNode));

			Sockets.Add(_inputSocketVec);
			Sockets.Add(_inputSocketOffset);
			Sockets.Add(_inputSocketNoise);

			Sockets.Add(new OutputSocket(this, typeof(AbstractVector3Node)));
			Width = 90;
			Height = 80;
		}

		public override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "vec");
			_tmpRect.Set(3, 20, 50, 20);
			GUI.Label(_tmpRect, "offset");
			_tmpRect.Set(3, 40, 50, 20);
			GUI.Label(_tmpRect, "noise");

			_tmpRect.Set(50, 0, 50, 20);
			var currentOffsetX = GUI.Toggle(_tmpRect, _offsetX, "x");
			_tmpRect.Set(50, 20, 50, 20);
			var currentOffsetY = GUI.Toggle(_tmpRect, _offsetY, "y");
			_tmpRect.Set(50, 40, 50, 20);
			var currentOffsetZ = GUI.Toggle(_tmpRect, _offsetZ, "z");

			bool needsUpdate = currentOffsetX != _offsetX || currentOffsetY != _offsetY || currentOffsetZ != _offsetZ;
			_offsetX = currentOffsetX;
			_offsetY = currentOffsetY;
			_offsetZ = currentOffsetZ;
			if (needsUpdate) TriggerChangeEvent();

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override List<UnityEngine.Vector3> GetVector3List(OutputSocket socket, float x, float y, float z, float sizeX, float sizeY, float sizeZ, float seed)
		{
			List<UnityEngine.Vector3> vec = GetInputVector3List(_inputSocketVec, x, y, z, sizeX, sizeY, sizeZ, seed);
			if (vec == null) return null;

			for (var i = 0; i < vec.Count; i++)
			{
				UnityEngine.Vector3 v = vec[i];

				float noise = AbstractNumberNode.GetInputNumber(_inputSocketNoise, v.x, v.y, v.z, seed);
				float offset = AbstractNumberNode.GetInputNumber(_inputSocketOffset, v.x, v.y, v.z, seed);

				if (float.IsNaN(noise) || float.IsNaN(offset))	continue;

				if (_offsetX) v.x += offset * noise;
				if (_offsetY) v.y += offset * noise;
				if (_offsetZ) v.z += offset * noise;

				vec[i] = v;
			}

			return vec;
		}
	}
}

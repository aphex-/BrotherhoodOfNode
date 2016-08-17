using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Vector3
{
	[Serializable]
	[GraphContextMenuItem("Vector", "Operator")]
	public class OperatorNode : AbstractVector3Node
	{

		[SerializeField] private int _selectedMode;
		[NonSerialized] public static string[] Operations = {"add", "sub", "mul", "div"};

		private Socket _inputSocketVectors;
		private Socket _inputSocketX;
		private Socket _inputSocketY;
		private Socket _inputSocketZ;

		private Rect _tmpRect;

		public OperatorNode(int id, Graph parent) : base(id, parent)
		{
			_tmpRect = new Rect();
			_inputSocketVectors = new Socket(this, typeof(AbstractVector3Node), SocketDirection.Input);

			_inputSocketX = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputSocketY = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputSocketZ = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);

			Sockets.Add(_inputSocketVectors);
			Sockets.Add(_inputSocketX);
			Sockets.Add(_inputSocketY);
			Sockets.Add(_inputSocketZ);

			Sockets.Add(new Socket(this, typeof(AbstractVector3Node), SocketDirection.Output));
			Width = 100;
		}

		public override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 60, 20);
			GUI.Label(_tmpRect, "vec");

			_tmpRect.Set(3, 20, 60, 20);
			GUI.Label(_tmpRect, "x");

			_tmpRect.Set(3, 40, 60, 20);
			GUI.Label(_tmpRect, "y");

			_tmpRect.Set(3, 60, 60, 20);
			GUI.Label(_tmpRect, "z");

			_tmpRect.Set(50, 0, 40, 80);
			int newMode = GUI.SelectionGrid(_tmpRect, _selectedMode, Operations, 1, "toggle");
			if (newMode != _selectedMode)
			{
				_selectedMode = newMode;
				TriggerChangeEvent();
			}

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override object GetResultOf(Socket outSocket)
		{
			return GetVector3List(_x, _y, _z, _width, _height, _depth, _seed);
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return true;
		}

		public override List<UnityEngine.Vector3> GetVector3List(float x, float y, float z, float width, float height, float depth, float seed)
		{
			List<UnityEngine.Vector3> vectors = GetInputVector3List(_inputSocketVectors, x, y, z, width, height, depth, seed);
			if (vectors != null)
			{
				for (var i = 0; i < vectors.Count; i++)
				{
					if (_selectedMode == 0)
					{
						UnityEngine.Vector3 v = vectors[i];
						v.x += v.x + AbstractNumberNode.GetInputNumber(_inputSocketX, v.x, v.y, v.z);
						v.y += v.y + AbstractNumberNode.GetInputNumber(_inputSocketY, v.x, v.y, v.z);
						v.z += v.z + AbstractNumberNode.GetInputNumber(_inputSocketZ, v.x, v.y, v.z);
						vectors[i] = v;
					}
				}
			}
			return vectors;
		}
	}
}

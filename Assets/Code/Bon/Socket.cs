using System;
using Assets.Code.Bon.Nodes;
using UnityEngine;

namespace Assets.Code.Bon
{

	public class Socket
	{
		private string _directInputString = "0";

		public Edge Edge;
		public Type Type;
		public Node Parent;

		public SocketDirection Direction;

		private Rect _boxRect;
		private Rect _directInputRect;
		private float _directInputNumber = float.NaN;
		private RectOffset _padding;

		public Socket(Node parent, Type type, SocketDirection direciton)
		{
			_padding = new RectOffset(0, 0, -2, 0);
			Parent = parent;
			Type = type;
			_boxRect.width = BonConfig.SocketSize;
			_boxRect.height = BonConfig.SocketSize;
			Direction = direciton;
		}

		/// The x position of the node
		public float X
		{
			get { return _boxRect.x; }
			set { _boxRect.x = value; }
		}

		/// The y position of the node
		public float Y
		{
			get { return _boxRect.y; }
			set { _boxRect.y = value; }
		}

		public bool IsConnected()
		{
			return Edge != null;
		}

		public Socket GetConnectedSocket()
		{
			if (Edge == null)
			{
				return null;
			}
			if (Direction == SocketDirection.Input)
			{
				return Edge.Output;
			}
			return Edge.Input;
		}

		public bool CanGetResult()
		{
			if (IsInDirectInputMode()) return true;
			return Direction == SocketDirection.Input && IsConnected() &&
			       GetConnectedSocket().Parent.CanGetResultOf(GetConnectedSocket());
		}

		public bool Intersects(Vector2 nodePosition)
		{
			if (Parent.Collapsed) return false;

			if (IsInDirectInputMode())
			{
				float width = CalcDirectInputOffset();
				_boxRect.x -= width;
				var intersects = _boxRect.Contains(nodePosition);
				_boxRect.x += width;
				return intersects;
			}
			return _boxRect.Contains(nodePosition);
		}

		public void Draw()
		{
			GUI.skin.box.normal.textColor = Node.GetEdgeColor(Type);
			GUI.skin.box.padding = _padding;
			GUI.skin.box.fontSize = 14;
			GUI.skin.box.fontStyle = FontStyle.Bold;
			if (IsInDirectInputMode()) DrawDirectNumberInput();
			else GUI.Box(_boxRect, ">");
		}

		public bool IsInDirectInputMode()
		{
			return Type == typeof(AbstractNumberNode) && Direction == SocketDirection.Input && Edge == null;
		}

		private float CalcDirectInputOffset()
		{
			return GUI.skin.textField.CalcSize(new GUIContent(_directInputString)).x + 5;
		}

		private void DrawDirectNumberInput()
		{
			float width = CalcDirectInputOffset();
			_boxRect.x -= width;
			GUI.Box(_boxRect, ">");
			_directInputRect.Set(_boxRect.x + _boxRect.width, _boxRect.y, width, _boxRect.height);
			if (NodeUtils.FloatTextField(_directInputRect, ref _directInputString))
			{
				_directInputNumber = float.Parse(_directInputString);
				Parent.TriggerChangeEvent();
			}
			_boxRect.x += width;
		}

		public float GetDirectInputNumber()
		{
			if (float.IsNaN(_directInputNumber)) _directInputNumber = float.Parse(_directInputString);
			return _directInputNumber;
		}

		public void SetDirectInputNumber(float number, bool triggerChangeEvent)
		{
			if (!float.IsNaN(number))
			{
				_directInputNumber = number;
				_directInputString = number + "";
				if (triggerChangeEvent) Parent.TriggerChangeEvent();
			}
		}

	}

	public enum SocketDirection
	{
		Input,
		Output
	}
}



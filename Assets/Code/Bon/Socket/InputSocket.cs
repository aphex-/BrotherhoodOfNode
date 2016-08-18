using System;
using Assets.Code.Bon.Nodes;
using UnityEngine;

namespace Assets.Code.Bon.Socket
{
	public class InputSocket : AbstractSocket {

		public Edge Edge;

		private Rect _directInputRect;
		private float _directInputNumber = float.NaN;
		private string _directInputString = "0";

		public InputSocket(Node parent, Type type) : base(parent, type)
		{
		}

		public bool CanGetResult()
		{
			if (IsInDirectInputMode()) return true;
			return IsInput() && IsConnected();
		}

		public bool IsInDirectInputMode()
		{
			return Type == typeof(AbstractNumberNode) && IsInput() && Edge == null;
		}

		private float CalcDirectInputOffset()
		{
			return GUI.skin.textField.CalcSize(new GUIContent(_directInputString)).x + 5;
		}

		private void DrawDirectNumberInput()
		{
			float width = CalcDirectInputOffset();
			BoxRect.x -= width;
			GUI.Box(BoxRect, ">");
			_directInputRect.Set(BoxRect.x + BoxRect.width, BoxRect.y, width, BoxRect.height);
			if (NodeUtils.FloatTextField(_directInputRect, ref _directInputString))
			{
				_directInputNumber = float.Parse(_directInputString);
				Parent.TriggerChangeEvent();
			}
			BoxRect.x += width;
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

		public override bool IsConnected()
		{
			return Edge != null;
		}

		public override bool Intersects(Vector2 nodePosition)
		{
			if (Parent.Collapsed) return false;

			if (IsInDirectInputMode())
			{
				float width = CalcDirectInputOffset();
				BoxRect.x -= width;
				var intersects = BoxRect.Contains(nodePosition);
				BoxRect.x += width;
				return intersects;
			}
			return BoxRect.Contains(nodePosition);
		}

		protected override void OnDraw()
		{
			if (IsInDirectInputMode()) DrawDirectNumberInput();
			else GUI.Box(BoxRect, ">");
		}

		public OutputSocket GetConnectedSocket()
		{
			return Edge.Output;
		}
	}
}

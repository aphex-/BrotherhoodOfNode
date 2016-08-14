using System;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.String
{
	[Serializable]
	[GraphContextMenuItem("String", "String")]
	public class StringNode : AbstractStringNode {

		[SerializeField] private string _text = "";
		[NonSerialized] private Rect _textFieldRect;

		public StringNode(int id, Graph parent) : base(id, parent)
		{
			_textFieldRect = new Rect(3, 0, 100, 20);
			Sockets.Add(new Socket(this, typeof(AbstractStringNode), SocketDirection.Output));
			Height = 45;
			Width = 108;
		}

		public override void OnGUI()
		{
			string newText = GUI.TextField(_textFieldRect, _text);
			if (!newText.Equals(_text)) TriggerChangeEvent();
			_text = newText;
		}

		public override object GetResultOf(Socket outSocket)
		{
			return _text;
		}

		public override bool CanGetResultOf(Socket outSocket)
		{
			return true;
		}

		public override string GetString()
		{
			return _text;
		}
	}
}

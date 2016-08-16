using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Noise
{
	public abstract class AbstractNoiseNode : AbstractNumberNode
	{
		protected List<GUIThreadedTexture> _textures;
		private Rect _errorMessageLabel;

		protected AbstractNoiseNode(int id, Graph parent) : base(id, parent)
		{
			_errorMessageLabel = new Rect(3, 0, 100, 15);
			_textures = new List<GUIThreadedTexture>();
		}


		protected void DrawTextures()
		{

			for (var i = 0; i < _textures.Count; i++) _textures[i].OnGUI();
			//if (!CanCreatePreview()) GUI.Label(_errorMessageLabel, NodeUtils.NotConnectedMessage);


			if (IsUpdatingTexture()) GUI.Label(_errorMessageLabel, "updating data..");
		}

		protected bool IsUpdatingTexture()
		{
			foreach (GUIThreadedTexture t in _textures) if (t.IsUpdating) return true;
			return false;
		}

	}
}

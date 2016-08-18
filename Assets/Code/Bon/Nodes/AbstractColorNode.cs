using System;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Socket;

namespace Assets.Code.Bon.Nodes
{
	public abstract class AbstractColorNode : Node, IColorSampler
	{

		protected AbstractColorNode(int id, Graph parent) : base(id, parent)
		{

		}

		public abstract UnityEngine.Color GetColor(OutputSocket socket, float i);
	}
}

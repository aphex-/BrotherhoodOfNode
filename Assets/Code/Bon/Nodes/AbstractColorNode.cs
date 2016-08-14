using System;
using Assets.Code.Bon.Interface;

namespace Assets.Code.Bon.Nodes
{
	public abstract class AbstractColorNode : Node, IColorSampler1D
	{

		[NonSerialized] protected float _i;

		protected AbstractColorNode(int id, Graph parent) : base(id, parent)
		{

		}

		public void SetPosition(float i)
		{
			_i = i;
		}

		public abstract UnityEngine.Color GetColorFrom(float i);
	}
}

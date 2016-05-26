using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Bon.Graph
{
	[Serializable]
	public class Graph : ISerializationCallbackReceiver
	{
		public List<Node> nodes;
		public String id;

		/// <summary>Unity serialization callback.</summary>
		public void OnBeforeSerialize()
		{
			Debug.Log("OnBefore");
		}

		/// <summary>Unity serialization callback.</summary>
		public void OnAfterDeserialize()
		{
			Debug.Log("OnAfter");
		}

	}
}
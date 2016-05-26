using System;
using System.Collections.Generic;
using Assets.Code.Bon.Graph.Custom;
using UnityEngine;

namespace Assets.Code.Bon.Graph
{
	[Serializable]
	public class Graph : ISerializationCallbackReceiver
	{
		public List<Node> nodes = new List<Node>();

		private int _uniqueIdOffset = 0;

		[SerializeField]
		public String id;

		[SerializeField]
		private List<SerializableEdge> serializedEdges = new List<SerializableEdge>();

		[SerializeField]
		private List<SerializableNode> serializedNodes = new List<SerializableNode>();


		public int GetUniqueId()
		{
			_uniqueIdOffset += 1;
			return _uniqueIdOffset;
		}

		public Node CreateNode(Type nodeType)
		{
			return CreateNode(nodeType, GetUniqueId());
		}

		public Node CreateNode(Type nodeType, int id)
		{
			if (nodeType == typeof(Multiplexer)) return new Multiplexer(id);
			if (nodeType == typeof(SamplerNode)) return new SamplerNode(id);
			return null;
		}

		/// <summary>Unity serialization callback.</summary>
		public void OnBeforeSerialize()
		{
			Debug.Log("OnBefore");
			if (nodes.Count == 0) return;

			serializedEdges.Clear();
			serializedNodes.Clear();
			Debug.Log("OnBefore Clear");

			// serialize edges

			Debug.Log("Node Count " + nodes.Count);

			foreach (var node in nodes)
			{
				serializedNodes.Add(node.ToSerializedNode());
				foreach (var socket in node.Sockets)
				{
					if (socket.Edge != null)
					{
						// serialize only the edge of the source (to avoid doubled edges)
						if (socket.Edge.Source.Parent == node)
						{
							serializedEdges.Add(socket.Edge.ToSerializedEgde());
						}
					}

				}
			}
		}

		/// <summary>Unity serialization callback.</summary>
		public void OnAfterDeserialize()
		{
			Debug.Log("OnAfter");
			if (nodes.Count > 0) return;

			//Debug.Log("OnAfter nodes.Count: " + nodes.Count + " serializedNodes.Count: " + serializedNodes.Count);


			foreach (var sNode in serializedNodes)
			{

				Debug.Log("Type " + sNode.type);
				Node n = CreateNode(sNode.type, sNode.id);
				if (n != null)
				{
					JsonUtility.FromJsonOverwrite(sNode.data, n);
					Debug.Log("Created " + n);
					nodes.Add(n);
				}
			}
		}
	}
}
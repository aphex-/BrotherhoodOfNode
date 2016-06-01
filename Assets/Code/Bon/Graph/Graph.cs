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

		public Node GetNode(int nodeId)
		{
			if (nodes == null) return null;
			foreach (var node in nodes)
			{
				if (node.Id == nodeId)
				{
					return node;
				}
			}
			return null;
		}

		/// <summary>Unity serialization callback.</summary>
		public void OnBeforeSerialize()
		{
			if (nodes.Count == 0) return; // nothing to serialize
			serializedEdges.Clear();
			serializedNodes.Clear();
			// serialize data
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
			if (serializedNodes.Count == 0) return;	// Nothing to deserialize.
			nodes.Clear(); // clear original data.

			// deserialize nodes
			foreach (var sNode in serializedNodes)
			{
				Node n = CreateNode(Type.GetType(sNode.type), sNode.id);
				if (n != null)
				{
					JsonUtility.FromJsonOverwrite(sNode.data, n);
					n.X = sNode.X;
					n.Y = sNode.Y;
					nodes.Add(n);
				}
			}

			// deserialize edges
			foreach (var sEdge in serializedEdges)
			{
				Node sourceNode = GetNode(sEdge.sourceNodeId);
				Node sinkNode = GetNode(sEdge.sinkNodeId);
				if (sourceNode == null || sinkNode == null)
				{
					Debug.LogWarning("Try to create an edge but can not find at least on of the nodes.");
					continue;
				}

				if (sEdge.sourceSocketIndex > sourceNode.Sockets.Count || sEdge.sinkSocketIndex > sinkNode.Sockets.Count)
				{
					Debug.LogWarning("Try to create an edge but can not find at least on of the sockets.");
					continue;
				}
				Edge edge = new Edge(sourceNode.Sockets[sEdge.sourceSocketIndex], sinkNode.Sockets[sEdge.sinkSocketIndex]);
				sourceNode.Sockets[sEdge.sourceSocketIndex].Edge = edge;
				sinkNode.Sockets[sEdge.sinkSocketIndex].Edge = edge;
			}
		}
	}
}
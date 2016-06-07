using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using Assets.Code.Bon.Graph.Custom;
using UnityEngine;

namespace Assets.Code.Bon.Graph
{
	[Serializable]
	public class Graph : ISerializationCallbackReceiver
	{
		public List<Node> nodes = new List<Node>();

		[SerializeField]
		private List<SerializableEdge> serializedEdges = new List<SerializableEdge>();

		[SerializeField]
		private List<SerializableNode> serializedNodes = new List<SerializableNode>();

		private IGraphListener listener;

		public void RegisterListener(IGraphListener listener)
		{
			this.listener = listener;
		}

		public int GetUniqueId()
		{
			var tmpId = 0;
			while (GetNode(tmpId) != null)
			{
				tmpId++;
			}
			return tmpId;
		}

		public object CreateNode(Type nodeType)
		{
			return CreateNode(nodeType, GetUniqueId());
		}

		public Node CreateNode(Type nodeType, int id)
		{
			try
			{
				return (Node) Activator.CreateInstance(nodeType, id); ;
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Node {0} could not be created", nodeType.FullName);
			}
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


		public void RemoveNode(Node node)
		{
			if (node == null) return;

			foreach (var socket in node.Sockets)
			{
				if (socket.Edge != null)
				{
					UnLink(socket);
				}
			}

			nodes.Remove(node);
			if (listener != null)
			{
				listener.OnNodeRemoved(node);
			}

		}

		public void UnLink(Socket s01, Socket s02)
		{
			if (listener != null)
			{
				listener.OnUnLink(s01, s02);
			}

			if (s01 != null && s01.Edge != null)
			{
				s01.Edge.Sink = null;
				s01.Edge.Source = null;
				s01.Edge = null;
			}
			if (s02 != null && s02.Edge != null)
			{
				s02.Edge.Sink = null;
				s02.Edge.Source = null;
				s02.Edge = null;
			}
		}

		public void UnLink(Socket socket)
		{
			Socket socket2 = null;
			if (socket.Edge != null)
			{
				socket2 = socket.Edge.GetOtherSocket(socket);
			}
			UnLink(socket, socket2);
		}

		public bool Link(Socket ownSocket, Socket foreignSocket)
		{
			if (ownSocket == null || foreignSocket == null)
			{
				Debug.LogWarning("Try to link sockets but at least one socket does not exist.");
				return false;
			}
			if (ownSocket.Type == foreignSocket.Type)
			{
				Edge edge = new Edge(ownSocket, foreignSocket);
				ownSocket.Edge = edge;
				foreignSocket.Edge = edge;
				if (listener != null)
				{
					listener.OnLink(edge);
				}
			}
			return true;
		}

		/// <summary> Returns true if the sockets can be linked.</summary>
		/// <param name="socket01"> The first socket</param>
		/// <param name="socket02"> The second socket</param>
		/// <returns>True if the sockets can be linked.</returns>
		public bool CanBeLinked(Socket socket01, Socket socket02)
		{
			return socket02 != null && socket01 != null && socket01.Type == socket02.Type
				   && socket01 != socket02;
		}


		// === SERIALIZATION ===

		public string ToJson()
		{
			return JsonUtility.ToJson(this);
		}

		public static Graph FromJson(string json)
		{
			return JsonUtility.FromJson<Graph>(json);
		}

		public static bool Save(string fileName, Graph graph)
		{
			Debug.Log("Save " + fileName);
			var file = File.CreateText(fileName);
			file.Write(graph.ToJson());
			file.Close();
			return true;
		}

		public static Graph Load(string fileName)
		{
			if(File.Exists(fileName)){
				var file = File.OpenText(fileName);
				var json = file.ReadToEnd();
				file.Close();
				return Graph.FromJson(json);
			} else {
				Debug.Log("Could not Open the file: " + fileName);
				return null;
			}
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
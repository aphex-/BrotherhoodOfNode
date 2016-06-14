using System;
using System.Collections.Generic;
using System.IO;
using Assets.Code.Bon.Interface;
using UnityEngine;

namespace Assets.Code.Bon
{

	[Serializable]
	public class Graph : ISerializationCallbackReceiver
	{
		private List<Node> nodes = new List<Node>();

		[SerializeField]
		private List<SerializableEdge> serializedEdges = new List<SerializableEdge>();

		[SerializeField]
		private List<SerializableNode> serializedNodes = new List<SerializableNode>();

		[SerializeField] private int version = BonConfig.Version;

		private IGraphListener listener;

		public bool TriggerEvents = true;

		public void RegisterListener(IGraphListener listener)
		{
			this.listener = listener;
			foreach (var node in nodes)
			{
				node.RegisterListener(listener);
			}
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

		public Node CreateNode(Type nodeType)
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
				Debug.LogErrorFormat("Node {0} could not be created " + exception.HelpLink, nodeType.FullName);
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

		public int GetNodeCount()
		{
			return nodes.Count;
		}

		public Node GetNodeAt(int index)
		{

			return nodes[index];
		}

		public void AddNode(Node node)
		{
			nodes.Add(node);
			if (listener != null && TriggerEvents)
			{
				node.RegisterListener(listener);
				listener.OnNodeAdded(node);
			}
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
			if (listener != null && TriggerEvents)
			{
				listener.OnNodeRemoved(node);
			}
			node.RegisterListener(null);
		}

		public void RemoveNode(int id)
		{
			RemoveNode(GetNode(id));
		}

		public void UnLink(Socket s01, Socket s02)
		{
			if (listener != null && TriggerEvents)
			{
				listener.OnUnLink(s01, s02);
			}
			if (s01 != null && s01.Edge != null)
			{
				s01.Edge.Input = null;
				s01.Edge.Output = null;
				s01.Edge = null;
			}
			if (s02 != null && s02.Edge != null)
			{
				s02.Edge.Input = null;
				s02.Edge.Output = null;
				s02.Edge = null;
			}
			if (listener != null && TriggerEvents)
			{
				listener.OnUnLinked(s01, s02);
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

		public bool Link(Socket inputSocket, Socket sourceSocket)
		{
			if (!CanBeLinked(inputSocket, sourceSocket))
			{
				Debug.LogWarning("Sockets can not be linked.");
			}

			if (inputSocket.Type == sourceSocket.Type)
			{
				Edge edge = new Edge(inputSocket, sourceSocket);
				inputSocket.Edge = edge;
				sourceSocket.Edge = edge;
				if (listener != null && TriggerEvents)
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
				   && socket01 != socket02 && socket01.Direction != socket02.Direction;
		}


		// === SERIALIZATION ===

		public string ToJson()
		{
			return JsonUtility.ToJson(this);
		}

		public static Graph FromJson(string json, IGraphListener listener)
		{
			Graph g = JsonUtility.FromJson<Graph>(json);
			listener.OnCreate(g);
			g.RegisterListener(listener);
			return g;
		}

		public static bool Save(string fileName, Graph graph)
		{
			var file = File.CreateText(fileName);
			file.Write((string) graph.ToJson());
			file.Close();
			return true;
		}

		public static Graph Load(string fileName, IGraphListener listener)
		{
			if(File.Exists(fileName)){
				var file = File.OpenText(fileName);
				var json = file.ReadToEnd();
				file.Close();
				return FromJson(json, listener);
			} else {
				Debug.Log("Could not Open the file: " + fileName);
				return null;
			}
		}

		/// <summary>Unity serialization callback.</summary>
		public void OnBeforeSerialize()
		{
			if (nodes.Count == 0) return; // nothing to serialize
			bool wasTriggering = TriggerEvents;
			TriggerEvents = false;

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
						if (socket.Edge.Output.Parent == node)
						{
							serializedEdges.Add(socket.Edge.ToSerializedEgde());
						}
					}
				}
			}
			TriggerEvents = wasTriggering;
		}

		/// <summary>Unity serialization callback.</summary>
		public void OnAfterDeserialize()
		{
			if (serializedNodes.Count == 0) return;	// Nothing to deserialize.
			bool wasTriggering = TriggerEvents;
			TriggerEvents = false;

			nodes.Clear(); // clear original data.

			// deserialize nodes
			foreach (var sNode in serializedNodes)
			{
				Node n = CreateNode(Type.GetType(sNode.type), sNode.id);
				if (n != null)
				{
					JsonUtility.FromJsonOverwrite(sNode.data, n);
					n.OnDeserialization(sNode);
					n.X = sNode.X;
					n.Y = sNode.Y;
					AddNode(n);
				}
			}

			// deserialize edges
			foreach (var sEdge in serializedEdges)
			{
				Node inputNode = GetNode(sEdge.InputNodeId);
				Node outputNode = GetNode(sEdge.OutputNodeId);
				if (inputNode == null || outputNode == null)
				{
					Debug.LogWarning("Try to create an edge but can not find at least on of the nodes.");
					continue;
				}

				if (sEdge.OutputSocketIndex > outputNode.Sockets.Count || sEdge.InputSocketIndex > inputNode.Sockets.Count)
				{
					Debug.LogWarning("Try to create an edge but can not find at least on of the sockets.");
					continue;
				}
				Edge edge = new Edge(inputNode.Sockets[sEdge.InputSocketIndex], outputNode.Sockets[sEdge.OutputSocketIndex]);
				inputNode.Sockets[sEdge.InputSocketIndex].Edge = edge;
				outputNode.Sockets[sEdge.OutputSocketIndex].Edge = edge;
			}
			TriggerEvents = wasTriggering;
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes;
using UnityEngine;

namespace Assets.Code.Bon
{

	[Serializable]
	public class Graph : ISerializationCallbackReceiver
	{
		private List<Node> _nodes = new List<Node>();

		[SerializeField]
		private List<SerializableEdge> _serializedEdges = new List<SerializableEdge>();

		[SerializeField]
		private List<SerializableNode> _serializedNodes = new List<SerializableNode>();

		[SerializeField] public int Version = BonConfig.Version;

		// be warned to allow circles.. if you parse the graph you can end up in
		// an endless recursion this can crash unity.
		[SerializeField] public bool AllowCicles = false;


		private bool _needsUpdate = true;

		[System.NonSerialized] public bool TriggerEvents = true;


		public int ObtainUniqueNodeId()
		{
			var tmpId = 0;
			while (GetNode(tmpId) != null)
			{
				tmpId++;
			}
			return tmpId;
		}

		public Node CreateNode<T>()
		{
			return CreateNode<T>(ObtainUniqueNodeId());
		}

		public Node CreateNode<T>(int id)
		{
			return CreateNode(typeof(T), id);
		}

		public Node CreateNode(Type type)
		{
			return CreateNode(type, ObtainUniqueNodeId());
		}

		public Node CreateNode(Type type, int id)
		{
			_needsUpdate = true;
			try
			{
				return (Node) Activator.CreateInstance(type, id, this);
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Node {0} could not be created " + exception.HelpLink, type.FullName);
			}
			return null;
		}

		public Node GetNode(int nodeId)
		{
			if (_nodes == null) return null;
			foreach (var node in _nodes)
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
			return _nodes.Count;
		}

		public Node GetNodeAt(int index)
		{

			return _nodes[index];
		}

		public void AddNode(Node node)
		{
			_needsUpdate = true;
			_nodes.Add(node);
			if (TriggerEvents)
			{
				EventManager.TriggerOnAddedNode(this, node);
			}
		}

		public void RemoveNode(Node node)
		{
			_needsUpdate = true;
			if (node == null) return;

			foreach (var socket in node.Sockets)
			{
				if (socket.Edge != null)
				{
					UnLink(socket);
				}
			}

			_nodes.Remove(node);
			if (TriggerEvents)
			{
				EventManager.TriggerOnNodeRemoved(this, node);
			}
		}

		public void RemoveNode(int id)
		{
			RemoveNode(GetNode(id));
		}

		public void UnLink(Socket s01, Socket s02)
		{
			_needsUpdate = true;
			if (TriggerEvents)
			{
				EventManager.TriggerOnUnLinkSockets(this, s01, s02);
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
			if (TriggerEvents)
			{
				EventManager.TriggerOnUnLinkedSockets(this, s01, s02);
			}
		}

		public void UnLink(Socket socket)
		{
			_needsUpdate = true;
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
				return false;
			}
			_needsUpdate = true;

			if (inputSocket.Type == sourceSocket.Type)
			{
				Edge edge = new Edge(inputSocket, sourceSocket);
				inputSocket.Edge = edge;
				sourceSocket.Edge = edge;

				if (!AllowCicles && HasCicle())
				{
					// revert
					Debug.Log("Can not link sockets. Circles are not allowed.");
					return false;
				}

				if (TriggerEvents)
				{
					EventManager.TriggerOnLinkEdge(this, edge);
				}
			}
			return true;
		}


		public bool HasCicle()
		{
			foreach (var node in _nodes)
			{
				if (HasCicle(node))
				{
					ResetVisitFlags();
					return true;
				}
				ResetVisitFlags();
			}
			return false;
		}

		private void ResetVisitFlags()
		{
			foreach (var node in _nodes) node.VisitFlag = false;
		}

		private bool HasCicle(Node node)
		{
			if (node.VisitFlag) return true;
			node.VisitFlag = true;
			foreach (Socket s in node.Sockets)
			{
				if (s.Direction == SocketDirection.Output && s.Edge != null)
				{
					return HasCicle(s.Edge.Input.Parent);
				}
			}
			return false;
		}

		public static List<Node> CreateUpperNodesList(Node node)
		{
			List<Node> upperNodes = new List<Node>();
			AddUpperNodes(node, ref upperNodes);
			return upperNodes;
		}

		private static void AddUpperNodes(Node node, ref List<Node> upperNodesList)
		{
			foreach (Socket s in node.Sockets)
			{
				if (s.Direction == SocketDirection.Input && s.Edge != null)
				{
					Socket connected = s.GetConnectedSocket();
					upperNodesList.Add(connected.Parent);
					AddUpperNodes(connected.Parent, ref upperNodesList);
				}
			}
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

		public void LogCircleError()
		{
			Debug.LogError("The graph contains ciclyes.");
		}

		// === SERIALIZATION ===

		public string ToJson()
		{
			return JsonUtility.ToJson(this);
		}

		public static Graph FromJson(string json)
		{
			Graph g = JsonUtility.FromJson<Graph>(json);
			//listener.OnCreate(g);
			EventManager.TriggerOnCreateGraph(g);
			return g;
		}

		public static bool Save(string fileName, Graph graph)
		{
			var file = File.CreateText(fileName);
			file.Write((string) graph.ToJson());
			file.Close();
			return true;
		}

		public static Graph Load(string fileName)
		{
			if(File.Exists(fileName)){
				var file = File.OpenText(fileName);
				var json = file.ReadToEnd();
				file.Close();
				Graph deserializedGraph = FromJson(json);
				if (deserializedGraph.Version != BonConfig.Version)
				{
					Debug.LogWarning("You loading a graph with a different version number: " + deserializedGraph.Version +
						" the current version is " + BonConfig.Version);
				}
				return deserializedGraph;
			} else {
				Debug.Log("Could not Open the file: " + fileName);
				return null;
			}
		}

		public void UpdateNodes()
		{
			if (!_needsUpdate) return;

			if (HasCicle())
			{
				LogCircleError();
				return;
			}

			for (var i = 0; i < GetNodeCount(); i++)
			{
				Node n = GetNodeAt(i);
				var updateable = n as IUpdateable;
				if (updateable != null)
				{
					updateable.Update();
				}
			}
			_needsUpdate = false;
		}

		public void ForceUpdateNodes()
		{
			_needsUpdate = true;
			UpdateNodes();
		}

		/// <summary>Unity serialization callback.</summary>
		public void OnBeforeSerialize()
		{
			if (_nodes.Count == 0) return; // nothing to serialize
			bool wasTriggering = TriggerEvents;
			TriggerEvents = false;

			_serializedEdges.Clear();
			_serializedNodes.Clear();
			// serialize data
			foreach (var node in _nodes)
			{
				_serializedNodes.Add(node.ToSerializedNode());
				foreach (var socket in node.Sockets)
				{
					if (socket.Edge != null)
					{
						// serialize only the edge of the source (to avoid doubled edges)
						if (socket.Edge.Output.Parent == node)
						{
							_serializedEdges.Add(socket.Edge.ToSerializedEgde());
						}
					}
				}
			}
			TriggerEvents = wasTriggering;
		}

		/// <summary>Unity serialization callback.</summary>
		public void OnAfterDeserialize()
		{
			if (_serializedNodes.Count == 0) return;	// Nothing to deserialize.
			bool wasTriggering = TriggerEvents;
			TriggerEvents = false;

			_nodes.Clear(); // clear original data.

			// deserialize nodes
			foreach (var sNode in _serializedNodes)
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
			foreach (var sEdge in _serializedEdges)
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
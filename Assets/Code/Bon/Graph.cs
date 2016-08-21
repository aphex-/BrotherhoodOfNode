using System;
using System.Collections.Generic;
using System.IO;
using Assets.Code.Bon.Nodes;
using Assets.Code.Bon.Socket;
using UnityEngine;

namespace Assets.Code.Bon
{

	[Serializable]
	public class Graph : ISerializationCallbackReceiver
	{

		[SerializeField] public string Name;

		private List<Node> _nodes = new List<Node>();

		[HideInInspector] [SerializeField] private List<SerializableEdge> _serializedEdges = new List<SerializableEdge>();
		[HideInInspector] [SerializeField] private List<SerializableNode> _serializedNodes = new List<SerializableNode>();
		[SerializeField] public int Version = BonConfig.Version;

		// be warned to allow circles.. if you parse the graph you can end up in
		// an endless recursion this can crash unity.
		[HideInInspector] [SerializeField] public bool AllowCicles = false;

		private bool _invalidating = false;

		private bool _needsUpdate = true; [NonSerialized] public bool TriggerEvents = true;

		/// <summary>Returns an id for a Node that is unique for this Graph.</summary>
		/// <returns> An id for a Node that is unique for this Graph.</returns>
		public int ObtainUniqueNodeId()
		{
			var tmpId = 0;
			while (GetNode(tmpId) != null) tmpId++;
			return tmpId;
		}

		/// <summary>Creates a Node of the given type. Type must inherit from Node.
		/// Does not add the Node to the Graph.</summary>
		/// <returns>The created Node of the given Type.</returns>
		public Node CreateNode<T>()
		{
			return CreateNode<T>(ObtainUniqueNodeId());
		}

		/// <summary>Creates a Node of the given type with the assigned id. Type must inherit from Node.
		/// Does not add the Node to the Graph.</summary>
		/// <returns>The created Node of the given Type with the assigned id.</returns>
		public Node CreateNode<T>(int id)
		{
			return CreateNode(typeof(T), id);
		}

		/// <summary>Creates a Node of the given type. Type must inherit from Node.
		/// Does not add the Node to the Graph.</summary>
		/// <param name="type">The Type of the Node to create.</param>
		/// <returns>The created Node of the given Type.</returns>
		public Node CreateNode(Type type)
		{
			return CreateNode(type, ObtainUniqueNodeId());
		}

		/// <summary>Creates a Node of the given type with the assigned id. Type must inherit from Node.
		/// Does not add the Node to the Graph.</summary>
		/// <param name="type">The Type of the Node to create.</param>
		/// <param name="id">The id of the Node to create.</param>
		/// <returns>The created Node of the given Type with the assigned id.</returns>
		public Node CreateNode(Type type, int id)
		{
			if (type == null) return null;
			_needsUpdate = true;
			try
			{
				return (Node) Activator.CreateInstance(type, id, this);
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Node {0} could not be created " + exception.Message, type.FullName);
			}
			return null;
		}

		/// <summary>Returns the Node with the assigned id or null.</summary>
		/// <param name="nodeId">The id of the Node to get.</param>
		/// <returns>The Node with the assigned id or null.</returns>
		public Node GetNode(int nodeId)
		{
			if (_nodes == null) return null;
			foreach (var node in _nodes) if (node.Id == nodeId) return node;
			return null;
		}

		/// <summary>Returns the count of Nodes in this Graph.</summary>
		/// <returns>The count of Nodes in this Graph.</returns>
		public int GetNodeCount()
		{
			return _nodes.Count;
		}

		/// <summary>Returns the Node at the assigned index.</summary>
		/// <param name="index">The index of the Node to get.</param>
		/// <returns>The Node at the assigned index.</returns>
		public Node GetNodeAt(int index)
		{
			if (index >= _nodes.Count) return null;
			return _nodes[index];
		}

		/// <summary>Adds a node to the Graph. Does not add Nodes with an id that is already taken.
		/// Triggers a 'AddedNode' event. </summary>
		/// <param name="node">The Node to add.</param>
		/// <returns>True if the node was added.</returns>
		public bool AddNode(Node node)
		{
			if (GetNode(node.Id) != null) return false;
			_needsUpdate = true;
			_nodes.Add(node);
			if (TriggerEvents) EventManager.TriggerOnAddedNode(this, node);
			return true;
		}

		/// <summary>Removes the assigned Node from this Graph.</summary>
		/// <param name="node">The Node to remove.</param>
		/// <returns>True if the Node was removed.</returns>
		public bool RemoveNode(Node node)
		{
			if (node == null) return false;
			_needsUpdate = true;

			foreach (var socket in node.Sockets) UnLink(socket);
			bool removed = _nodes.Remove(node);
			if (TriggerEvents) EventManager.TriggerOnNodeRemoved(this, node);
			return removed;
		}

		/// <summary>Removes the Node with the assigned id from this Graph.</summary>
		/// <param name="id">The id of the Node to remove.</param>
		/// <returns>True if the Node was removed.</returns>
		public bool RemoveNode(int id)
		{
			return RemoveNode(GetNode(id));
		}

		public bool AreConected(InputSocket inputSocket, OutputSocket outputSocket)
		{
			if (inputSocket == null || outputSocket == null || inputSocket.Edge == null || outputSocket.Edges.Count == 0) return false;
			return outputSocket.Edges.Contains(inputSocket.Edge);
		}

		/// <summary>Unlinkes the assigned sockets. Triggeres 'Unlink' events.</summary>
		public void UnLink(InputSocket inputSocket, OutputSocket outputSocket)
		{
			_needsUpdate = true;

			if (inputSocket == null || outputSocket == null || !inputSocket.IsConnected() || !outputSocket.IsConnected()) return;
			if (!AreConected(inputSocket, outputSocket)) return;

			if (TriggerEvents)
			{
				EventManager.TriggerOnUnLinkSockets(this, inputSocket, outputSocket);
			}

			var index = outputSocket.Edges.IndexOf(inputSocket.Edge);
			if (index > -1)
			{
				outputSocket.Edges[index].Input = null;
				outputSocket.Edges[index].Output = null;
				outputSocket.Edges.RemoveAt(index);
			}

			inputSocket.Edge.Input = null;
			inputSocket.Edge.Output = null;
			inputSocket.Edge = null;

			if (TriggerEvents)
			{
				EventManager.TriggerOnUnLinkedSockets(this, inputSocket, outputSocket);
			}
		}

		public void UnLink(AbstractSocket socket)
		{
			if (socket == null || !socket.IsConnected()) return;


			if (socket.IsInput())
			{
				InputSocket inputSocket = (InputSocket) socket;
				if (inputSocket.Edge != null) UnLink(inputSocket, inputSocket.Edge.Output);
			}

			if (socket.IsOutput())
			{
				OutputSocket outputSocket = (OutputSocket) socket;
				Edge[] edgeCopy = new Edge[outputSocket.Edges.Count];
				outputSocket.Edges.CopyTo(edgeCopy);
				foreach (var edge in edgeCopy)
				{
					UnLink(edge.Input, outputSocket);
				}
			}
		}

		public bool Link(InputSocket inputSocket, OutputSocket outputSocket)
		{
			if (!CanBeLinked(inputSocket, outputSocket))
			{
				Debug.LogWarning("Sockets can not be linked.");
				return false;
			}
			_needsUpdate = true;

			if (inputSocket.Type == outputSocket.Type)
			{
				Edge edge = new Edge(outputSocket, inputSocket);
				Edge oldEdge = inputSocket.Edge;
				inputSocket.Edge = edge;
				outputSocket.Edges.Add(edge);

				if (!AllowCicles && HasCycle())
				{
					// revert
					inputSocket.Edge = oldEdge;
					outputSocket.Edges.Remove(edge);
					Log.Info("Can not link sockets. Circles are not allowed.");
					return false;
				}

				if (TriggerEvents)
				{
					EventManager.TriggerOnLinkEdge(this, edge);
				}
			}
			return true;
		}

		private void StartInvalidation() 
		{
			if (_invalidating) 
			{
				throw new UnityException ("Graph is already invalidating");
			}

			_invalidating = true;

			foreach (var node in _nodes) 
			{
				node.VisitCount = 0;
			}
		}

		private void EndInvalidation() 
		{
			_invalidating = false;
		}

		public void InvalidateNodeDependents(Node node)
		{
			foreach (AbstractSocket s in node.Sockets) 
			{
				if (s.IsOutput () && s.IsConnected ()) 
				{
					OutputSocket outputSocket = (OutputSocket) s;
					foreach (var edge in outputSocket.Edges) 
					{
						var invalidations = ++edge.Input.Parent.VisitCount;

						if (invalidations == edge.Input.Parent.GetNumConnectedInputEdges()) 
						{
							InvalidateNodeDependents (edge.Input.Parent);
						}
					}

				}
			}
		}



		private void InvalidateFromRootNodes() 
		{
			foreach (var node in _nodes) 
			{
				if (node.IsRootNode())
					InvalidateNodeDependents (node);
			}
		}

		public bool HasCycle() {

			StartInvalidation ();

			InvalidateFromRootNodes ();

			foreach (var node in _nodes) 
			{
				if (node.VisitCount != node.GetNumConnectedInputEdges()) 
				{
					EndInvalidation ();
					return true;
				}
			}

			EndInvalidation ();
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
			foreach (AbstractSocket s in node.Sockets)
			{
				if (s.IsInput() && s.IsConnected())
				{
					InputSocket inputSocket = (InputSocket) s;
					AbstractSocket connected = inputSocket.GetConnectedSocket();
					upperNodesList.Add(connected.Parent);
					AddUpperNodes(connected.Parent, ref upperNodesList);
				}
			}
		}

		/// <summary> Returns true if the sockets can be linked.</summary>
		/// <param name="inSocket"> The input socket</param>
		/// <param name="outSocket"> The output socket</param>
		/// <returns>True if the sockets can be linked.</returns>
		public bool CanBeLinked(InputSocket inSocket, OutputSocket outSocket)
		{
			return inSocket != null && outSocket != null && outSocket.Type == inSocket.Type;
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


		private void InternalUpdateNode(Node node) {
			var updateable = node as IUpdateable;
			if (updateable != null)
			{
				updateable.Update();
			}

			foreach (AbstractSocket s in node.Sockets) {
				if (s.IsOutput () && s.IsConnected ()) {
					OutputSocket outputSocket = (OutputSocket)s;
					foreach (var edge in outputSocket.Edges) {
						var invalidations = --edge.Input.Parent.VisitCount;
						if (invalidations == 0) {
							InternalUpdateNode (edge.Input.Parent);
						}
					}
				}
			}
		}
		public void UpdateNodeAndDependents(Node node)
		{
			if (!_needsUpdate) return;

			StartInvalidation ();

			InvalidateNodeDependents (node);

			InternalUpdateNode (node);

			EndInvalidation ();
			_needsUpdate = false;
		}

		public void UpdateNodes()
		{
			if (!_needsUpdate) return;

			StartInvalidation ();
			InvalidateFromRootNodes ();

			foreach (var node in _nodes) {
				if (node.IsRootNode())
					InternalUpdateNode (node);
			}

			EndInvalidation ();
			_needsUpdate = false;
		}

		public void ForceUpdateNodeAndDependents(Node node) {
			_needsUpdate = true;
			UpdateNodeAndDependents (node);
		}

		public void ForceUpdateNodes()
		{
			_needsUpdate = true;
			UpdateNodes ();
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
					if (socket.IsInput() && socket.IsConnected()) // serialize only input socket edges to avoid double edge serialization
					{
						InputSocket inputSocket = (InputSocket) socket;
						_serializedEdges.Add(inputSocket.Edge.ToSerializedEgde());
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
				Type nodeType = Type.GetType(sNode.type);
				if (nodeType == null)
				{
					Debug.LogWarning("Unknown node type: " + sNode.type);
					continue;
				}
				Node n = CreateNode(nodeType, sNode.id);
				if (n != null)
				{
					JsonUtility.FromJsonOverwrite(sNode.data, n);
					n.OnDeserialization(sNode);
					n.X = sNode.X;
					n.Y = sNode.Y;

					for (var i = 0; i < sNode.directInputValues.Length; i++)
					{
						if (n.Sockets[i].IsInput())
						{
							InputSocket inputSocket = (InputSocket) n.Sockets[i];
							inputSocket.SetDirectInputNumber(sNode.directInputValues[i], false);
						}
					}

					if (sNode.Collapsed) n.Collapse();
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

				InputSocket inputSocket = (InputSocket) inputNode.Sockets[sEdge.InputSocketIndex];
				OutputSocket outputSocket = (OutputSocket) outputNode.Sockets[sEdge.OutputSocketIndex];
				Edge edge = new Edge(outputSocket, inputSocket);
				inputSocket.Edge = edge;
				outputSocket.Edges.Add(edge);
			}
			TriggerEvents = wasTriggering;
		}
	}
}
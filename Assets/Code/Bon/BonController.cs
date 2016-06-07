using System;
using System.Collections.Generic;
using Assets.Code.Bon.Graph;
using Assets.Code.Bon.Graph.Custom;
using UnityEngine;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Assets.Code.Bon
{
	public class BonController : IGraphListener
	{


		public void OnWindowOpen()
		{
			Debug.Log("OnWindowOpen");
		}

		// =====  Graph Initialization =====
		public Graph.Graph LoadGraph(string path)
		{
			Debug.ClearDeveloperConsole();

			if (path.Equals(BonConfig.DefaultGraphName))
			{
				Graph.Graph graph = new Graph.Graph();
				graph.RegisterListener(this);

				var samplerNode01 = new SamplerNode(graph.GetUniqueId());
				samplerNode01.X = 20;
				samplerNode01.Y = 20;
				graph.nodes.Add(samplerNode01);

				var multiplexer01 = new Multiplexer(graph.GetUniqueId());
				multiplexer01.X = 200;
				multiplexer01.Y = 200;
				graph.nodes.Add(multiplexer01);

				graph.Link(samplerNode01.GetSocket(Color.red, 1), multiplexer01.GetSocket(Color.red, 0));


				// test serialization an deserialization
				string serializedJSON = graph.ToJson();
				Graph.Graph deserializedGraph = Graph.Graph.FromJson(serializedJSON);

				return deserializedGraph;
			}
			else
			{
				Graph.Graph graph = Graph.Graph.Load(path);
				graph.RegisterListener(this);
				return graph;
			}

		}


		public void SaveGraph(Graph.Graph g, string path)
		{
			Graph.Graph.Save(path, g);
		}

		public Dictionary<string, Type> CreateMenuEntries(string graphId)
		{
			Dictionary<string, Type> menuEntries = new Dictionary<string, Type>();

			IEnumerable<Type> classesExtendingNode = Assembly.GetAssembly(typeof(Node)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Node)));

			foreach (Type type in classesExtendingNode)
			{
				menuEntries.Add(getItemMenuName(type), type);
			}

			menuEntries.OrderBy(x => x.Key);

			return menuEntries;
		}

		private string getItemMenuName(Type type)
		{
			object[] attrs = type.GetCustomAttributes(typeof(GraphContextMenuItem), true);

			if (attrs.Length == 0)
			{
				return type.Name;
			}
			else
			{
				GraphContextMenuItem attr = (GraphContextMenuItem)attrs[0];
				StringBuilder name = new StringBuilder(string.IsNullOrEmpty(attr.Name) ? type.Name : attr.Name);

				if (!string.IsNullOrEmpty(attr.Path))
				{
					name.Insert(0, string.Format("{0}{1}", attr.Path, attr.Path.EndsWith("/") ? "" : "/"));
				}

				return name.ToString();
			}

		}




		// ======= Events =======

		public void OnLink(Edge edge)
		{
			Debug.Log("OnLink: Node " + edge.Source.Parent.Id + " with Node " + edge.Sink.Parent.Id);
		}

		public void OnUnLink(Socket s01, Socket s02)
		{
			Debug.Log("OnUnLink: Node " + s01.Edge.Source.Parent.Id + " from Node " + s02.Edge.Sink.Parent.Id);
		}

		public void OnNodeRemoved(Node node)
		{
			Debug.Log("OnNodeRemoved: Node " + node.Id);
		}


	}
}



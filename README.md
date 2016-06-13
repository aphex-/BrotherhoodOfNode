# BrotherhoodOfNode
A visual graph editor for Unity3D

![alt text](https://github.com/aphex-/BrotherhoodOfNode/blob/master/preview.png "preview")

### Install

#### This project is still under development... But you can use it already!

1. copy the files from the asset folder to your unity project asset folder
2. in unity click on window->BrotherhoodOfNode to open the editor window
3. continue reading and afterwards take a look at Assets.Code.Bon.BonController.cs to learn how the project works
4. help us make it even better

### Features
* user friendly editor UI
* human readable JSON serialization of the graph
* easy creation of custom nodes (extend Node.cs)
* tab pages for multiple graphs

### Non-Features
* nothing but a tool to create graphs and make them persitent

### Usage
Right click to add a node. Middle mouse button to scroll. DEL to delete a node. Click/Drag to connect Sockets. Mouse wheel to zoom.



## How To Create A Custom Node
To create your own nodes you need to create a new class. Let's call it MyNode.cs
for this example. **MyNode** now needs to inherit from the class Node. And this 
class needs the annotation **[Serializable]** in order to save it as a json file.
```cs
using System;
using UnityEngine;

namespace Assets.Code.Bon.Graph.Custom
{
	[Serializable]
	[GraphContextMenuItem("Standard", "MyNode")]
	public class MyNode : Node {

		[SerializeField]
		private int myNumber;

		public MyNode(int id) : base(id)
		{
			Sockets.Add(new Socket(this, Color.red, true));
			Sockets.Add(new Socket(this, Color.red, false));
			Sockets.Add(new Socket(this, Color.red, false));
			Height = 65;
		}

		public override void OnGUI()
		{
			// for custom ui elements
		}

		public override void OnSerialization(SerializableNode sNode)
		{
			// for custom serialization
		}

		public override void OnDeserialization(SerializableNode sNode)
		{
			// for custom deserialization
		}
	}
}
```
The anotation **[GraphContextMenuItem]** tells the editor where to add the menu entry for our **Node**.

### Adding Sockets
We created a node class that contains three red **Sockets**. A **Socket** needs a parent node as first parameter (this) and a connection type (Color.Red). The last boolean parameter tells the socket to be on the left (true) or on the right (false) side of the node.

### Add UI elememts
To add custom UI elements to your node simply use the **OnGUI** method as usual.

### Save nodes as json (serialization / deserializaiton)
If we have got class members (like myNumber) we want to make persistent
we need to prefix the annotation **[SerializeField]**
to it.
Also take a look at: http://docs.unity3d.com/Manual/JSONSerialization.html
If you really need a more complex way to serialize your **Node** you can use
the methods **OnSerialization** and **OnDeserialization** to add your logic.


## How To Receive Graph Events

Every time a **Graph** is created you should register a **IGraphListener**
in order to receive events of this graph. You can create your own class 
that inherits from **IGraphListener** and add your own logic.
The default implementation that inherits from **IGraphListener** is our
**BonController**.

```cs
	public void OnLink(Edge edge)
	{
		Debug.Log("OnLink: Node " + edge.Source.Parent.Id + " with Node " + edge.Sink.Parent.Id);
	}

	public void OnUnLink(Socket s01, Socket s02)
	{
		Debug.Log("OnUnLink: Node " + s01.Edge.Source.Parent.Id + " from Node " + s02.Edge.Sink.Parent.Id);
	}

	public void OnNodeAdded(Node node)
	{
		Debug.Log("OnNodeAdded: Node " + node.GetType() + " with id " + node.Id);
	}

	public void OnNodeRemoved(Node node)
	{
		Debug.Log("OnNodeRemoved: Node " + node.GetType() + " with id " + node.Id);
	}

	public void OnNodeChanged(Node node)
	{
		Debug.Log("OnNodeChanged: Node " + node.GetType() + " with id " + node.Id);
	}
```

### Next Up..
* Multiple edges per socket?
* A help dialog to explain the controls.
* Code style and code documentation (no idea whats the state of the art. Following microsofts or unitys recommendations?)
* cleanup code, refactor namespaces

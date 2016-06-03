# BrotherhoodOfNode
A visual graph editor for Unity3D

![alt text](https://github.com/aphex-/BrotherhoodOfNode/blob/master/preview.png "preview")

### Work in progress!

This project is still under development. If you want to try it out:

1. copy the files from the asset folder to your unity project asset folder
2. in unity click on window->BrotherhoodOfNode to open the editor window
3. start at Assets.Code.Bon.BonController.cs to learn how the project works

### Features
* user friendly editor UI
* human readable JSON serialization of the graph
* easy creation of custom nodes (extend Node.cs)

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
	public class MyNode : Node {

		public MyNode(int id) : base(id)
		{
			Sockets.Add(new Socket(this, Color.red, true));
			Sockets.Add(new Socket(this, Color.red, false));
			Sockets.Add(new Socket(this, Color.red, false));
			Height = 65;
		}

		public override void OnGUI()
		{
		}

		public override void ApplySerializationData(SerializableNode sNode)
		{
			sNode.data = JsonUtility.ToJson(this);
		}
	}
}

```
### Adding Sockets
We created a node class that contains three red **Sockets**. A **Socket** needs a parent node as first parameter (this) and a connection type (Color.Red). The last boolean parameter tells the socket to be on the left (true) or on the right (false) side of the node.

### Add UI elememts
To add custom UI elements to your node simply use the **OnGUI** method as usual.

### Save nodes as json
(This may change in future versions)
To be able to save the node in a json file (to make it persistent) we need 
to override the method **ApplySerializationData**. It gives us a parameter 
of the type **SerializableNode** where we can save our custom data.

## How To Register A Custom Node
In order to let the editor know about your cutsom node you need to add logic how to create it. Find the class Graph.cs
and its method **CreateNode**. Notice: the method is overloaded.. we need the one that looks like this:
```cs
	public Node CreateNode(Type nodeType, int id)
	{
		if (nodeType == typeof(Multiplexer)) return new Multiplexer(id);
		if (nodeType == typeof(SamplerNode)) return new SamplerNode(id);
		Debug.Log("Can not create a node of the type '" + nodeType.FullName + "'");
		return null;
	}
```
You just need to create another if statement to check for our **MyNode** class and return a new instance.

We also need to add a menu entry of our node to make it addable from the editor.
```cs
Find the class **BonController ** and its method **CreateMenuEntries** that looks like this:
	public Dictionary<string, Type> CreateMenuEntries(string graphId)
	{
		Dictionary<string, Type> menuEntries = new Dictionary<string, Type>();
		menuEntries.Add("Standard/SamplerNode", 	typeof(SamplerNode));
		menuEntries.Add("Standard/Multiplex", 	typeof(Multiplexer));
		return menuEntries;
	}
```
You just need to add another entry to the **menuEntries** Dictionary. The first 
parameter is the menu hierarchy as a string. The second parameter is the type
of node to create if the menu entry is clicked. In our case this would be **MyNode**.



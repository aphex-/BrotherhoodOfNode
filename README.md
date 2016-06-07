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
	[GraphContextMenuItem("Standard", "MyNode")]
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
The anotation **[GraphContextMenuItem]** tells the editor where to add the menu entry for our **Node**.

### Adding Sockets
We created a node class that contains three red **Sockets**. A **Socket** needs a parent node as first parameter (this) and a connection type (Color.Red). The last boolean parameter tells the socket to be on the left (true) or on the right (false) side of the node.

### Add UI elememts
To add custom UI elements to your node simply use the **OnGUI** method as usual.

### Save nodes as json
(This may change in future versions)
To be able to save the node in a json file (to make it persistent) we need 
to override the method **ApplySerializationData**. It gives us a parameter 
of the type **SerializableNode** where we can save our custom data.

### Next Up..
* The IGraphListener needs a graph-id parameter for each method. 
* * A help dialog to explain the controls.
* Code style and code documentation (no idea whats the state of the art. Following microsofts or unitys recommendations?)
* cleanup code, refactor namespaces

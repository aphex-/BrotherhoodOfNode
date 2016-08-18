# BrotherhoodOfNode
A visual graph editor for Unity3D

![alt text](https://github.com/aphex-/BrotherhoodOfNode/blob/master/preview.png "math Nodes preview")

![alt text](https://github.com/aphex-/BrotherhoodOfNode/blob/master/noise_preview.png "noise Nodes preview")

### Install

#### This project is still under development... But you can use it already!

1. copy the files from the asset folder to your unity project asset folder
2. in unity click on window->BrotherhoodOfNode to open the editor window
3. continue reading and afterwards take a look at Assets.Code.Bon.BonLauncher.cs to learn how the project works
4. help us make it even better

### Features
* user friendly editor UI
* human readable JSON serialization of the graph
* easy creation of custom nodes (extend Node.cs)
* tab pages for multiple graphs

![alt text](https://github.com/aphex-/BrotherhoodOfNode/blob/master/feature_preview.gif "feature preview")

### Non-Features
* nothing but a tool to create graphs and make them persitent (ok.. there are some math related Nodes now..)


### Usage
Right click to add a node. Middle mouse button to scroll. Right click on a node to delete a node. Click/Drag to connect Sockets. Mouse wheel to zoom.



## How To Create A Custom Node
To create your own nodes you need to create a new class. Let's call it MyNode.cs
for this example. **MyNode** now needs to inherit from the class **Node** (or from its abstract subclasses). And this 
class needs the annotation **[Serializable]** in order to save it as a json file.

Notice that our **Node** is extending **AbstractNumberNode** and contains an output socket for numbers.

```cs
using System;
using UnityEngine;

namespace Assets.Code.Bon.Graph.Custom
{
	[Serializable]
	[GraphContextMenuItem("Standard", "MyNode")]
	public class MyNode : AbstractNumberNode {

		[SerializeField]
		private int myNumber;

		public MyNode(int id, Graph parent) : base(id, parent)
		{
			// Add the input / output sockets of this Node
			Sockets.Add(new OutputSocket(this, typeof(AbstractNumberNode));
			Sockets.Add(new InputSocket(this, typeof(AbstractColorNode));
			Sockets.Add(new InputSocket(this, typeof(AbstractNumberNode));
			Height = 65;
		}

		// To create your custom UI elements in the Node.
		public override void OnGUI()
		{
			// Use GUI or GUILayout as usual
			// (if your nodes content has changed call: TriggerChangeEvent())
		}
		
		// This method comes from the AbstractNumberNode. It makes sure that all classes of
		// this type can return a number. Return your number here depending on the parameters.
		// Your number is usually also depending on the InputSockets of this Node. The assigned
		// OutputSocket can be igrnored as long as you only have one output.
		public override float GetNumber(OutputSocket outSocket, float x, float y, float z, float seed)
		{
			return 0f;
		}
	

	}
}
```
The anotation **[GraphContextMenuItem]** tells the editor where to add the menu entry for our **Node**.


### Add UI elememts
To add custom UI elements to your node simply use the **OnGUI** method as usual.

## How To Receive Graph Events
The editor (now) uses the **EventManager** and its static methods to trigger events.
To receive the events simply subscribe to the **EventManager**. 
```cs
	public Awake()
	{
		EventManager.OnCreateGraph += OnCreate;
		EventManager.OnChangedNode += OnNodeChanged;
		// .. there are more events
	}

	public void OnCreate(Graph graph)
	{
		Debug.Log("OnCreateGraph");
		graph.UpdateNodes();
	}

	public void OnNodeChanged(Graph graph, Node node)
	{
		Debug.Log("OnNodeChanged: Node " + node.GetType() + " with id " + node.Id);
		graph.ForceUpdateNodes();
	}
```

## How To Update The Graphs
You can find the standard implementation for updating the Graphs in the **StandardGraphController**. This class subscribes to the *EventManager* to update the Graph on the events. You maybe want to extend its logic to also update objects in your scene. You could also alter the update mechanism or something.

### Save nodes as json (serialization / deserializaiton)
If we have got class members (like myNumber) we want to make persistent
we need to prefix the annotation **[SerializeField]**
to it. You may also need **[System.NonSerialized]** to avoid serialization for your memners.
Also take a look at: http://docs.unity3d.com/Manual/JSONSerialization.html
If you really need a more complex way to serialize your **Node** you can use
the methods **OnSerialization** and **OnDeserialization** to add your logic.


### Next Up..
* Implement multiple output Edges per Socket https://github.com/aphex-/BrotherhoodOfNode/issues/3
* Remove the tab-pages and implement the possibility of multiple editor windows
* Check for recursion in the graph.
* Unit tests
* A help dialog to explain the controls.
* Code style and code documentation (no idea whats the state of the art. Following microsofts or unitys recommendations?)
* cleanup code, refactor namespaces

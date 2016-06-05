using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assets.Code.Bon;
using Assets.Code.Bon.Graph;
using Assets.Editor.Bon;

namespace Assets.Editor
{
	public class BonWindow : EditorWindow
	{
		private const string Name = "BrotherhoodOfNode";
		public const int TopOffset = 20;
		private const int WindowTitleHeight = 21; // Unity issue
		private const float CanvasZoomMin = 0.01f;
		private const float CanvasZoomMax = 1.0f;

		private readonly Rect openButtonRect = new Rect(0, 0, 80, TopOffset);
		private readonly Rect saveButtonRect = new Rect(80, 0, 80, TopOffset);
		private readonly Rect helpButtonRect = new Rect(160, 0, 80, TopOffset);

		private BonController controller;
		private Dictionary<string, BonCanvas> canvasList = new Dictionary<string, BonCanvas>();
		private BonCanvas currentCanvas = null;
		private Rect canvasRegion = new Rect();

		private Socket currentDragSocket = null;
		private Vector2 lastMousePosition = new Vector2();

		private GenericMenu menu;
		private Dictionary<string, Type> menuEntryToNodeType;


		[MenuItem("Window/" + Name)]
		public static void CreateEditor()
		{
			BonWindow window = EditorWindow.GetWindow<BonWindow>();
			window.wantsMouseMove = true;
			window.Show();
		}

		public BonWindow()
		{
			titleContent = new GUIContent(Name);
			controller = new BonController();
			controller.OnWindowOpen();

			menuEntryToNodeType = controller.CreateMenuEntries(BonConfig.DefaultGraphName);
			Graph graph = controller.LoadGraph(BonConfig.DefaultGraphName);
			currentCanvas = new BonCanvas(graph);
			canvasList.Add(graph.id, currentCanvas);
			menu = CreateGenericMenu();
		}


		void OnGUI()
		{
			canvasRegion.Set(0, TopOffset, Screen.width, Screen.height);

			HandleNodeRemoving();
			HandleCanvasTranslation();
			HandleDragAndDrop();

			if (Event.current.type == EventType.ContextClick)
			{
				menu.ShowAsContext();
				Event.current.Use();
			}
			HandleMenuButtons();

			currentCanvas.Draw((EditorWindow) this, canvasRegion, currentDragSocket);

			lastMousePosition = Event.current.mousePosition;
		}

		private GenericMenu CreateGenericMenu()
		{
			GenericMenu m = new GenericMenu();
			foreach(KeyValuePair<string, Type> entry in menuEntryToNodeType)
				m.AddItem(new GUIContent(entry.Key), false, OnGenericMenuClick, entry.Value);
			return m;
		}

		private void OnGenericMenuClick(object item)
		{
			currentCanvas.CreateNode((Type) item, lastMousePosition);
		}


		private void HandleMenuButtons()
		{
			if (GUI.Button(openButtonRect, "Load"))
			{
				var path = EditorUtility.OpenFilePanel("load graph data", "", "json");
				if (!path.Equals(""))
				{
					currentCanvas.Graph = controller.LoadGraph(path);
				}
			}

			// Save Button
			if (GUI.Button(saveButtonRect, "Save"))
			{
				var path = EditorUtility.SaveFilePanel("save graph data", "", "graph", "json");
				if (!path.Equals(""))
				{
					controller.SaveGraph(currentCanvas.Graph, path);
				}
			}

			// Help Button
			GUI.Button(helpButtonRect, "Help");
		}

		private void HandleNodeRemoving()
		{
			// Delete or Backspace
			if (Event.current.keyCode == KeyCode.Delete || Input.GetKeyDown(KeyCode.Backspace))
			{
				currentCanvas.RemoveFocusedNode();
				Repaint();
			}
		}

		private void HandleCanvasTranslation()
		{
			// Zoom
			if (Event.current.type == EventType.ScrollWheel)
			{
				Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(Event.current.mousePosition);
				float zoomDelta = -Event.current.delta.y/150.0f;
				float oldZoom = currentCanvas.Zoom;
				currentCanvas.Zoom = Mathf.Clamp(currentCanvas.Zoom + zoomDelta, CanvasZoomMin, CanvasZoomMax);
				currentCanvas.Position += (zoomCoordsMousePos - currentCanvas.Position) -
				                    (oldZoom/currentCanvas.Zoom)*(zoomCoordsMousePos - currentCanvas.Position);
				Event.current.Use();
				return;
			}

			// Translate
			if (Event.current.type == EventType.MouseDrag &&
			    (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
			    Event.current.button == 2)
			{
				Vector2 delta = Event.current.delta;
				delta /= currentCanvas.Zoom;
				currentCanvas.Position += delta;

				Event.current.Use();
				return;
			}
		}

		private void HandleDragAndDrop()
		{
			if (Event.current.type == EventType.MouseDown)
			{
				Socket target = currentCanvas.GetSocketAt(Event.current.mousePosition);
				if (target != null && currentDragSocket == null)
				{
					if (target.Edge == null)
					{
						currentDragSocket = target;
					}
					else
					{
						currentDragSocket = target.Edge.GetOtherSocket(target);
						currentCanvas.Graph.UnLink(currentDragSocket, target);
					}
					Event.current.Use();
				}
			}

			if (Event.current.type == EventType.MouseUp)
			{
				if (currentDragSocket != null)
				{
					Socket target = currentCanvas.GetSocketAt(Event.current.mousePosition);
					if (currentCanvas.Graph.CanBeLinked(target, currentDragSocket))
					{
						// drop edge event
						if (target.Edge != null)
						{
							// replace edge
							currentCanvas.Graph.UnLink(target);
						}
						currentCanvas.Graph.Link(currentDragSocket, target);
						Event.current.Use();
					}
					currentDragSocket = null;
					Repaint();
				}
			}

			if (Event.current.type == EventType.MouseDrag)
			{
				if (currentDragSocket != null) Event.current.Use();
			}
		}

		private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
		{
			return (screenCoords - canvasRegion.TopLeft())/currentCanvas.Zoom + currentCanvas.Position;
		}

	}
}



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
		private BonController _controller;
		private Graph graph;

		private Socket _currentDragSocket = null;
		private Dictionary<string, Type> _menuEntryToNodeType;


		public const int TopOffset = 20;
		private const int WindowTitleHeight = 21; // Unity issue

		private const float CanvasZoomMin = 0.01f;
		private const float CanvasZoomMax = 1.0f;

		private Dictionary<string, BonCanvas> canvasList = new Dictionary<string, BonCanvas>();
		private BonCanvas currentCanvas;

		private Rect _zoomArea = new Rect();

		private Vector2 _lastMousePosition = new Vector2();
		private Vector2 _tmpVector01 = new Vector2();
		private Vector2 _tmpVector02 = new Vector2();

		private Rect openButtonRect = new Rect(0, 0, 80, TopOffset);
		private Rect saveButtonRect = new Rect(80, 0, 80, TopOffset);
		private Rect helpButtonRect = new Rect(160, 0, 80, TopOffset);


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
			_controller = new BonController();
			_controller.OnWindowOpen();


			_menuEntryToNodeType = _controller.CreateMenuEntries(BonConfig.DefaultGraphName);
			graph = _controller.LoadGraph(BonConfig.DefaultGraphName);
			currentCanvas = new BonCanvas(graph);
			canvasList.Add(graph.id, currentCanvas);
			menu = CreateGenericMenu();
		}

		private GenericMenu menu;

		void OnGUI()
		{
			_zoomArea.Set(0, TopOffset, Screen.width, Screen.height);

			HandleNodeRemoving();
			HandleCanvasTranslation();
			HandleConextMenu();
			HandleDragAndDrop();

			if (Event.current.type == EventType.ContextClick)
			{
				menu.ShowAsContext();
				Event.current.Use();
			}
			HandleMenuButtons();
			DrawZoomArea();

			_lastMousePosition = Event.current.mousePosition;
		}

		private GenericMenu CreateGenericMenu()
		{
			GenericMenu menu = new GenericMenu();
			foreach(KeyValuePair<string, Type> entry in _menuEntryToNodeType)
				menu.AddItem(new GUIContent(entry.Key), false, OnGenericMenuClick, entry.Value);
			return menu;
		}

		private void OnGenericMenuClick(object item)
		{
			Node node = graph.CreateNode((Type) item);
			var position = currentCanvas.ProjectToDrawArea(_lastMousePosition);
			node.X = position.x;
			node.Y = position.y;
			graph.nodes.Add(node);
			//UpdateCanvas();
		}


		private void HandleMenuButtons()
		{
			if (GUI.Button(openButtonRect, "Load"))
			{
				var path = EditorUtility.OpenFilePanel("load graph data", "", "json");
				if (!path.Equals(""))
				{
					graph = _controller.LoadGraph(path);
					//UpdateCanvas();
				}
			}

			// Save Button
			if (GUI.Button(saveButtonRect, "Save"))
			{
				var path = EditorUtility.SaveFilePanel("save graph data", "", "graph", "json");
				if (!path.Equals(""))
				{
					_controller.SaveGraph(graph, path);
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
				Node node = currentCanvas.GetFocusedNode();
				if (node != null)
				{
					graph.RemoveNode(node);
					Repaint();
				}
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
				if (target != null && _currentDragSocket == null)
				{
					if (target.Edge == null)
					{
						_currentDragSocket = target;
					}
					else
					{
						_currentDragSocket = target.Edge.GetOtherSocket(target);
						graph.UnLink(_currentDragSocket, target);
					}
					Event.current.Use();
				}
			}

			if (Event.current.type == EventType.MouseUp)
			{
				if (_currentDragSocket != null)
				{
					Socket target = currentCanvas.GetSocketAt(Event.current.mousePosition);
					if (CanBeLinked(target, _currentDragSocket))
					{
						// drop edge event
						if (target.Edge != null)
						{
							// replace edge
							graph.UnLink(target);
						}
						graph.Link(_currentDragSocket, target);
						Event.current.Use();
					}
					_currentDragSocket = null;
					Repaint();
				}
			}

			if (Event.current.type == EventType.MouseDrag)
			{
				if (_currentDragSocket != null) Event.current.Use();
			}
		}



		private void HandleConextMenu()
		{
			// Context Menu
			if (Event.current.type == EventType.MouseDown &&
			    (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
			    Event.current.button == 2)
			{
			}
		}

		/// <summary> Returns true if the sockets can be linked.</summary>
		/// <param name="socket01"> The first socket</param>
		/// <param name="socket02"> The second socket</param>
		/// <returns>True if the sockets can be linked.</returns>
		private bool CanBeLinked(Socket socket01, Socket socket02)
		{
			return socket02 != null && socket01 != null && socket01.Type == socket02.Type
			       && socket01 != socket02;
		}



		private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
		{
			return (screenCoords - _zoomArea.TopLeft())/currentCanvas.Zoom + currentCanvas.Position;
		}



		private void DrawZoomArea()
		{
			EditorZoomArea.Begin(currentCanvas.Zoom, _zoomArea);
			currentCanvas.DrawArea.Set(currentCanvas.Position.x, currentCanvas.Position.y, 100000.0f, 100000.0f);
			GUILayout.BeginArea(currentCanvas.DrawArea, currentCanvas.Style);
			currentCanvas.DrawEdges();
			BeginWindows();
			currentCanvas.DrawNodes();
			EndWindows();
			DrawDragEdge();
			GUILayout.EndArea();
			EditorZoomArea.End();
		}

		private void DrawDragEdge()
		{
			if (_currentDragSocket != null)
			{
				_tmpVector01 = Edge.GetEdgePosition(_currentDragSocket, _tmpVector01);
				_tmpVector02 = Edge.GetTangentPosition(_currentDragSocket, _tmpVector01);
				Edge.DrawEdge(_tmpVector01, _tmpVector02, Event.current.mousePosition, Event.current.mousePosition,
					_currentDragSocket.Type);
			}
		}
	}
}



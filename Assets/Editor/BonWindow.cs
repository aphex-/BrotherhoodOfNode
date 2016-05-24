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

		private readonly BonCanvas _canvas;
		private Socket _currentDragSocket = null;
		private List<string> _nodeTypes;


		public const int TopOffset = 20;
		private const int WindowTitleHeight = 21; // Unity issue

		private const float CanvasZoomMin = 0.01f;
		private const float CanvasZoomMax = 1.0f;

		private Rect _zoomArea = new Rect();

		private Vector2 _lastMousePosition = new Vector2();

		private Vector2 _tmpVector01 = new Vector2();
		private Vector2 _tmpVector02 = new Vector2();


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
			_canvas = new BonCanvas();

			string graphId = "default";
			_nodeTypes = _controller.CreateNodeTypeList(graphId);
			_canvas.Nodes = _controller.LoadGraph(graphId);
		}


		void OnGUI()
		{
			_zoomArea.Set(0, TopOffset, Screen.width, Screen.height);

			HandleNodeRemoving();
			HandleCanvasTranslation();
			HandleConextMenu();
			HandleDragAndDrop();

			if (Event.current.type == EventType.ContextClick)
			{
				CreateGenericMenu().ShowAsContext();
				Event.current.Use();
			}

			GUI.Button(new Rect(0, 1, 100, TopOffset - 1), "Open");
			GUI.Button(new Rect(100, 1, 100, TopOffset - 1), "Save");
			GUI.Button(new Rect(200, 1, 100, TopOffset - 1), "Help");

			DrawZoomArea();

			_lastMousePosition = Event.current.mousePosition;
		}


		private GenericMenu CreateGenericMenu()
		{
			GenericMenu menu = new GenericMenu();
			foreach (var nodeType in _nodeTypes) menu.AddItem(new GUIContent(nodeType), false, OnGenericMenuClick, nodeType);
			return menu;
		}

		private void OnGenericMenuClick(object item)
		{
			Node node = _controller.CreateNode((string) item);
			Vector2 position = ProjectToDrawArea(_lastMousePosition);
			node.X = position.x;
			node.Y = position.y;
			_canvas.Nodes.Add(node);
		}


		private void HandleNodeRemoving()
		{
			// Delete or Backspace
			if (Event.current.keyCode == KeyCode.Delete || Input.GetKeyDown(KeyCode.Backspace))
			{
				Node node = GetFocusedNode();
				if (node != null)
				{
					_controller.RemoveNode(node);
					_canvas.Nodes.Remove(node);
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
				float oldZoom = _canvas.Zoom;
				_canvas.Zoom = Mathf.Clamp(_canvas.Zoom + zoomDelta, CanvasZoomMin, CanvasZoomMax);
				_canvas.Position += (zoomCoordsMousePos - _canvas.Position) -
				                    (oldZoom/_canvas.Zoom)*(zoomCoordsMousePos - _canvas.Position);
				Event.current.Use();
				return;
			}

			// Translate
			if (Event.current.type == EventType.MouseDrag &&
			    (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
			    Event.current.button == 2)
			{
				Vector2 delta = Event.current.delta;
				delta /= _canvas.Zoom;
				_canvas.Position += delta;

				Event.current.Use();
				return;
			}
		}

		private void HandleDragAndDrop()
		{
			if (Event.current.type == EventType.MouseDown)
			{
				Socket target = GetSocketAt(Event.current.mousePosition);
				if (target != null && _currentDragSocket == null)
				{
					if (target.Edge == null)
					{
						_currentDragSocket = target;
					}
					else
					{
						_currentDragSocket = target.Edge.GetOtherSocket(target);
						_controller.UnLink(_currentDragSocket, target);
					}
					Event.current.Use();
				}
			}

			if (Event.current.type == EventType.MouseUp)
			{
				if (_currentDragSocket != null)
				{
					Socket target = GetSocketAt(Event.current.mousePosition);
					if (CanBeLinked(target, _currentDragSocket))
					{
						// drop edge event
						if (target.Edge != null)
						{
							// replace edge
							_controller.UnLink(target);
						}
						_controller.Link(_currentDragSocket, target);
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

		private Node GetFocusedNode()
		{
			foreach (Node node in _canvas.Nodes)
			{
				if (node.HasFocus()) return node;
			}
			return null;
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

		/// <summary> Returns the socket at the window position.</summary>
		/// <param name="windowPosition"> The position to get the Socket from in window coordinates</param>
		/// <returns>The socket at the posiiton or null or null.</returns>
		private Socket GetSocketAt(Vector2 windowPosition)
		{
			Vector2 projectedPosition = ProjectToDrawArea(windowPosition);
			foreach (Node node in _canvas.Nodes)
			{
				if (node.Intersects(projectedPosition))
				{
					Socket socket = node.SearchSocketAt(projectedPosition);
					if (socket != null)
					{
						return socket;
					}
				}
			}
			return null;
		}

		private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
		{
			return (screenCoords - _zoomArea.TopLeft())/_canvas.Zoom + _canvas.Position;
		}

		public Vector2 ProjectToDrawArea(Vector2 windowPosition)
		{
			windowPosition.y += (21) - ((TopOffset*2));
			windowPosition = windowPosition/_canvas.Zoom;
			windowPosition.x -= (_canvas.DrawArea.x);
			windowPosition.y -= (_canvas.DrawArea.y);
			return windowPosition;
		}

		private void DrawZoomArea()
		{
			EditorZoomArea.Begin(_canvas.Zoom, _zoomArea);
			_canvas.DrawArea.Set(_canvas.Position.x, _canvas.Position.y, 100000.0f, 100000.0f);
			GUILayout.BeginArea(_canvas.DrawArea, _canvas.Style);
			_canvas.DrawEdges();
			BeginWindows();
			_canvas.DrawNodes();
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



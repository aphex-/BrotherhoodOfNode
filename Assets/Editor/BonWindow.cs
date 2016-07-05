using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Assets.Code.Bon;
using Assets.Editor.Bon;
using System.Linq;


namespace Assets.Editor
{
	public class BonWindow : EditorWindow
	{
		private const string Name = "BrotherhoodOfNode";
		public const int TopOffset = 32;
		public const int BottomOffset = 20;
		public const int TopMenuHeight = 20;

		private const int TabButtonWidth = 200;
		private const int TabButtonMargin = 4;
		private const int TabCloseButtonSize = TopMenuHeight;

		private const int WindowTitleHeight = 21; // Unity issue
		private const float CanvasZoomMin = 0.1f;
		private const float CanvasZoomMax = 1.0f;

		private readonly Rect _openButtonRect = new Rect(0, 0, 80, TopMenuHeight);
		private readonly Rect _saveButtonRect = new Rect(80, 0, 80, TopMenuHeight);
		private readonly Rect _newButtonRect = new Rect(160, 0, 80, TopMenuHeight);
		private readonly Rect _helpButtonRect = new Rect(240, 0, 80, TopMenuHeight);

		private Vector2 _nextTranlationPosition = new Vector2();

		private readonly Color _tabColorUnselected = new Color(0.8f, 0.8f, 0.8f, 0.5f);
		private readonly Color _tabColorSelected = Color.white;


		private readonly BonLauncher _launcher;
		private readonly List<BonCanvas> _canvasList = new List<BonCanvas>();
		private BonCanvas _currentCanvas = null;
		private Rect _canvasRegion = new Rect();

		private Socket _currentDragSocket = null;
		private Vector2 _lastMousePosition = new Vector2();

		private readonly GenericMenu _menu;
		private readonly Dictionary<string, Type> _menuEntryToNodeType;

		private Rect _tmpRect = new Rect();

		[MenuItem("Window/" + Name)]
		public static void CreateEditor()
		{
			BonWindow window = EditorWindow.GetWindow<BonWindow>();
			// BonWindow window = CreateInstance<BonWindow>(); // to create a new window
			window.wantsMouseMove = true;
			GUIContent c = new GUIContent();
			c.text = "Graph";
			window.titleContent = c;
			window.Show();
		}

		public BonWindow()
		{
			titleContent = new GUIContent(Name);
			_launcher = new BonLauncher();
			_launcher.OnWindowOpen();


			_menuEntryToNodeType = CreateMenuEntries();
			Graph graph = _launcher.LoadGraph(BonConfig.DefaultGraphName);
			_currentCanvas = new BonCanvas(graph);
			_canvasList.Add(_currentCanvas);
			_menu = CreateGenericMenu();
		}

		/// <summary>Creates a dictonary that maps a menu entry string to a node type using reflection.</summary>
		/// <returns>Dictonary that maps a menu entry string to a node type</returns>
		public Dictionary<string, Type> CreateMenuEntries()
		{
			Dictionary<string, Type> menuEntries = new Dictionary<string, Type>();

			IEnumerable<Type> classesExtendingNode = Assembly.GetAssembly(typeof(Node)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Node)));

			foreach (Type type in classesExtendingNode) menuEntries.Add(GetItemMenuName(type), type);

			menuEntries.OrderBy(x => x.Key);
			return menuEntries;
		}

		private string GetItemMenuName(Type type)
		{
			string path = Node.GetNodePath(type);
			if (path != null) return path + "/" + Node.GetNodeName(type);
			return Node.GetNodeName(type);
		}

		/// <summary>Draws the UI</summary>
		void OnGUI()
		{
			HandleNodeRemoving();
			HandleCanvasTranslation();
			HandleDragAndDrop();

			if (Event.current.type == EventType.ContextClick)
			{
				_menu.ShowAsContext();
				Event.current.Use();
			}
			HandleMenuButtons();

			HandleTabButtons();

			if (_currentCanvas != null) {
				float infoPanelY = Screen.height - TopOffset - 6;
				_tmpRect.Set(5, infoPanelY, 55, 20);
				GUI.Label(_tmpRect, "zoom: " + Math.Round(_currentCanvas.Zoom, 1));
				_tmpRect.Set(60, infoPanelY, 70, 20);
				GUI.Label(_tmpRect, "x: " + Math.Round(_currentCanvas.Position.x));
				_tmpRect.Set(130, infoPanelY, 70, 20);
				GUI.Label(_tmpRect, "y: " + Math.Round(_currentCanvas.Position.y));
				_tmpRect.Set(200, infoPanelY, 70, 20);
				GUI.Label(_tmpRect, "nodes: " + _currentCanvas.Graph.GetNodeCount());
				_tmpRect.Set(5, Screen.height - TopOffset - 6, 100, 20);
				GUI.Label(_tmpRect, "zoom: " + Math.Round(_currentCanvas.Zoom, 1));
			}

			if (_currentCanvas != null)
			{
				_canvasRegion.Set(0, TopOffset, Screen.width, Screen.height - 2 * TopOffset - BottomOffset);
				_currentCanvas.Draw((EditorWindow) this, _canvasRegion, _currentDragSocket);
			}
			_lastMousePosition = Event.current.mousePosition;
		}

		private void HandleTabButtons()
		{
			Color standardBackgroundColor = GUI.backgroundColor;
			int tabIndex = 0;
			BonCanvas canvasToClose = null;
			foreach (BonCanvas tmpCanvas in _canvasList)
			{
				int width = TabButtonWidth + TabButtonMargin + TabCloseButtonSize;
				int xOffset = width*tabIndex;

				tmpCanvas.TabButton.Set(xOffset, TopMenuHeight + TabButtonMargin,TabButtonWidth, TopMenuHeight);
				tmpCanvas.CloseTabButton.Set(xOffset + width - TabCloseButtonSize - TabButtonMargin -4,
					TopMenuHeight + TabButtonMargin, TabCloseButtonSize, TabCloseButtonSize);

				bool isSelected = (_currentCanvas == tmpCanvas);
				string tabName;
				if (tmpCanvas.FilePath == null) tabName = "untitled";
				else tabName = Path.GetFileName(tmpCanvas.FilePath);


				if (isSelected) GUI.backgroundColor = _tabColorSelected;
				else GUI.backgroundColor = _tabColorUnselected;

				if (GUI.Button(tmpCanvas.TabButton, tabName))
				{
					SetCurrentCanvas(tmpCanvas);
				}
				if (isSelected)
				{
					if (GUI.Button(tmpCanvas.CloseTabButton, "X"))
					{
						canvasToClose = tmpCanvas;
					}
				}
				tabIndex++;
			}

			GUI.backgroundColor = standardBackgroundColor;
			if (canvasToClose != null) 	CloseCanvas(canvasToClose);

		}

		private void SetCurrentCanvas(BonCanvas canvas)
		{
			_currentCanvas = canvas;
		}

		private void CloseCanvas(BonCanvas canvas)
		{
			bool doSave = EditorUtility.DisplayDialog("Do you want to save.", "Do you want to save the graph " + canvas.FilePath + " ?",
				"Yes", "No");
			if (doSave)
			{
				if (canvas.FilePath == null) OpenSaveDialog();
				else _launcher.SaveGraph(canvas.Graph, canvas.FilePath);
			}
			_canvasList.Remove(canvas);
			if (_canvasList.Count > 0) _currentCanvas = _canvasList[0];
			else _currentCanvas = null;
		}

		private GenericMenu CreateGenericMenu()
		{
			GenericMenu m = new GenericMenu();
			foreach(KeyValuePair<string, Type> entry in _menuEntryToNodeType)
				m.AddItem(new GUIContent(entry.Key), false, OnGenericMenuClick, entry.Value);
			return m;
		}

		private void OnGenericMenuClick(object item)
		{
			if (_currentCanvas != null)
			{
				_currentCanvas.CreateNode((Type) item, _lastMousePosition);
			}
		}

		private void CreateCanvas(string path)
		{
			BonCanvas canvas;
			if (path != null) canvas = new BonCanvas(_launcher.LoadGraph(path));
			else canvas = new BonCanvas(new Graph());
			canvas.FilePath = path;
			_canvasList.Add(canvas);
			SetCurrentCanvas(canvas);
		}

		private void OpenSaveDialog()
		{
			var path = EditorUtility.SaveFilePanel("save graph data", "", "graph", "json");
			if (!path.Equals(""))
			{
				_launcher.SaveGraph(_currentCanvas.Graph, path);
				_currentCanvas.FilePath = path;
			}
		}

		private void HandleMenuButtons()
		{
			if (GUI.Button(_openButtonRect, "Open"))
			{
				var path = EditorUtility.OpenFilePanel("load graph data", "", "json");
				if (!path.Equals("")) CreateCanvas(path);
			}

			// Save Button
			if (GUI.Button(_saveButtonRect, "Save"))
			{
				OpenSaveDialog();
			}

			// New Button
			if (GUI.Button(_newButtonRect, "New"))
			{
				CreateCanvas(null);
			}

			// Help Button
			GUI.Button(_helpButtonRect, "Help");
		}

		private void HandleNodeRemoving()
		{
			// Delete or Backspace
			/*if (Event.current.keyCode == KeyCode.Delete || Input.GetKeyDown(KeyCode.Backspace))
			{
				if (currentCanvas != null)
				{
					currentCanvas.RemoveFocusedNode();
					Repaint();
				}
			}*/
		}

		private void HandleCanvasTranslation()
		{
			if (_currentCanvas == null) return;

			// Zoom
			if (Event.current.type == EventType.ScrollWheel)
			{
				Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(Event.current.mousePosition);
				float zoomDelta = -Event.current.delta.y/150.0f;
				float oldZoom = _currentCanvas.Zoom;
				_currentCanvas.Zoom = Mathf.Clamp(_currentCanvas.Zoom + zoomDelta, CanvasZoomMin, CanvasZoomMax);

				_nextTranlationPosition = _currentCanvas.Position + (zoomCoordsMousePos - _currentCanvas.Position) -
					(oldZoom/_currentCanvas.Zoom)*(zoomCoordsMousePos - _currentCanvas.Position);

				if (_nextTranlationPosition.x >= 0) _nextTranlationPosition.x = 0;
				if (_nextTranlationPosition.y >= 0) _nextTranlationPosition.y = 0;
				_currentCanvas.Position = _nextTranlationPosition;
				Event.current.Use();
				return;
			}

			// Translate
			if (Event.current.type == EventType.MouseDrag &&
			    (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
			    Event.current.button == 2)
			{
				Vector2 delta = Event.current.delta;
				delta /= _currentCanvas.Zoom;

				_nextTranlationPosition = _currentCanvas.Position + delta;
				if (_nextTranlationPosition.x >= 0) _nextTranlationPosition.x = 0;
				if (_nextTranlationPosition.y >= 0) _nextTranlationPosition.y = 0;

				_currentCanvas.Position = _nextTranlationPosition;
				Event.current.Use();
				return;
			}
		}

		private void HandleDragAndDrop()
		{
			if (_currentCanvas == null) return;

			if (Event.current.type == EventType.MouseDown)
			{
				Socket target = _currentCanvas.GetSocketAt(Event.current.mousePosition);
				if (target != null && _currentDragSocket == null)
				{
					if (target.Edge == null)
					{
						_currentDragSocket = target;
					}
					else
					{
						_currentDragSocket = target.Edge.GetOtherSocket(target);
						_currentCanvas.Graph.UnLink(_currentDragSocket, target);
					}
					Event.current.Use();
				}
			}

			if (Event.current.type == EventType.MouseUp)
			{
				if (_currentDragSocket != null)
				{
					Socket target = _currentCanvas.GetSocketAt(Event.current.mousePosition);
					if (_currentCanvas.Graph.CanBeLinked(target, _currentDragSocket))
					{
						// drop edge event
						if (target.Edge != null)
						{
							// replace edge
							_currentCanvas.Graph.UnLink(target);
						}

						if (_currentDragSocket.Direction == SocketDirection.Input)
						{
							_currentCanvas.Graph.Link(_currentDragSocket, target);
						}
						else
						{
							_currentCanvas.Graph.Link(target, _currentDragSocket);
						}

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

		private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
		{
			return (screenCoords - _canvasRegion.TopLeft())/_currentCanvas.Zoom + _currentCanvas.Position;
		}

	}
}



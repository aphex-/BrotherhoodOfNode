using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Assets.Code.Bon;
using Assets.Code.Bon.Socket;
using Assets.Editor.Bon;
using System.Linq;


namespace Assets.Editor
{
	/// <summary>
	/// This class contains the logic of the editor window. It contains canvases that
	/// are containing graphs. It uses the BonLauncher to load, save and close Graphs.
	/// </summary>
	public class BonWindow : EditorWindow
	{
		private const string Name = "BrotherhoodOfNode";
		public const int TopOffset = 32;
		public const int BottomOffset = 20;
		public const int TopMenuHeight = 20;


		private const int TabButtonWidth = 200;
		private const int TabButtonMargin = 4;
		private const int TabCloseButtonSize = TopMenuHeight;

		private const int WindowTitleHeight = 21;
		private const float CanvasZoomMin = 0.1f;
		private const float CanvasZoomMax = 1.0f;

		private  Rect _openButtonRect = new Rect(0, 0, 80, TopMenuHeight);
		private  Rect _saveButtonRect = new Rect(80, 0, 80, TopMenuHeight);
		private  Rect _newButtonRect = new Rect(160, 0, 80, TopMenuHeight);
		private  Rect _helpButtonRect = new Rect(240, 0, 80, TopMenuHeight);

		private Vector2 _nextTranlationPosition;

		private Color _tabColorUnselected = new Color(0.8f, 0.8f, 0.8f, 0.5f);
		private Color _tabColorSelected = Color.white;


		private  List<BonCanvas> _canvasList = new List<BonCanvas>();
		private BonCanvas _currentCanvas;
		private Rect _canvasRegion = new Rect();

		private AbstractSocket _dragSourceSocket = null;
		private Vector2 _lastMousePosition;

		private GenericMenu _menu;
		private Dictionary<string, Type> _menuEntryToNodeType;

		private Rect _tmpRect = new Rect();


		[MenuItem("Window/" + Name)]
		static void OnCreateWindow()
		{
			BonWindow window = GetWindow<BonWindow>();
			// BonWindow window = CreateInstance<BonWindow>(); // to create a new window
			window.Show();
		}

		public void OnEnable()
		{
			Init();
		}

		public void Init()
		{

			EditorApplication.playmodeStateChanged = OnPlaymodeStateChanged;
			// create GameObject and the Component if it is not added to the scene

			titleContent = new GUIContent(Name);
			wantsMouseMove = true;
			EventManager.TriggerOnWindowOpen();
			_menuEntryToNodeType = CreateMenuEntries();
			_menu = CreateGenericMenu();

			_canvasList.Clear();
			_currentCanvas = null;

			if (GetLauncher().Graphs.Count > 0) LoadCanvas(GetLauncher().Graphs);
			else LoadCanvas(GetLauncher().LoadGraph(BonConfig.DefaultGraphName));
			UpdateGraphs();
			Repaint();
		}

		private void OnPlaymodeStateChanged()
		{
			UpdateGraphs();
			Repaint();
		}

		private void UpdateGraphs()
		{
			foreach (var graph in GetLauncher().Graphs)
			{
				graph.ForceUpdateNodes();
			}
		}

		private void LoadCanvas(List<Graph> graphs)
		{
			foreach (var graph in graphs) LoadCanvas(graph);
		}

		private void LoadCanvas(Graph graph)
		{
			_currentCanvas = new BonCanvas(graph);
			_canvasList.Add(_currentCanvas);
		}

		/// <summary>
		/// Creates a dictonary that maps a menu entry string to a node type using reflection.
		/// </summary>
		/// <returns>
		/// Dictonary that maps a menu entry string to a node type
		/// </returns>
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


		private BonLauncher GetLauncher()
		{
			if (GameObject.Find(BonConfig.GameObjectName) == null)
			{
				new GameObject(BonConfig.GameObjectName);
				Log.Info("Created GameObject '" + BonConfig.GameObjectName + "'");
			}
			if (GameObject.Find(BonConfig.GameObjectName).GetComponent<BonLauncher>() == null)
			{
				Log.Info("Added BonLauncher component to the GameObject '" + BonConfig.GameObjectName + "'");
				GameObject.Find(BonConfig.GameObjectName).AddComponent<BonLauncher>();
			}
			return GameObject.Find(BonConfig.GameObjectName).GetComponent<BonLauncher>();
		}

		/// <summary>Draws the UI</summary>
		void OnGUI()
		{
			HandleCanvasTranslation();
			HandleDragAndDrop();

			if (Event.current.type == EventType.ContextClick)
			{
				_menu.ShowAsContext();
				Event.current.Use();
			}
			HandleMenuButtons();

			if (GetLauncher() == null) return;

			HandleTabButtons();

			if (_currentCanvas != null) {
				float infoPanelY = Screen.height - TopOffset - 6;
				_tmpRect.Set(5, infoPanelY, 70, 20);
				GUI.Label(_tmpRect, "zoom: " + Math.Round(_currentCanvas.Zoom, 1));
				_tmpRect.Set(60, infoPanelY, 70, 20);
				GUI.Label(_tmpRect, "x: " + Math.Round(_currentCanvas.Position.x));
				_tmpRect.Set(130, infoPanelY, 70, 20);
				GUI.Label(_tmpRect, "y: " + Math.Round(_currentCanvas.Position.y));
				_tmpRect.Set(200, infoPanelY, 70, 20);
				GUI.Label(_tmpRect, "nodes: " + _currentCanvas.Graph.GetNodeCount());
			}

			if (_currentCanvas != null)
			{
				_canvasRegion.Set(0, TopOffset, Screen.width, Screen.height - 2 * TopOffset - BottomOffset);
				_currentCanvas.Draw((EditorWindow) this, _canvasRegion, _dragSourceSocket);
			}
			_lastMousePosition = Event.current.mousePosition;

			Repaint();
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
				string tabName = tmpCanvas.Graph.Name;
				//tabName = Path.GetFileName(tmpCanvas.FilePath);

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
			UpdateGraphs();
			Repaint();
			if (canvas != null) EventManager.TriggerOnFocusGraph(canvas.Graph);
			_currentCanvas = canvas;
		}

		private void CloseCanvas(BonCanvas canvas)
		{
			bool doSave = EditorUtility.DisplayDialog("Do you want to save.", "Do you want to save the graph " + canvas.FilePath + " ?",
				"Yes", "No");
			if (doSave)
			{
				if (canvas.FilePath == null) OpenSaveDialog();
				else GetLauncher().SaveGraph(canvas.Graph, canvas.FilePath);
			}
			EventManager.TriggerOnCloseGraph(canvas.Graph);
			GetLauncher().RemoveGraph(canvas.Graph);
			_canvasList.Remove(canvas);
			if (_canvasList.Count > 0) SetCurrentCanvas(_canvasList[0]);
			else SetCurrentCanvas(null);
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
			if (path != null) canvas = new BonCanvas(GetLauncher().LoadGraph(path));
			else canvas = new BonCanvas(GetLauncher().LoadGraph(BonConfig.DefaultGraphName));
			canvas.FilePath = path;
			_canvasList.Add(canvas);
			SetCurrentCanvas(canvas);
		}

		private void OpenSaveDialog()
		{
			var path = EditorUtility.SaveFilePanel("save graph data", "", "graph", "json");
			if (!path.Equals(""))
			{
				GetLauncher().SaveGraph(_currentCanvas.Graph, path);
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
			if (GUI.Button(_saveButtonRect, "Save")) OpenSaveDialog();
			// New Button
			if (GUI.Button(_newButtonRect, "New")) CreateCanvas(null);
			// Help Button
			GUI.Button(_helpButtonRect, "Help");
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
			}
		}

		private void HandleSocketDrag(AbstractSocket dragSource)
		{
			if (dragSource != null)
			{
				if (dragSource.IsInput() && dragSource.IsConnected())
				{
					_dragSourceSocket = ((InputSocket) dragSource).Edge.GetOtherSocket(dragSource);
					_currentCanvas.Graph.UnLink((InputSocket) dragSource, (OutputSocket) _dragSourceSocket);
				}
				if (dragSource.IsOutput()) _dragSourceSocket = dragSource;
				Event.current.Use();
			}
			Repaint();
		}

		private void HandleSocketDrop(AbstractSocket dropTarget)
		{
			if (dropTarget != null && dropTarget.GetType() != _dragSourceSocket.GetType())
			{
				if (dropTarget.IsInput())
				{
					_currentCanvas.Graph.Link((InputSocket) dropTarget, (OutputSocket) _dragSourceSocket);
				}
				Event.current.Use();
			}
			_dragSourceSocket = null;
			Repaint();
		}

		private void HandleDragAndDrop()
		{
			if (_currentCanvas == null) return;

			if (Event.current.type == EventType.MouseDown)
			{
				HandleSocketDrag(_currentCanvas.GetSocketAt(Event.current.mousePosition));
			}

			if (Event.current.type == EventType.MouseUp && _dragSourceSocket != null)
			{
				HandleSocketDrop(_currentCanvas.GetSocketAt(Event.current.mousePosition));
			}

			if (Event.current.type == EventType.MouseDrag)
			{
				if (_dragSourceSocket != null) Event.current.Use();
			}
		}

		private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
		{
			return (screenCoords - _canvasRegion.TopLeft())/_currentCanvas.Zoom + _currentCanvas.Position;
		}

	}
}



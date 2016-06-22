using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
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
		public const int TopMenuHeight = 20;

		private const int TabButtonWidth = 200;
		private const int TabButtonMargin = 4;
		private const int TabCloseButtonSize = TopMenuHeight;

		private const int WindowTitleHeight = 21; // Unity issue
		private const float CanvasZoomMin = 0.1f;
		private const float CanvasZoomMax = 1.0f;

		private readonly Rect openButtonRect = new Rect(0, 0, 80, TopMenuHeight);
		private readonly Rect saveButtonRect = new Rect(80, 0, 80, TopMenuHeight);
		private readonly Rect newButtonRect = new Rect(160, 0, 80, TopMenuHeight);
		private readonly Rect helpButtonRect = new Rect(240, 0, 80, TopMenuHeight);

		private readonly Color TabColorUnselected = new Color(0.8f, 0.8f, 0.8f, 0.5f);
		private readonly Color TabColorSelected = Color.white;


		private BonLauncher launcher;
		private List<BonCanvas> canvasList = new List<BonCanvas>();
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
			launcher = new BonLauncher();
			launcher.OnWindowOpen();


			menuEntryToNodeType = CreateMenuEntries();
			Graph graph = launcher.LoadGraph(BonConfig.DefaultGraphName);
			currentCanvas = new BonCanvas(graph);
			canvasList.Add(currentCanvas);
			menu = CreateGenericMenu();
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
				menu.ShowAsContext();
				Event.current.Use();
			}
			HandleMenuButtons();

			HandleTabButtons();

			if (currentCanvas != null)
			{
				canvasRegion.Set(0, TopOffset, Screen.width, Screen.height);
				currentCanvas.Draw((EditorWindow) this, canvasRegion, currentDragSocket);
			}
			lastMousePosition = Event.current.mousePosition;
		}

		private void HandleTabButtons()
		{
			Color standardBackgroundColor = GUI.backgroundColor;
			int tabIndex = 0;
			BonCanvas canvasToClose = null;
			foreach (BonCanvas tmpCanvas in canvasList)
			{
				int width = TabButtonWidth + TabButtonMargin + TabCloseButtonSize;
				int xOffset = width*tabIndex;

				tmpCanvas.TabButton.Set(xOffset, TopMenuHeight + TabButtonMargin,TabButtonWidth, TopMenuHeight);
				tmpCanvas.CloseTabButton.Set(xOffset + width - TabCloseButtonSize - TabButtonMargin -4,
					TopMenuHeight + TabButtonMargin, TabCloseButtonSize, TabCloseButtonSize);

				bool isSelected = (currentCanvas == tmpCanvas);
				string tabName;
				if (tmpCanvas.FilePath == null) tabName = "untitled";
				else tabName = Path.GetFileName(tmpCanvas.FilePath);


				if (isSelected) GUI.backgroundColor = TabColorSelected;
				else GUI.backgroundColor = TabColorUnselected;

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
			currentCanvas = canvas;
		}

		private void CloseCanvas(BonCanvas canvas)
		{
			bool doSave = EditorUtility.DisplayDialog("Do you want to save.", "Do you want to save the graph " + canvas.FilePath + " ?",
				"Yes", "No");
			if (doSave)
			{
				if (canvas.FilePath == null) OpenSaveDialog();
				else launcher.SaveGraph(canvas.graph, canvas.FilePath);
			}
			canvasList.Remove(canvas);
			if (canvasList.Count > 0) currentCanvas = canvasList[0];
			else currentCanvas = null;
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
			if (currentCanvas != null)
			{
				currentCanvas.CreateNode((Type) item, lastMousePosition);
			}
		}

		private void CreateCanvas(string path)
		{
			BonCanvas canvas;
			if (path != null) canvas = new BonCanvas(launcher.LoadGraph(path));
			else canvas = new BonCanvas(new Graph());
			canvas.FilePath = path;
			canvasList.Add(canvas);
			SetCurrentCanvas(canvas);
		}

		private void OpenSaveDialog()
		{
			var path = EditorUtility.SaveFilePanel("save graph data", "", "graph", "json");
			if (!path.Equals(""))
			{
				launcher.SaveGraph(currentCanvas.graph, path);
				currentCanvas.FilePath = path;
			}
		}

		private void HandleMenuButtons()
		{
			if (GUI.Button(openButtonRect, "Open"))
			{
				var path = EditorUtility.OpenFilePanel("load graph data", "", "json");
				if (!path.Equals("")) CreateCanvas(path);
			}

			// Save Button
			if (GUI.Button(saveButtonRect, "Save"))
			{
				OpenSaveDialog();
			}

			// New Button
			if (GUI.Button(newButtonRect, "New"))
			{
				CreateCanvas(null);
			}

			// Help Button
			GUI.Button(helpButtonRect, "Help");
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
			if (currentCanvas == null) return;

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
			if (currentCanvas == null) return;

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
						currentCanvas.graph.UnLink(currentDragSocket, target);
					}
					Event.current.Use();
				}
			}

			if (Event.current.type == EventType.MouseUp)
			{
				if (currentDragSocket != null)
				{
					Socket target = currentCanvas.GetSocketAt(Event.current.mousePosition);
					if (currentCanvas.graph.CanBeLinked(target, currentDragSocket))
					{
						// drop edge event
						if (target.Edge != null)
						{
							// replace edge
							currentCanvas.graph.UnLink(target);
						}

						if (currentDragSocket.Direction == SocketDirection.Input)
						{
							currentCanvas.graph.Link(currentDragSocket, target);
						}
						else
						{
							currentCanvas.graph.Link(target, currentDragSocket);
						}

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



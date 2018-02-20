using Assets.Scripts.Enum;
using Assets.Scripts.Library;
using Assets.Scripts.Library.Action;
using Assets.Scripts.Library.Action.ModuleActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.ModuleEditor
{
	public class ModuleEditor : MonoBehaviour
	{
		public float MaxTabWidth = 120f;
		public float MinTabWidth = 80f;
		private Font _arialFont;

		public void Awake()
		{
			_arialFont = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
			CreateTabs();
		}

		private void CreateTabs()
		{
			var actionLibraryModules = ActionLibrary.ActionLibraryModules;
			float tabSize = GetTabWidth(actionLibraryModules.Count);

			float offset = 0f;
			foreach (IActionLibraryModule actionLibraryModule in actionLibraryModules)
			{
				Vector2 tabPosition = new Vector2(tabSize / 2 + offset, 0);
				string tabText = actionLibraryModule.Type.ToString();

				CreateTabPane(tabSize, tabPosition, tabText, actionLibraryModule);
				offset += tabSize;
			}
		}

		private void CreateTabPane(float tabSize, Vector2 tabPosition, string tabText, IActionLibraryModule actionLibraryModule)
		{
			GameObject tabPane = Instantiate(Resources.Load("Tab Pane") as GameObject, transform, false);
			RectTransform[] children = tabPane.GetComponentsInChildren<RectTransform>();
			RectTransform tab = children[2];
			tab.sizeDelta = new Vector2(tabSize, tab.sizeDelta.y);
			tab.anchoredPosition = tab.anchoredPosition + tabPosition;
			tab.GetComponentInChildren<Text>().text = tabText;

			CreateActionPanels(children[1], actionLibraryModule);
			CreateModulePanels(children[4], actionLibraryModule);
		}

		private void CreateModulePanels(Transform modulePaneGO, IActionLibraryModule actionLibraryModule)
		{
			for (int i = 0, offset = 0; i < 3; i++, offset += 200)
			{
				GameObject module = new GameObject("Module");
				module.transform.SetParent(modulePaneGO, false);

				Vector2 modulePos = new Vector2(75 + offset, 0);
				GameObject modulePanel = CreateModulePanel(module.transform, modulePos);
				CreateSnapPoint(TraderType.Alpha, "SnapPoint", module.transform, modulePanel.transform);
			}
		}

		private GameObject CreateModulePanel(Transform modulePane, Vector2 modulePos)
		{
			GameObject modulePanel = new GameObject("ModulePanel");
			modulePanel.transform.SetParent(modulePane, false);
			modulePanel.transform.position = new Vector2(0, modulePanel.transform.position.y) + modulePos;
			modulePanel.AddComponent<CanvasRenderer>();
			Image i = modulePanel.AddComponent<Image>();
			i.color = Color.gray;
			return modulePanel;
		}

		private void CreateActionPanels(Transform actionPane, IActionLibraryModule actionLibraryModule)
		{
			float actionOffset = 0;
			foreach (string actionName in actionLibraryModule.Actions_ByName.Keys)
			{
				Vector2 actionPos = new Vector2(75 + actionOffset, 0);
				CreateActionPanel(actionPane, actionName, actionPos, actionLibraryModule.Type);
				actionOffset += 110;
			}
		}

		private void CreateActionPanel(Transform actionPane, string actionName, Vector2 actionPos, TraderType type)
		{
			GameObject actionPanel = new GameObject("ActionPanel");
			actionPanel.transform.SetParent(actionPane, false);
			GameObject action = CreateAction(actionPanel.transform, actionName, actionPos);
			CreateEventTriggers(action);
			CreateText(action.transform, actionName);
			GameObject snapPoint = CreateSnapPoint(type, "SnapPoint: " + actionName, actionPanel.transform, action.transform, true);
			snapPoint.transform.position = new Vector2(0, snapPoint.transform.position.y) + actionPos;

			Image i = snapPoint.AddComponent<Image>();
			i.color = Color.gray;
			action.transform.SetAsLastSibling();
		}

		private GameObject CreateAction(Transform actionPane, string actionName, Vector2 actionPos)
		{
			GameObject actionPanel = new GameObject(actionName);
			actionPanel.transform.SetParent(actionPane, false);
			actionPanel.transform.position = new Vector2(0, actionPanel.transform.position.y) + actionPos;
			actionPanel.AddComponent<CanvasRenderer>();
			Image i = actionPanel.AddComponent<Image>();
			i.color = Color.red;
			return actionPanel;
		}

		private void CreateText(Transform actionPanel, string actionName)
		{
			GameObject textGO = new GameObject("Text");
			Text text = textGO.AddComponent<Text>();
			textGO.transform.SetParent(actionPanel, false);
			text.text = actionName;
			text.font = _arialFont;
			text.alignment = TextAnchor.MiddleCenter;
		}

		private void CreateEventTriggers(GameObject actionPanel)
		{
			DragableUI dragable = actionPanel.AddComponent<DragableUI>();
			EventTrigger et = actionPanel.AddComponent<EventTrigger>();
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.BeginDrag, dragable));
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.Drag, dragable));
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.EndDrag, dragable));
		}

		private EventTrigger.Entry CreateEventTriggerEntry(EventTriggerType eventTriggerType, DragableUI dragable)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = eventTriggerType;

			switch (eventTriggerType)
			{
				case EventTriggerType.BeginDrag:
					entry.callback.AddListener((eventData) => { dragable.BeginDrag(); });
					break;

				case EventTriggerType.Drag:
					entry.callback.AddListener((eventData) => { dragable.OnDrag(); });
					break;

				case EventTriggerType.EndDrag:
					entry.callback.AddListener((eventData) => { dragable.EndDrag(); });
					break;
			}

			return entry;
		}

		private GameObject CreateSnapPoint(TraderType type, string snapPointName, Transform parentTransform, Transform targetPositionTransform, bool occupied = false)
		{
			//create snapPoint GameObject
			GameObject snapPoint = new GameObject(snapPointName);

			//set parent to transform with target position
			snapPoint.AddComponent<RectTransform>();
			snapPoint.transform.SetParent(targetPositionTransform, false);

			//Create SnapPoint
			SnapPoint sp = snapPoint.AddComponent<SnapPoint>();
			sp.IsOccupied = occupied;
			//sp.Type = type;

			//Set Parent to intended transform
			snapPoint.transform.SetParent(parentTransform, true);
			return snapPoint;
		}

		private float GetTabWidth(int numTabs)
		{
			float tabSize = Screen.width / numTabs;
			return Mathf.Clamp(tabSize, MinTabWidth, MaxTabWidth);
		}
	}
}

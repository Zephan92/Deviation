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
		public void Awake()
		{
			var modTypes = ActionLibrary.ActionLibraryModules;
			float tabSize = GetTabWidth(modTypes.Count);

			float offset = 0f;
			foreach (IActionLibraryModule modType in modTypes)
			{
				GameObject prefab2 = Resources.Load("Tab Pane") as GameObject;
				GameObject pane = Instantiate(prefab2, transform, false);
				RectTransform [] children = pane.GetComponentsInChildren<RectTransform>();
				RectTransform tab = children[2];
				tab.sizeDelta = new Vector2(tabSize, tab.sizeDelta.y);
				tab.anchoredPosition = tab.anchoredPosition +  new Vector2(tabSize/2 + offset, 0);
				tab.GetComponentInChildren<Text>().text = modType.ModuleName;
				float actionOffset = 0;
				foreach(string action in modType.Actions.Keys)
				{
					GameObject actionPanel = new GameObject("ActionPanel-" + action);
					actionPanel.AddComponent<CanvasRenderer>();
					Image i = actionPanel.AddComponent<Image>();
					DragableUI dragable = actionPanel.AddComponent<DragableUI>();

					EventTrigger et = actionPanel.AddComponent<EventTrigger>();

					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.BeginDrag;
					entry.callback.AddListener((eventData) => { dragable.BeginDrag(); });
					et.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.Drag;
					entry.callback.AddListener((eventData) => { dragable.OnDrag(); });
					et.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.EndDrag;
					entry.callback.AddListener((eventData) => { dragable.EndDrag(); });
					et.triggers.Add(entry);


					GameObject textGO = new GameObject("Text");
					i.color = Color.red;
					actionPanel.transform.SetParent(pane.transform, false);
					actionPanel.transform.position = new Vector2(75 + actionOffset, actionPanel.transform.position.y);
					Text text = textGO.AddComponent<Text>();
					textGO.transform.SetParent(actionPanel.transform, false);
					text.text = action;
					Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

					text.font = ArialFont;
					text.alignment = TextAnchor.MiddleCenter;
					actionOffset += 110;
				}

				offset += tabSize;
				
			}
		}

		private float GetTabWidth(int numTabs)
		{
			float tabSize = Screen.width / numTabs;
			return Mathf.Clamp(tabSize, MinTabWidth, MaxTabWidth);
		}

		private void OnGUI()
		{
			
		}
	}
}

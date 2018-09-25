using Assets.Scripts.ModuleEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Deviation.Client.Scripts.UserInterface
{
	public class DragableUIFactory
	{
		public static DragableUI CreateDraggableUI<T>(GameObject panel, DragableUI.onEndDrag<T> onEndDrag, T type, bool copyPanel = false)
		{
			DragableUI dragable = panel.AddComponent<DragableUI>();
			EventTrigger et = panel.AddComponent<EventTrigger>();
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.BeginDrag, dragable, onEndDrag, type, copyPanel));
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.Drag, dragable, onEndDrag, type, copyPanel));
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.EndDrag, dragable, onEndDrag, type, copyPanel));
			return dragable;
		}

		private static EventTrigger.Entry CreateEventTriggerEntry<T>(EventTriggerType eventTriggerType, DragableUI dragable, DragableUI.onEndDrag<T> onEndDrag, T type, bool copyPanel = false)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry
			{
				eventID = eventTriggerType
			};

			switch (eventTriggerType)
			{
				case EventTriggerType.BeginDrag:
					entry.callback.AddListener((eventData) => { dragable.BeginDrag(copyPanel); });
					break;

				case EventTriggerType.Drag:
					entry.callback.AddListener((eventData) => { dragable.OnDrag(copyPanel); });
					break;

				case EventTriggerType.EndDrag:
					entry.callback.AddListener((eventData) => { dragable.EndDrag(onEndDrag, type, copyPanel); });
					break;
			}

			return entry;
		}
	}
}

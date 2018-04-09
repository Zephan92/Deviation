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
		public static void CreateDraggableUI<T>(GameObject panel, DragableUI.onEndDrag<T> onEndDrag, T type)
		{
			DragableUI dragable = panel.AddComponent<DragableUI>();
			EventTrigger et = panel.AddComponent<EventTrigger>();
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.BeginDrag, dragable, onEndDrag, type));
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.Drag, dragable, onEndDrag, type));
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.EndDrag, dragable, onEndDrag, type));
		}

		private static EventTrigger.Entry CreateEventTriggerEntry<T>(EventTriggerType eventTriggerType, DragableUI dragable, DragableUI.onEndDrag<T> onEndDrag, T type)
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
					entry.callback.AddListener((eventData) => { dragable.EndDrag(onEndDrag, type); });
					break;
			}

			return entry;
		}
	}
}

using Assets.Deviation.Client.Scripts.Client.Kits;
using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.Exchange.Scripts.DTO.Exchange;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.ModuleEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Match
{
	public class ClipDetailsPanel : MonoBehaviour
	{
		public IClip Clip { get; set; }
		public Transform Title;
		public Transform Actions;
		public Dictionary<string, ActionBubble> bubbles = new Dictionary<string, ActionBubble>();

		public void Awake()
		{
			Title = transform.Find("Title");
			Actions = transform.Find("Actions");
		}

		public void AddAction(IExchangeAction action, int amount = 1)
		{
			if (bubbles.ContainsKey(action.Name))
			{
				bubbles[action.Name].Plus(amount);
			}
			else
			{
				var actionBubbleGO = Instantiate(Resources.Load("ActionBubble"), Actions) as GameObject;
				ActionBubble actionBubble = actionBubbleGO.GetComponent<ActionBubble>();
				actionBubble.OnActionBubbleDestroyed += () => { RemoveAction(action); };
				actionBubble.Action = action;
				actionBubble.Set(amount);
				var drag = DragableUIFactory.CreateDraggableUI(actionBubbleGO, ValidSnapCheck, actionBubble);
				drag.OnEndDragSuccessAction += () => { Destroy(actionBubbleGO); };
				drag.OnEndDragFailureAction += () => { Destroy(actionBubbleGO); };
				bubbles.Add(action.Name, actionBubble);
			}
		}

		private bool ValidSnapCheck(SnapPoint snap, ActionBubble actionBubble)
		{
			ClipDetailsPanel component = snap.GetComponent<ClipDetailsPanel>();
			component.AddAction(actionBubble.Action, actionBubble.ActionCount);
			return component != null;
		}

		public void RemoveAction(IExchangeAction action)
		{
			bubbles.Remove(action.Name);
		}
	}
}

using Assets.Deviation.Client.Scripts.Match;
using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Assets.Scripts.ModuleEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client.Kits
{
	public class EditKitController : MonoBehaviour
	{
		public Button CloseEditKitButton;
		public VerticalScrollPanel ActionList;

		public void Awake()
		{
			var parent = GameObject.Find("EditKitUI");
			Transform closeEditKitButtonTransform = parent.transform.Find("CloseEditKitButton");
			CloseEditKitButton = closeEditKitButtonTransform.GetComponent<Button>();
			CloseEditKitButton.onClick.AddListener(() => { SaveAndCloseKit(parent); });
			ActionList = GetComponentInChildren<VerticalScrollPanel>();

			List<IExchangeAction> actions = new List<IExchangeAction>();

			actions.Add(ActionLibrary.GetActionInstance("Wall Push"));
			actions.Add(ActionLibrary.GetActionInstance("Small Projectile"));
			actions.Add(ActionLibrary.GetActionInstance("Medium Projectile"));
			actions.Add(ActionLibrary.GetActionInstance("Large Projectile"));
			actions.Add(ActionLibrary.GetActionInstance("StunField"));
			actions.Add(ActionLibrary.GetActionInstance("ShockWave"));
			actions.Add(ActionLibrary.GetActionInstance("Tremor"));
			actions.Add(ActionLibrary.GetActionInstance("Drain"));
			actions.Add(ActionLibrary.GetActionInstance("Ambush"));

			InitializeKitPanels(actions);
		}

		public void InitializeKitPanels(List<IExchangeAction> actions)
		{
			for (int i = 0; i < actions.Count; i++)
			{
				IExchangeAction action = actions[i];
				Create_ActionPanel(action, ActionList.List);
			}
			ActionList.OnListChange(actions.Count);
		}

		public GameObject Create_ActionPanel(IExchangeAction action, GameObject parent)
		{
			var actionPanel = Instantiate(Resources.Load("ActionPanel"), parent.transform) as GameObject;
			var actionDetailsPanel = actionPanel.GetComponent<ActionDetailsPanel>();
			actionDetailsPanel.UpdateActionDetails(action);
			var drag = DragableUIFactory.CreateDraggableUI(actionPanel, ValidSnapCheck, actionDetailsPanel, true);
			drag.OnCloneBeginDragAction += (clone) => { OnClone(clone, action); };
			drag.OnCloneEndDragSuccessAction += (clone) => { Destroy(clone); };
			drag.OnCloneReturnToOriginalParent += (clone) => { Destroy(clone); };
			return actionPanel;
		}

		private void OnClone(GameObject clone, IExchangeAction action)
		{
			clone.transform.SetParent(ActionList.List.transform);
			var actionDetailsPanel = clone.GetComponent<ActionDetailsPanel>();
			actionDetailsPanel.UpdateActionDetails(action);
			Destroy(clone.GetComponent<DragableUI>());
			var drag = DragableUIFactory.CreateDraggableUI(clone, ValidSnapCheck, actionDetailsPanel);
			drag.OnEndDragFailureAction += () => { Destroy(clone); };
		}

		private bool ValidSnapCheck(SnapPoint snap, ActionDetailsPanel actionPanel)
		{
			ClipDetailsPanel component = snap.GetComponent<ClipDetailsPanel>();
			component.AddAction(actionPanel.Action);
			return component != null;
		}

		private void SaveAndCloseKit(GameObject parent)
		{
			parent.SetActive(false);
		}
	}
}

using Assets.Deviation.Client.Scripts.Match;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client.Kits
{
	public class ActionBubble : MonoBehaviour
	{
		public int ActionCount;
		private IExchangeAction _action;
		public IExchangeAction Action { set { _action = value; UnexpandedActionName.text = value.Name.ToString(); } get { return _action; } }

		public UnityAction OnActionBubbleDestroyed;
		private ActionDetailsPanel ActionDetails { get; set; }

		private Transform Minimized;
		private Text UnexpandedActionName;
		private Text UnexpandedActionActionCountText;
		private Button UnexpandedActionPlusButton;
		private Button UnexpandedActionMinusButton;

		public void Awake()
		{
			Minimized = transform.Find("Minimized");
			UnexpandedActionName = Minimized.Find("ActionName").GetComponent<Text>();
			UnexpandedActionActionCountText = Minimized.Find("ActionCounter").Find("Amount").GetComponentInChildren<Text>();
			UnexpandedActionPlusButton = Minimized.Find("ActionCounter").Find("Plus").GetComponentInChildren<Button>();
			UnexpandedActionMinusButton = Minimized.Find("ActionCounter").Find("Minus").GetComponentInChildren<Button>();

			ActionCount = 1;
			UnexpandedActionActionCountText.text = ActionCount.ToString();
			//ActionDetails = GetComponentInChildren<ActionDetailsPanel>();
			UnexpandedActionPlusButton.onClick.AddListener(() => { Plus(); });
			UnexpandedActionMinusButton.onClick.AddListener(Minus);
		}

		public void Expand()
		{

		}

		public void UnExpand()
		{

		}

		public void Set(int amount)
		{
			if (0 < amount && amount <= 5)
			{
				ActionCount = amount;
				UnexpandedActionActionCountText.text = ActionCount.ToString();
			}
		}

		public void Plus(int amount  = 1)
		{
			if (ActionCount + amount <= 5)
			{
				ActionCount += amount;
				UnexpandedActionActionCountText.text = ActionCount.ToString();
			}
		}

		public void Minus()
		{
			if (ActionCount > 1)
			{
				ActionCount--;
				UnexpandedActionActionCountText.text = ActionCount.ToString();
			}
			else if (ActionCount == 1)
			{
				Destroy(gameObject);
			}
		}

		public void OnDestroy()
		{
			OnActionBubbleDestroyed?.Invoke();
		}
	}
}

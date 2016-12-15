using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Utilities
{
	public class IgnoreButtonSelect : MonoBehaviour, ISelectHandler
	{
		private Selectable _ghostButton;

		void Awake()
		{
			_ghostButton = GameObject.FindGameObjectWithTag("GhostButton").GetComponent<Selectable>();
		}

		public void OnSelect(BaseEventData eventData)
		{
			StartCoroutine(DelaySelect(_ghostButton));
		}

		private IEnumerator DelaySelect(Selectable select)
		{
			yield return new WaitForEndOfFrame();
			select.Select();
		}
	}
}

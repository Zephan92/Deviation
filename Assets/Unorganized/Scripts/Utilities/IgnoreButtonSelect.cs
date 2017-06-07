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
			//on awake find ghost button to select
			_ghostButton = GameObject.FindGameObjectWithTag("GhostButton").GetComponent<Selectable>();
		}

		//Selects button after short delay
		public void OnSelect(BaseEventData eventData)
		{
			StartCoroutine(DelaySelectCoroutine(_ghostButton));
		}

		//Delay Select Coroutine
		private IEnumerator DelaySelectCoroutine(Selectable select)
		{
			yield return new WaitForEndOfFrame();
			select.Select();
		}
	}
}

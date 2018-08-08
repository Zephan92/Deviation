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

		public void Awake()
		{
			var parent = GameObject.Find("EditKitUI");
			Transform closeEditKitButtonTransform = parent.transform.Find("CloseEditKitButton");
			CloseEditKitButton = closeEditKitButtonTransform.GetComponent<Button>();
			CloseEditKitButton.onClick.AddListener(() => { SaveAndCloseKit(parent); });
		}

		private void SaveAndCloseKit(GameObject parent)
		{
			parent.SetActive(false);
		}
	}
}

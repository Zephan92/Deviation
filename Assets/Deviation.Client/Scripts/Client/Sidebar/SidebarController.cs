using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client.Sidebar
{
	public enum SidebarMenuOptions
	{
		None,
		Notification
	}

	public class SidebarController : MonoBehaviour
	{
		public Transform Popups;
		public Transform Footer;
		public GameObject CurrentPopup;

		public GameObject NotificationPopup;

		public Button NotificationButton;

		public SidebarMenuOptions CurrentSidebarOption;


		public void Awake()
		{
			Popups = transform.Find("SidebarPopups");
			Footer = transform.Find("Footer");

			NotificationPopup = Popups.transform.Find("NotificationPopup").gameObject;
			NotificationButton = Footer.Find("Notification").GetComponent<Button>();

			NotificationButton.onClick.AddListener(() => { SwitchTab(SidebarMenuOptions.Notification); });
		}

		public void SwitchTab(SidebarMenuOptions option)
		{
			if (CurrentPopup != null)
			{
				CurrentPopup.SetActive(false);
			}

			CurrentSidebarOption = CurrentSidebarOption == option ? SidebarMenuOptions.None : option;

			switch (CurrentSidebarOption)
			{
				case SidebarMenuOptions.Notification:
					NotificationPopup.SetActive(true);
					CurrentPopup = NotificationPopup;
					break;

				case SidebarMenuOptions.None:
				default:
					CurrentPopup.SetActive(false);
					CurrentPopup = null;
					break;
			}
		}
	}
}

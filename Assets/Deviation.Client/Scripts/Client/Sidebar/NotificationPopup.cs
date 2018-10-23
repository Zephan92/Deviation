using Assets.Deviation.Client.Scripts.Client.Market;
using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Deviation.MasterServer.Scripts.Notification;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.Client.Scripts.Client.Sidebar
{
	public class NotificationPopup : MonoBehaviour
	{
		public VerticalScrollPanel Notifications;

		public void Awake()
		{
			Notifications = transform.GetComponentInChildren<VerticalScrollPanel>();
		}

		public void OnEnable()
		{
			List<ISerializablePacket> buys = ClientDataRepository.Instance.GetNotifications(NotificationType.Bought);
			foreach (ITradeItem trade in buys)
			{
				Create_Panel(NotificationType.Bought, trade, Notifications.List);
			}

			List<ISerializablePacket> sells = ClientDataRepository.Instance.GetNotifications(NotificationType.Sold);
			foreach (ITradeItem trade in sells)
			{
				Create_Panel(NotificationType.Sold, trade, Notifications.List);
			}
		}

		public GameObject Create_Panel(NotificationType type, ISerializablePacket packet, GameObject parent)
		{
			var panel = Instantiate(Resources.Load("NotificationPanel"), parent.transform) as GameObject;
			var panelDetails = panel.GetComponent<NotificationPanel>();
			panelDetails.UpdateDetails(type, packet);
			return panel;
		}
	}
}

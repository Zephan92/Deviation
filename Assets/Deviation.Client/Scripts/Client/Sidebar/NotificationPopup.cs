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
			List<ISerializablePacket> orders = ClientDataRepository.Instance.GetNotifications(NotificationType.MarketUpdate);
			foreach (ITradeItem trade in orders)
			{
				Create_Panel(NotificationType.MarketUpdate, trade, Notifications.List);
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

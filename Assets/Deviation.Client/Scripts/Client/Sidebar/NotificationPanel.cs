using Assets.Deviation.Client.Scripts.Client.Market;
using Assets.Deviation.MasterServer.Scripts.Notification;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client.Sidebar
{
	public class NotificationPanel : MonoBehaviour
	{
		private const string BOUGHT_MESSAGE = "Bought {0} {1}{2}.";
		private const string SOLD_MESSAGE = "Sold {0} {1}{2}.";

		public Text Type;
		public Text Message;
		public Button Dismiss;

		public NotificationType NotificationType;
		public ISerializablePacket Packet;

		public void Awake()
		{
			var type = transform.Find("Type");
			var message = transform.Find("Message");
			var dismiss = transform.Find("Dismiss");

			Type = type?.GetComponentInChildren<Text>();
			Message = message?.GetComponentInChildren<Text>();
			Dismiss = dismiss?.GetComponentInChildren<Button>();

			Dismiss.onClick.AddListener(DismissNotification);

			ResetDetails();
		}

		public void UpdateDetails(NotificationType type, ISerializablePacket packet)
		{
			NotificationType = type;
			Packet = packet;

			if (Type != null)
			{
				Type.text = type.ToString();
			}

			if (Message != null)
			{
				switch(type)
				{
					case NotificationType.Bought:
						ITradeItem buy = (ITradeItem) Packet;
						Message.text = string.Format(BOUGHT_MESSAGE, buy.Quantity, buy.Name, buy.Quantity > 1 ? "s" : "");
						break;

					case NotificationType.Sold:
						ITradeItem sell = (ITradeItem) Packet;
						Message.text = string.Format(SOLD_MESSAGE, sell.Quantity, sell.Name, sell.Quantity > 1 ? "s" : "");
						break;
				}

			}
		}

		public void ResetDetails()
		{
			if (Type != null)
			{
				Type.text = "";
			}

			if (Message != null)
			{
				Message.text = "";
			}
		}

		public void DismissNotification()
		{
			//tell server to stop sending notification

			Destroy(gameObject);
		}
	}
}

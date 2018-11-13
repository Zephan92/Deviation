using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Deviation.Client.Scripts.Client.Market
{
	public class ActionTradeItem : TradeItem
	{
		private IExchangeAction _action;

		public ActionTradeItem()
		{
			ResourceType = ResourceType.Action;
		}

		public ActionTradeItem(long id, string name, int price, int quantity, long playerId, OrderType orderType) 
			: base(id, name, price, quantity, playerId, ResourceType.Action, orderType)
		{
			_action = ActionLibrary.GetActionInstance(name);
		}

		public ActionTradeItem(long id, IExchangeAction action, int price, int quantity, long playerId, OrderType orderType)
			: base(id, action.Name, price, quantity, playerId, ResourceType.Action, orderType)
		{
			_action = action;
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			base.FromBinaryReader(reader);
			_action = ActionLibrary.GetActionInstance(Name);
		}
	}
}

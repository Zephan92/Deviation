using Barebones.Networking;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Deviation.Exchange
{
	public class ExchangeDataEntry
	{
		public int ExchangeId { get; set; }
		public long PlayerId { get; set; }
		public ActionModulePacket ActionGuids;
		public string CharacterGuid;

		public ExchangeDataEntry()
		{

		}

		public ExchangeDataEntry(int id, long playerId, ActionModulePacket actionGuids, Guid characterGuid)
		{
			ExchangeId = id;
			PlayerId = playerId;
			ActionGuids = actionGuids;
			CharacterGuid = characterGuid.ToString();
		}

		public ExchangeDataEntry(InitExchangePlayerPacket packet)
		{
			ExchangeId = packet.ExchangeId;
			PlayerId = packet.PlayerAccount.Id;
			ActionGuids = packet.ActionModule;
			CharacterGuid = packet.CharacterGuid.ToString();
		}

		public override string ToString()
		{
			return String.Format("ExchangeData - ExchangeId: {0}. PlayerID: {1}. ActionGuids: {2}. CharacterGuid {3}", ExchangeId, PlayerId, ActionGuids, CharacterGuid);
		}
	}

	public class ExchangeDataAccess
	{
		private Dictionary<int, List<ExchangeDataEntry>> _exchangeData = new Dictionary<int, List<ExchangeDataEntry>>();

		public List<ExchangeDataEntry> GetExchange1v1Data(int exchangeDataId)
		{
			return _exchangeData[exchangeDataId];
		}

		public ExchangeDataEntry GetExchange1v1Entry(int exchangeDataId, long playerId)
		{
			if (_exchangeData.ContainsKey(exchangeDataId))
			{
				foreach (var entry in _exchangeData[exchangeDataId])
				{
					if (entry.PlayerId == playerId)
					{
						return entry;
					}
				}
			}

			return null;
		}

		public ExchangeDataEntry CreateExchangeData(int exchangeDataId, long playerId, ActionModulePacket actionModule, Guid characterGuid)
		{
			ExchangeDataEntry exchange = new ExchangeDataEntry(exchangeDataId, playerId, actionModule, characterGuid);
			InsertExchangeData(exchange);
			return exchange;
		}

		public ExchangeDataEntry CreateExchangeData(InitExchangePlayerPacket packet)
		{
			ExchangeDataEntry exchange = new ExchangeDataEntry(packet);
			InsertExchangeData(exchange);
			return exchange;
		}

		public void DeleteExchangeData(int exchangeId)
		{
			_exchangeData.Remove(exchangeId);
		}

		private void InsertExchangeData(ExchangeDataEntry entry)
		{
			if (_exchangeData.ContainsKey(entry.ExchangeId))
			{
				_exchangeData[entry.ExchangeId].Add(entry);
			}
			else
			{
				_exchangeData.Add(entry.ExchangeId, new List<ExchangeDataEntry>() { entry });
			}
		}
	}
}

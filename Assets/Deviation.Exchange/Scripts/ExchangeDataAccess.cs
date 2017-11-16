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
		[BsonId]
		public int Id { get; set; }
		public long PlayerId { get; set; }
		public ActionModulePacket ActionGuids;
		public Guid CharacterGuid;

		public ExchangeDataEntry()
		{

		}

		public ExchangeDataEntry(int id, long playerId, ActionModulePacket actionGuids, Guid characterGuid)
		{
			Id = id;
			PlayerId = playerId;
			ActionGuids = actionGuids;
			CharacterGuid = characterGuid;
		}

		public override string ToString()
		{
			return String.Format("ExchangeData - ID: {0}. PlayerID: {1}. ActionGuids: {2}. CharacterGuid {3}",Id, PlayerId, ActionGuids, CharacterGuid);
		}
	}

	public class ExchangeDataAccess
	{
		LiteDatabase db = new LiteDatabase(@"exchangeData.db");
		LiteCollection<ExchangeDataEntry> _exchangeData;

		string collectionName = "players";

		public ExchangeDataAccess()
		{
			_exchangeData = db.GetCollection<ExchangeDataEntry>(collectionName);
		}

		public List<ExchangeDataEntry> GetExchange1v1Data(int exchangeDataId)
		{
			return _exchangeData.Find(x => x.Id == exchangeDataId).ToList();
		}

		public ExchangeDataEntry GetExchange1v1Entry(int exchangeDataId, long playerId)
		{
			return _exchangeData.FindOne(x => x.Id == exchangeDataId && x.PlayerId == playerId);
		}

		public ExchangeDataEntry CreateExchangeData(int exchangeDataId, long playerId, ActionModulePacket actionModule, Guid characterGuid)
		{
			ExchangeDataEntry exchange = new ExchangeDataEntry(exchangeDataId, playerId, actionModule, characterGuid);
			Debug.LogErrorFormat("Creating Exchange Data and Inserting into DB. {0}", exchange);
			_exchangeData.Insert(exchange);
			return exchange;
		}

		public void DeleteExchangeData(long exchangeDataId)
		{
			_exchangeData.Delete(x => x.Id == exchangeDataId);
		}
	}
}

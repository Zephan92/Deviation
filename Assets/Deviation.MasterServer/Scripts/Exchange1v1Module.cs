using UnityEngine;
using System.Collections;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using Assets.Scripts.DTO.Exchange;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.Exchange.Scripts;
using Assets.Deviation.Exchange;
using System.IO;
using Assets.Deviation.MasterServer.Scripts;
using LiteDB;

public class ActionModulePacket : SerializablePacket
{
	public Guid Q { get; set; }
	public Guid W { get; set; }
	public Guid E { get; set; }
	public Guid R { get; set; }

	public ActionModulePacket()
	{
	}

	public ActionModulePacket(Guid q, Guid w, Guid e, Guid r)
	{
		Q = q;
		W = w;
		E = e;
		R = r;
	}

	public ActionModulePacket(BsonDocument document)
	{
		Q = document["Q"];
		W = document["W"];
		E = document["E"];
		R = document["R"];
	}

	public BsonDocument ToBsonDocument()
	{
		var retVal = new BsonDocument();

		retVal.Add("Q", Q);
		retVal.Add("W", W);
		retVal.Add("E", E);
		retVal.Add("R", R);

		return retVal;
	}

	public override void ToBinaryWriter(EndianBinaryWriter writer)
	{
		writer.Write(Q.ToByteArray());
		writer.Write(W.ToByteArray());
		writer.Write(E.ToByteArray());
		writer.Write(R.ToByteArray());
	}

	public override void FromBinaryReader(EndianBinaryReader reader)
	{
		Q = new Guid(reader.ReadBytes(16));
		W = new Guid(reader.ReadBytes(16));
		E = new Guid(reader.ReadBytes(16));
		R = new Guid(reader.ReadBytes(16));
	}

	public Guid[] GetActionGuids()
	{
		return new Guid[4] { Q, W, E, R };
	}

	public override string ToString()
	{
		return  $"--ActionModulePacket--" +
				$"\nQ: {Q}" +
				$"\nW: {W}" +
				$"\nE: {E}" +
				$"\nR: {R}";
	}
}

public class ExchangePlayerPacket : SerializablePacket
{
	public long ExchangeId { get; set; }
	public string Username { get; set; }

	public ExchangePlayerPacket(){}

	public ExchangePlayerPacket(long id, string name)
	{
		ExchangeId = id;
		Username = name;
	}

	public override void ToBinaryWriter(EndianBinaryWriter writer)
	{
		writer.Write(ExchangeId);
		writer.Write(Username);
	}

	public override void FromBinaryReader(EndianBinaryReader reader)
	{
		ExchangeId = reader.ReadInt64();
		Username = reader.ReadString();
	}

	public override string ToString()
	{
		return String.Format("ExchangeDataId: {0}\nUsername: {1}", ExchangeId, Username);
	}
}

public enum ExchangePlayerOpCodes
{
	GetPlayerAccount = 0,
	GetExchangePlayerInfo = 4,
	CreateExchangeData = 8,
	CreateExchangeResultData = 12,
	GetExchangeResultData = 16
	// (OpCodes should be unique. MSF internal opCodes 
	// start from 32000, so you can use anything from 0 to 32000
}

public class Exchange1v1Module : ServerModuleBehaviour
{
	private PlayerDataAccess pda = new PlayerDataAccess();
	private ExchangeDataAccess eda = new ExchangeDataAccess();

	public override void Initialize(IServer server)
	{
		base.Initialize(server);
		Debug.Log("Exchange Player Module initialized");
		server.SetHandler((short)ExchangePlayerOpCodes.GetPlayerAccount, HandleGetPlayerAccount);
		server.SetHandler((short)ExchangePlayerOpCodes.GetExchangePlayerInfo, HandleGetExchangePlayerInfo);
		server.SetHandler((short)ExchangePlayerOpCodes.CreateExchangeData, HandleCreateExchangeData);
		server.SetHandler((short)ExchangePlayerOpCodes.CreateExchangeResultData, HandleCreateExchangeResultData);
		server.SetHandler((short)ExchangePlayerOpCodes.GetExchangeResultData, HandleGetExchangeResultData);
	}

	private void HandleGetExchangeResultData(IIncommingMessage message)
	{
		ExchangePlayerPacket player = message.Deserialize(new ExchangePlayerPacket());
		List<ExchangeResult> results = eda.GetExchangeResults(player.ExchangeId);
		ExchangeResults packet = new ExchangeResults(results[0].ExchangeId, results[0].Timestamp, results); 
		message.Respond(packet, ResponseStatus.Success);		
	}

	private void HandleGetExchangePlayerInfo(IIncommingMessage message)
	{
		ExchangePlayerPacket player = message.Deserialize(new ExchangePlayerPacket());
		ExchangeDataEntry packet = GetExchangePlayerInfo(player.Username, player.ExchangeId);
		message.Respond(packet);
	}

	private void HandleGetPlayerAccount(IIncommingMessage message)
	{
		PlayerAccount account = GetPlayerAccount(message.AsString());
		if (account != null)
		{
			message.Respond(account, ResponseStatus.Success);
		}
		else
		{
			message.Respond("Player Account Didn't Exist", ResponseStatus.Error);
		}
	}

	private void HandleCreateExchangeData(IIncommingMessage message)
	{
		ExchangeDataEntry packet = message.Deserialize(new ExchangeDataEntry());
		eda.CreateExchangeData(packet);
		message.Respond("Success", ResponseStatus.Success);
	}

	private void HandleCreateExchangeResultData(IIncommingMessage message)
	{
		ExchangeResult packet = message.Deserialize(new ExchangeResult());
		eda.CreateExchangeResult(packet);
		message.Respond("Success", ResponseStatus.Success);
	}

	private ExchangeDataEntry GetExchangePlayerInfo(string username, long exchangeDataId)
	{
		PlayerAccount playerAccount = GetPlayerAccount(username);
		
		ExchangeDataEntry data = eda.GetExchangeDataEntry(exchangeDataId, playerAccount);

		if (data != null)
		{
			return new ExchangeDataEntry(exchangeDataId, playerAccount, data.ActionGuids, data.CharacterGuid);
		}
		else
		{
			Debug.LogError("ExchangeDataEntry was empty");
			return null;
		}
	}

	private PlayerAccount GetPlayerAccount(string username)
	{
		PlayerAccount playerAccount;

		if (pda.PlayerExists(username))
		{
			playerAccount = pda.GetPlayer(username);
		}
		else
		{
			playerAccount = pda.CreatePlayer(username);
		}

		if (playerAccount != null)
		{
			return playerAccount;
		}
		else
		{
			Debug.LogError("Player Account was null");
			return null;
		}
	}
}


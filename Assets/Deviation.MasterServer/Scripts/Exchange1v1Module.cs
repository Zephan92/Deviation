using UnityEngine;
using System.Collections;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using Assets.Scripts.DTO.Exchange;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.Exchange.Scripts.Interface;
using Assets.Deviation.Exchange.Scripts;
using Assets.Deviation.Exchange;
using System.IO;

public class ActionModulePacket : SerializablePacket
{
	public Guid Q;
	public Guid W;
	public Guid E;
	public Guid R;

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
		return String.Format("Q: {0}\nW: {1}\nE: {2}\nR: {3}", Q, W, E, R);
	}
}

public class PlayerAccountPacket : SerializablePacket
{
	public long Id;
	public string Name;
	public string Alias;

	public PlayerAccountPacket()
	{

	}

	public PlayerAccountPacket(long id, string name, string alias)
	{
		Id = id;
		Name = name;
		Alias = alias;
	}

	public PlayerAccountPacket(PlayerAccount account)
	{
		Id = account.Id;
		Name = account.Name;
		Alias = account.Alias;
	}

	public override void ToBinaryWriter(EndianBinaryWriter writer)
	{
		writer.Write(Id);
		writer.Write(Name);
		writer.Write(Alias);
	}

	public override void FromBinaryReader(EndianBinaryReader reader)
	{
		Id = reader.ReadInt64();
		Name = reader.ReadString();
		Alias = reader.ReadString();
	}

	public override string ToString()
	{
		return String.Format("Id: {0}\nName: {1}\nAlias: {2}", Id, Name, Alias);
	}
}

public class ExchangePlayerPacket : SerializablePacket
{
	public int ExchangeDataId;
	public string Username;

	public ExchangePlayerPacket()
	{

	}

	public ExchangePlayerPacket(int id, string name)
	{
		ExchangeDataId = id;
		Username = name;
	}


	public override void ToBinaryWriter(EndianBinaryWriter writer)
	{
		writer.Write(ExchangeDataId);
		writer.Write(Username);
	}

	public override void FromBinaryReader(EndianBinaryReader reader)
	{
		ExchangeDataId = reader.ReadInt32();
		Username = reader.ReadString();
	}

	public override string ToString()
	{
		return String.Format("ExchangeDataId: {0}\nUsername: {1}", ExchangeDataId, Username);
	}
}

public class InitExchangePlayerPacket : SerializablePacket
{
	public int ExchangeId;
	public PlayerAccountPacket PlayerAccount;
	public Guid CharacterGuid;
	public ActionModulePacket ActionModule;

	public InitExchangePlayerPacket()
	{

	}

	public InitExchangePlayerPacket(int exchangeId, PlayerAccountPacket playerAccount, Guid characterGuid, ActionModulePacket actionModule)
	{
		ExchangeId = exchangeId;
		PlayerAccount = playerAccount;
		CharacterGuid = characterGuid;
		ActionModule = actionModule;
	}


	public override void ToBinaryWriter(EndianBinaryWriter writer)
	{
		writer.Write(ExchangeId);
		writer.Write(PlayerAccount);
		writer.Write(CharacterGuid.ToByteArray());
		writer.Write(ActionModule);
	}

	public override void FromBinaryReader(EndianBinaryReader reader)
	{
		ExchangeId = reader.ReadInt32();
		PlayerAccount = reader.ReadPacket(new PlayerAccountPacket());
		CharacterGuid = new Guid(reader.ReadBytes(16));
		ActionModule = reader.ReadPacket(new ActionModulePacket());
	}

	public void FromBytes()
	{

	}

	public override string ToString()
	{
		return String.Format("ExchangeId: {0}.\n\nPlayerAccount: {1}\nCharacterGuid: {2}\nActionModule: {3}", ExchangeId, PlayerAccount, CharacterGuid, ActionModule);
	}
}

public enum ExchangePlayerOpCodes
{
	GetPlayerAccount = 0,
	GetExchangePlayerInfo = 4,
	CreateExchangeData = 8,
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
	}

	private void HandleGetExchangePlayerInfo(IIncommingMessage message)
	{
		ExchangePlayerPacket player = message.Deserialize(new ExchangePlayerPacket());
		InitExchangePlayerPacket packet = GetExchangePlayerInfo(player.Username, player.ExchangeDataId);
		message.Respond(packet);
	}

	private void HandleGetPlayerAccount(IIncommingMessage message)
	{
		PlayerAccountPacket account = GetPlayerAccount(message.AsString());
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
		InitExchangePlayerPacket packet = message.Deserialize(new InitExchangePlayerPacket());
		eda.CreateExchangeData(packet);
		message.Respond("Success", ResponseStatus.Success);
	}

	private InitExchangePlayerPacket GetExchangePlayerInfo(string username, int exchangeDataId)
	{
		PlayerAccountPacket playerAccount = GetPlayerAccount(username);
		
		ExchangeDataEntry data = eda.GetExchange1v1Entry(exchangeDataId, playerAccount.Id);

		if (data != null)
		{
			return new InitExchangePlayerPacket(exchangeDataId, playerAccount, new Guid(data.CharacterGuid), data.ActionGuids);
		}
		else
		{
			Debug.LogError("ExchangeDataEntry was empty");
			return null;
		}
	}

	private PlayerAccountPacket GetPlayerAccount(string username)
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
			return new PlayerAccountPacket(playerAccount);
		}
		else
		{
			Debug.LogError("Player Account was null");
			return null;
		}
	}
}


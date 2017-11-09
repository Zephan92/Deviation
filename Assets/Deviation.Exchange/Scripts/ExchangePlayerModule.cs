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

public class ActionModulePacket : SerializablePacket
{
	public Guid Q;
	public Guid W;
	public Guid E;
	public Guid R;
	public Guid[] ActionGuids { get { return new Guid[4] { Q, W, E, R };} }

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

	public override string ToString()
	{
		return String.Format("Q: {0}\nW: {1}\nE: {2}\nR: {3}", Q, W, E, R);
	}
}

public class InitExchangePlayerPacket : SerializablePacket
{
	public long Id;
	public string Alias;
	public Guid CharacterGuid;
	public ActionModulePacket ActionModule;

	public InitExchangePlayerPacket()
	{

	}

	public InitExchangePlayerPacket(long id, string alias, Guid characterGuid, ActionModulePacket actionModule)
	{
		Id = id;
		Alias = alias;
		CharacterGuid = characterGuid;
		ActionModule = actionModule;
	}

	public override void ToBinaryWriter(EndianBinaryWriter writer)
	{
		writer.Write(Id);
		writer.Write(Alias);
		writer.Write(CharacterGuid.ToByteArray());
		writer.Write(ActionModule);
	}

	public override void FromBinaryReader(EndianBinaryReader reader)
	{
		Id = reader.ReadInt64();
		Alias = reader.ReadString();
		CharacterGuid = new Guid(reader.ReadBytes(16));
		ActionModule = reader.ReadPacket(new ActionModulePacket());
	}

	public override string ToString()
	{
		return String.Format("Id: {0}\nAlias: {1}\nCharacterGuid: {2}\nActionModule: {3}", Id, Alias, CharacterGuid, ActionModule);
	}
}

public enum ExchangePlayerOpCodes
{
	GetExchangePlayerInfo = 0
	// (OpCodes should be unique. MSF internal opCodes 
	// start from 32000, so you can use anything from 0 to 32000
}

public class ExchangePlayerModule : ServerModuleBehaviour
{
	private PlayerDataAccess pda = new PlayerDataAccess();

	public override void Initialize(IServer server)
	{
		base.Initialize(server);
		Debug.Log("Exchange Player Module initialized");

		server.SetHandler((short)ExchangePlayerOpCodes.GetExchangePlayerInfo, HandleGetExchangePlayerInfo);
	}

	private void HandleGetExchangePlayerInfo(IIncommingMessage message)
	{
		InitExchangePlayerPacket packet = GetExchangePlayerInfo(message.Peer);

		message.Respond(packet);
	}

	private InitExchangePlayerPacket GetExchangePlayerInfo(IPeer peer)
	{
		string username = "";
		Msf.Server.Auth.GetPeerAccountInfo(peer.Id, (info, error) => { username = info.Username; });
		var playerAccount = pda.GetPlayer(username);
		var characterGuid = GetPlayerCharacter(playerAccount.Id);
		var actionModule = GetPlayerActionModule(playerAccount.Id);
		return new InitExchangePlayerPacket(playerAccount.Id, playerAccount.Alias, characterGuid, actionModule);
	}

	private Guid GetPlayerCharacter(long id)
	{
		return Guid.NewGuid();
	}

	private ActionModulePacket GetPlayerActionModule(long id)
	{
		var q = new Guid("688b267a-fde1-4250-91a0-300aa3343147");
		var w = new Guid("dacb468b-658f-4daa-9400-cd3f005d06bd");
		var e = new Guid("d504df35-dc93-4f84-829e-01e202878341");
		var r = new Guid("36a1cf13-8b79-4800-8574-7cec0c405594");
		//this would go get the actions the player chose
		return new ActionModulePacket(q,w,e,r);
	}
}


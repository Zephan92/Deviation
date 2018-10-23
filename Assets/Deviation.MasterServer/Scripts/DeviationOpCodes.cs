using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Deviation.MasterServer.Scripts
{
	// (OpCodes should be unique. MSF internal opCodes 
	// start from 32000, so you can use anything from 0 to 32000

	public enum ExchangePlayerOpCodes
	{
		GetPlayerAccount = 0,
		GetExchangePlayerInfo = 4,
		CreateExchangeData = 8,
		CreateExchangeResultData = 12,
		GetExchangeResultData = 16,
		Login = 20,
		Logout =24,
	}

	public enum Exchange1v1MatchMakingOpCodes
	{
		RequestJoinQueue = 128,
		RequestLeaveQueue = 132,
		RequestChangeQueuePool = 136,
		RespondMatchFound = 140,
		RespondChangeQueuePool = 144,
		RequestJoinMatch = 148,
		RequestDeclineMatch = 152,
		RespondMatchReady = 156,
		RespondMatchDisbanded = 160,
		RespondRoomId = 164,
	}

	public enum MaterialBankOpCodes
	{
		GetMaterials = 256,
		GetMaterialBank = 264,
		AddMaterialsToBag = 280,
		RemoveMaterialsFromBag = 296,
	}

	public enum MarketOpCodes
	{
		Buy = 384,
		Bought = 388,
		Sell = 392,
		Sold = 396,
		Collect = 400,
		Cancel = 408,
		Canceled = 412,
		GetPlayerOrders = 416,
	}
}

using Assets.Deviation.MasterServer.Scripts.MaterialBank;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Deviation.MasterServer.Scripts
{
	public class MaterialBankDataAccess
	{
		//LiteDatabase db = new LiteDatabase(@"resourceBank.db");

		//LiteCollection<PlayerAccount> _players;

		//string collectionName = "players";

		public MaterialBankDataAccess()
		{
			//_players = db.GetCollection<PlayerAccount>(collectionName);
		}

		//public Dictionary<Material, int> GetMaterialBank()
		//{
		//	Dictionary<string, int> resourceBank_db = new Dictionary<string, int>();
		//	//get resourceBank_db

		//	Dictionary<Material, int> retVal = CreateMaterialBankFromNames(resourceBank_db);
		//	return retVal;
		//}

		//private Dictionary<Material, int> CreateMaterialBankFromNames(Dictionary<string, int> resources)
		//{
		//	Dictionary<Material, int> retVal = new Dictionary<Material, int>();

		//	foreach (string resourceName in resources.Keys)
		//	{
		//		if (MaterialLibrary.MaterialExists(resourceName))
		//		{
		//			retVal.Add(MaterialLibrary.GetMaterial(resourceName), resources[resourceName]);
		//		}
		//	}

		//	return retVal;
		//}

		//public void SetMaterialBank(int id, Dictionary<Material, int> resourceBank)
		//{
		//	Dictionary<string, int> resourceBank_db = new Dictionary<string, int>();

		//	foreach (Material resource in resourceBank.Keys)
		//	{
		//		resourceBank_db.Add(resource.Name, resourceBank[resource]);
		//	}

		//	//set resourceBank_db
		//}
	}
}

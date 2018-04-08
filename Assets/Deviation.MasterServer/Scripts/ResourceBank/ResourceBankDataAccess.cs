using Assets.Deviation.MasterServer.Scripts.ResourceBank;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Deviation.MasterServer.Scripts
{
	public class ResourceBankDataAccess
	{
		//LiteDatabase db = new LiteDatabase(@"resourceBank.db");

		//LiteCollection<PlayerAccount> _players;

		//string collectionName = "players";

		public ResourceBankDataAccess()
		{
			//_players = db.GetCollection<PlayerAccount>(collectionName);
		}

		//public Dictionary<Resource, int> GetResourceBank()
		//{
		//	Dictionary<string, int> resourceBank_db = new Dictionary<string, int>();
		//	//get resourceBank_db

		//	Dictionary<Resource, int> retVal = CreateResourceBankFromNames(resourceBank_db);
		//	return retVal;
		//}

		//private Dictionary<Resource, int> CreateResourceBankFromNames(Dictionary<string, int> resources)
		//{
		//	Dictionary<Resource, int> retVal = new Dictionary<Resource, int>();

		//	foreach (string resourceName in resources.Keys)
		//	{
		//		if (ResourceLibrary.ResourceExists(resourceName))
		//		{
		//			retVal.Add(ResourceLibrary.GetResource(resourceName), resources[resourceName]);
		//		}
		//	}

		//	return retVal;
		//}

		//public void SetResourceBank(int id, Dictionary<Resource, int> resourceBank)
		//{
		//	Dictionary<string, int> resourceBank_db = new Dictionary<string, int>();

		//	foreach (Resource resource in resourceBank.Keys)
		//	{
		//		resourceBank_db.Add(resource.Name, resourceBank[resource]);
		//	}

		//	//set resourceBank_db
		//}
	}
}

using Deviation.Data.LootPool;
using Deviation.Data.Resource;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Http;

namespace Deviation.Server.Controllers
{
	public interface ILootPoolAPIController
	{
		string GetLoot();
		string GetLootPool();
	}

	public class LootPoolAPIController : ApiController, ILootPoolAPIController
	{
		private ILootPool _pool;

		public LootPoolAPIController(ILootPool pool)
		{
			_pool = pool;
		}

		[HttpGet]
		[Route("api/lootpool/getloot")]
		public string GetLoot()
		{
			return _pool.GetLoot().Name;
		}

		[HttpGet]
		[Route("api/lootpool/getlootpool")]
		public string GetLootPool()
		{
			Dictionary<IResource,int> pool = _pool.GetPool();
			Dictionary<string, int> sanitizedPool = new Dictionary<string, int>();
			dynamic retval = new { };
			foreach (IResource resource in pool.Keys)
			{
				sanitizedPool.Add(resource.Name, pool[resource]);
			}
			return JsonConvert.SerializeObject(sanitizedPool);
		}

		//[HttpDelete]
		//public string DeleteEmpDetails(string id)
		//{ 
		//	return "Employee details deleted having Id " + id;

		//}
		//[HttpPut]
		//public string UpdateEmpDetails(string Name, String Id)
		//{
		//	return "Employee details Updated with Name " + Name + " and Id " + Id;

		//}
	}
}

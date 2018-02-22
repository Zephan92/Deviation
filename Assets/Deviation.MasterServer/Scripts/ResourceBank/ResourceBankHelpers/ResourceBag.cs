using System.Collections.Generic;
using System.Linq;

namespace Assets.Deviation.MasterServer.Scripts.ResourceBank
{
	public class ResourceBag
	{
		public Dictionary<Resource, int> Resources;
		public Dictionary<Resource, int> History;

		public ResourceBag()
		{
			Resources = new Dictionary<Resource, int>();
			History = new Dictionary<Resource, int>();
		}

		public ResourceBag(Dictionary<Resource, int> resources)
		{
			Resources = resources;
			History = resources;
		}

		public ResourceBag(Dictionary<Resource, int> resources, Dictionary<Resource, int> history)
		{
			Resources = resources;
			History = history;
		}

		public void AddResource(Resource resource, int count = 1)
		{
			if (Resources.ContainsKey(resource))
			{
				Resources[resource] += count;
			}
			else
			{
				Resources.Add(resource, count);
			}

			if (History.ContainsKey(resource))
			{
				History[resource] += count;
			}
			else
			{
				History.Add(resource, count);
			}
		}
		public void AddResource(Dictionary<Resource, int> resources)
		{
			foreach (Resource resource in resources.Keys)
			{
				AddResource(resource, resources[resource]);
			}
		}

		public void RemoveResource(Resource resource, int count = 1)
		{
			if (Resources.ContainsKey(resource))
			{
				Resources[resource] -= count;
			}
		}

		public void RemoveResource(Dictionary<Resource, int> resources)
		{
			foreach (Resource resource in resources.Keys)
			{
				RemoveResource(resource, resources[resource]);
			}
		}

		public int ResourceCount()
		{
			return Resources.Values.Sum();
		}

		public int HistoryCount()
		{
			return History.Values.Sum();
		}

		public int ResourceCount(Resource resource)
		{
			if (Resources.ContainsKey(resource))
			{
				return Resources[resource];
			}
			else
			{
				return 0;
			}
		}

		public int HistoryCount(Resource resource)
		{
			if (History.ContainsKey(resource))
			{
				return History[resource];
			}
			else
			{
				return 0;
			}
		}

		public string HistoryToString()
		{
			string retval = "ResourceBag\n";

			foreach (Resource resource in History.Keys)
			{
				retval += resource.Name + ": " + History[resource] + "\n";
			}

			return retval;
		}

		public override string ToString()
		{
			string retval = "ResourceBag\n";

			foreach (Resource resource in Resources.Keys)
			{
				retval += resource.Name + ": " + Resources[resource] + "\n";
			}

			return retval;
		}
	}
}

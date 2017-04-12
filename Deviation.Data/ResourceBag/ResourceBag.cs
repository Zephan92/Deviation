using System.Collections.Generic;
using Deviation.Data.Resource;

namespace Deviation.Data.ResourceBag
{
	public class ResourceBag : IResourceBag
	{
		public Dictionary<IResource, int> Resources { get; set;}

		public ResourceBag()
		{
			Resources = new Dictionary<IResource, int>();
		}

		public void AddResource(string resourceName, int resourceCount = 1)
		{
			IResource resource = ResourceLibrary.GetResourceInstance(resourceName);
			AddResource(resource, resourceCount);
		}

		public void AddResource(IResource resource, int resourceCount = 1)
		{
			if (Resources.ContainsKey(resource))
			{
				Resources[resource] += resourceCount;
			}
			else
			{
				Resources.Add(resource, resourceCount);
			}
		}

		public bool TryAddResource(string resourceName, int resourceCount = 1)
		{
			IResource resource = ResourceLibrary.GetResourceInstance(resourceName);
			return TryAddResource(resource, resourceCount);
		}

		public bool TryAddResource(IResource resource, int resourceCount = 1)
		{
			if (GetResourceCount(resource) - resourceCount >= 0)
			{
				AddResource(resource, resourceCount);
				return true;
			}
			else
			{
				return false;
			}
		}

		public int GetResourceCount(string resourceName)
		{
			IResource resource = ResourceLibrary.GetResourceInstance(resourceName);
			return GetResourceCount(resource);
		}

		public int GetResourceCount(IResource resource)
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

		public string [] ToStringArray()
		{
			string [] retVal = new string [Resources.Count];
			int i = 0;
			foreach (IResource resource in Resources.Keys)
			{
				retVal[i] += resource.Name + ": " + Resources[resource];
				i++;
			}
			return retVal;
		}
	}
}

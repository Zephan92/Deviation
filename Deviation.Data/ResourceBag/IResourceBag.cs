﻿using Deviation.Data.Resource;
using System.Collections.Generic;

namespace Deviation.Data.ResourceBag
{
	public interface IResourceBag
	{
		Dictionary<IResource, int> Resources { get; set; }
		void AddResource(string resourceName, int resourceCount = 1);
		void AddResource(IResource resource, int resourceCount = 1);
		bool TryAddResource(string resourceName, int resourceCount = 1);
		bool TryAddResource(IResource resource, int resourceCount = 1);
		int GetResourceCount(string resourceName);
		int GetResourceCount(IResource resourceName);
		string[] ToStringArray();
	}
}

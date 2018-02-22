using Assets.Deviation.MasterServer.Scripts.ResourceBank;
using Barebones.MasterServer;
using Barebones.Networking;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts
{
	public enum ResourceBankOpCodes
	{
		GetResources = 256,
		GetResourceBank = 264,
		AddResourcesToBag = 280,
		RemoveResourcesFromBag = 296,
	}

	public class ResourceBankModule : ServerModuleBehaviour
	{
		private ResourceBankObject _currentResourceBank;
		private ResourceBankDataAccess rbda = new ResourceBankDataAccess();
		private ResourceBankFactory resourceBankFactory = new ResourceBankFactory();

		public override void Initialize(IServer server)
		{
			Debug.Log("Resource Bank Module initialized");
			base.Initialize(server);

			_currentResourceBank = resourceBankFactory.Create(new ResourceBankTypeGeneral());

			server.SetHandler((short)ResourceBankOpCodes.GetResources, HandleGetResources);
			server.SetHandler((short)ResourceBankOpCodes.GetResourceBank, HandleGetResourceBank);
		}

		private void HandleGetResources(IIncommingMessage message)
		{
			//raritystart
			//take in a modifier (if winner get higher chance of good item)
			//number of resources to get
			//server only
			int count = 1;
			Rarity rarityStart = Rarity.Common;
			int modifier = 0;

			var resourcesDict = new Dictionary<Resource, int>();

			for (int i = 0; i < count; i++)
			{
				var resource = GetResource(rarityStart, modifier);

				if (resourcesDict.ContainsKey(resource))
				{
					resourcesDict[resource] += 1;
				}
				else
				{
					resourcesDict.Add(resource, 1);
				}
			}
			ResourcesPacket packet = new ResourcesPacket(resourcesDict);
			message.Respond(packet, statusCode: ResponseStatus.Success);
		}

		private void HandleGetResourceBank(IIncommingMessage message)
		{
			//request specific resource/rarity/all
			//public

			ResourceBankPacket packet = new ResourceBankPacket(_currentResourceBank);
			message.Respond(packet, statusCode: ResponseStatus.Success);
		}

		private void HandleAddResources(IIncommingMessage message)
		{
			//player account
			//server only
			ResourcesPacket packet = message.Deserialize(new ResourcesPacket());

		}

		private void HandleRemoveResourcesFromBag(IIncommingMessage message)
		{
			//player account
			//player auth or server
			ResourcesPacket packet = message.Deserialize(new ResourcesPacket());

		}

		private Resource GetResource(Rarity rarityStart = Rarity.Common, int modifier = 0)
		{
			if (_currentResourceBank.Empty())
			{
				_currentResourceBank = resourceBankFactory.Create(new ResourceBankTypeGeneral());
			}

			return _currentResourceBank.GetRandomResource(rarityStart, modifier);
		}
	}
}

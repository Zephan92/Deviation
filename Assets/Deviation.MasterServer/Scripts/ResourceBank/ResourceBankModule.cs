using Assets.Deviation.MasterServer.Scripts.ResourceBank;
using Barebones.MasterServer;
using Barebones.Networking;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts
{
	public enum ResourceBankOpCodes
	{
		GetResource = 256,
		GetResourceBank = 264
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

			server.SetHandler((short)ResourceBankOpCodes.GetResource, HandleGetResource);
			server.SetHandler((short)ResourceBankOpCodes.GetResourceBank, HandleGetResourceBank);
		}

		private void HandleGetResource(IIncommingMessage message)
		{
			//probably should make sure whoever is asking for this is allowed
			var resource = GetResource();
			ResourcePacket packet = new ResourcePacket(resource);
			message.Respond(packet, statusCode: ResponseStatus.Success);
		}

		private void HandleGetPlayerResources(IIncommingMessage message)
		{
			//probably should make sure the user is who they say they are
			//ResourceBankPacket packet = new ResourceBankPacket(_currentResourceBank);
			//message.Respond(packet, statusCode: ResponseStatus.Success);
		}

		private void HandleGetResourceBank(IIncommingMessage message)
		{
			ResourceBankPacket packet = new ResourceBankPacket(_currentResourceBank);
			message.Respond(packet, statusCode: ResponseStatus.Success);
		}

		private Resource GetResource()
		{
			if (_currentResourceBank.Empty())
			{
				_currentResourceBank = resourceBankFactory.Create(new ResourceBankTypeGeneral());
			}

			return _currentResourceBank.GetResource();
		}
	}
}

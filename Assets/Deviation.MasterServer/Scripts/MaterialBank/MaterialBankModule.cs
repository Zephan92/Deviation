using Assets.Deviation.MasterServer.Scripts.MaterialBank;
using Assets.Deviation.Materials;
using Barebones.MasterServer;
using Barebones.Networking;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts
{
	public enum MaterialBankOpCodes
	{
		GetMaterials = 256,
		GetMaterialBank = 264,
		AddMaterialsToBag = 280,
		RemoveMaterialsFromBag = 296,
	}

	public class MaterialBankModule : ServerModuleBehaviour
	{
		private MaterialBankObject _currentMaterialBank;
		//private MaterialBankDataAccess rbda = new MaterialBankDataAccess();
		private MaterialBankFactory materialBankFactory = new MaterialBankFactory();

		public override void Initialize(IServer server)
		{
			Debug.Log("Material Bank Module initialized");
			base.Initialize(server);

			_currentMaterialBank = materialBankFactory.Create(new MaterialBankTypeGeneral());

			server.SetHandler((short)MaterialBankOpCodes.GetMaterials, HandleGetMaterials);
			server.SetHandler((short)MaterialBankOpCodes.GetMaterialBank, HandleGetMaterialBank);
		}

		private void HandleGetMaterials(IIncommingMessage message)
		{
			//raritystart
			//take in a modifier (if winner get higher chance of good item)
			//number of materials to get
			//server only
			int count = 1;
			Rarity rarityStart = Rarity.Common;
			int modifier = 0;

			var materialsDict = new Dictionary<Materials.Material, int>();

			for (int i = 0; i < count; i++)
			{
				var material = GetMaterial(rarityStart, modifier);

				if (materialsDict.ContainsKey(material))
				{
					materialsDict[material] += 1;
				}
				else
				{
					materialsDict.Add(material, 1);
				}
			}
			MaterialsPacket packet = new MaterialsPacket(materialsDict);
			message.Respond(packet, statusCode: ResponseStatus.Success);
		}

		private void HandleGetMaterialBank(IIncommingMessage message)
		{
			//request specific material/rarity/all
			//public

			MaterialBankPacket packet = new MaterialBankPacket(_currentMaterialBank);
			message.Respond(packet, statusCode: ResponseStatus.Success);
		}

		private void HandleAddMaterials(IIncommingMessage message)
		{
			//player account
			//server only
			//MaterialsPacket packet = message.Deserialize(new MaterialsPacket());

		}

		private void HandleRemoveMaterialsFromBag(IIncommingMessage message)
		{
			//player account
			//player auth or server
			//MaterialsPacket packet = message.Deserialize(new MaterialsPacket());

		}

		private Materials.Material GetMaterial(Rarity rarityStart = Rarity.Common, int modifier = 0)
		{
			if (_currentMaterialBank.Empty())
			{
				_currentMaterialBank = materialBankFactory.Create(new MaterialBankTypeGeneral());
			}

			return _currentMaterialBank.GetRandomMaterial(rarityStart, modifier);
		}
	}
}

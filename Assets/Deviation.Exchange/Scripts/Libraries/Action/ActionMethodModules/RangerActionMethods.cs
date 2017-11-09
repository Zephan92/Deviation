using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Exchange.Attacks;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class RangerActions : MonoBehaviour
	{
		public static readonly Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>>
		{
			{"SmallProjectile",
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					attack.InitiateAttack(new List<IExchangePlayer>{ player}, AttackAlignment.Allies );

					System.Action<GameObject> onStartMethod = delegate(GameObject actionGO)
					{
						var mover = actionGO.GetComponent<ActionObjectMover>();
						mover.Init(player.Mover.CurrentCoordinate, 10);
					};

					System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate(Collider other, GameObject actionGO, IAttack actionAttack)
					{
						if(other.tag == "Player")
						{
							IExchangePlayer otherPlayer = other.GetComponent<IExchangePlayer>();
							if(!otherPlayer.Equals(player))
							{
								actionAttack.InitiateAttack(new List<IExchangePlayer>{ otherPlayer}, AttackAlignment.Enemies );
								actionGO.GetComponent<ActionObject>().DisableRenderer();
								actionGO.GetComponent<ActionObjectMover>().StopObject();
								Destroy(actionGO, 1f);
							}
						}
					};

					bc.SpawnActionObject(0.0f, 10f, "Rocket", player.Position, attack,
						rotation: Quaternion.Euler(player.Rotation.eulerAngles),
						onStartAction: onStartMethod,
						onTriggerAction: onTriggerEnterMethod);
				}
			},
			{"MediumProjectile",
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					attack.InitiateAttack(new List<IExchangePlayer>{ player}, AttackAlignment.Allies );

					System.Action<GameObject> onStartMethod = delegate(GameObject actionGO)
					{
						var mover = actionGO.GetComponent<ActionObjectMover>();
						mover.Init(player.Mover.CurrentCoordinate, 4);
					};

					System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate(Collider other, GameObject actionGO, IAttack actionAttack)
					{
						if(other.tag == "Player")
						{
							IExchangePlayer otherPlayer = other.GetComponent<IExchangePlayer>();
							if(!otherPlayer.Equals(player))
							{
								actionAttack.InitiateAttack(new List<IExchangePlayer>{ otherPlayer}, AttackAlignment.Enemies );
								actionGO.GetComponent<ActionObject>().DisableRenderer();
								actionGO.GetComponent<ActionObjectMover>().StopObject();
								Destroy(actionGO, 1f);
							}
						}
					};

					bc.SpawnActionObject(0.0f, 4f, "Rocket", player.Position, attack,
						rotation: Quaternion.Euler(player.Rotation.eulerAngles),
						onStartAction: onStartMethod,
						onTriggerAction: onTriggerEnterMethod);
				}
			},
			{"LargeProjectile",
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					attack.InitiateAttack(new List<IExchangePlayer>{ player}, AttackAlignment.Allies );

					System.Action<GameObject> onStartMethod = delegate(GameObject actionGO)
					{
						var mover = actionGO.GetComponent<ActionObjectMover>();
						mover.Init(player.Mover.CurrentCoordinate, 2);
					};

					System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate(Collider other, GameObject actionGO, IAttack actionAttack)
					{
						if(other.tag == "Player")
						{
							IExchangePlayer otherPlayer = other.GetComponent<IExchangePlayer>();
							if(!otherPlayer.Equals(player))
							{
								actionAttack.InitiateAttack(new List<IExchangePlayer>{ otherPlayer}, AttackAlignment.Enemies );
								actionGO.GetComponent<ActionObject>().DisableRenderer();
								actionGO.GetComponent<ActionObjectMover>().StopObject();
								Destroy(actionGO, 1f);
							}
						}
					};

					bc.SpawnActionObject(0.0f, 15f, "Rocket", player.Position, attack,
						rotation: Quaternion.Euler(player.Rotation.eulerAngles),
						onStartAction: onStartMethod,
						onTriggerAction: onTriggerEnterMethod);
				}
			},
		};
	}
}

using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class RangerActions : MonoBehaviour
	{
		public static readonly Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>>
		{
			{"FireRocket", //this method instantiates a rocket and launches it at the opponent
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
				//	IPlayer enemy = player.Enemies[0];
				//	attack.Attacker = player;
				//	attack.Defender = enemy;
				//	attack.InitiateRecoil();

				//	System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate(Collider other, GameObject projectile, IAttack atk)
				//	{
				//		//if this objects hit a player attack it
				//		if (other.tag.Equals("Player") || other.tag.Equals("MainPlayer"))
				//		{
				//			atk.SetDefender(other.GetComponent<Player>());
				//			atk.InitiateDrain();
				//			Destroy(projectile);
				//		}
				//	};

				//	System.Action<GameObject> onStartAction = delegate(GameObject projectile)
				//	{
				//		projectile.GetComponent<Rigidbody>().velocity = 2 * new Vector3(0, 0, 15);
				//	};

				//	System.Action<GameObject> updateAction = delegate(GameObject projectile)
				//	{
				//		if (projectile.transform.position.y <= -15.0)
				//		{
				//			Destroy(projectile);
				//		}
				//	};

				//	bc.SpawnProjectile(5f, "Rocket", player.Transform.position + new Vector3(0,0,1), player.Transform.rotation, attack, onTriggerEnterMethod, onStartAction, updateAction);
				}
			},
		};
	}
}

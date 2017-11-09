using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class MageActions : MonoBehaviour
	{
		public static readonly Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>>
		{
			{"PortalAttack", //this method instantiates a portal behind the opponent and launches 3 rockets
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					//IPlayer enemy = player.Enemies[0];
					//attack.Attacker = player;
					//attack.Defender = enemy;
					//attack.InitiateRecoil();
					//Vector3 origin = ActionUtilities.FindOrigin(enemy);
					//float originZ = origin.z;
					//Vector3 portalLocation = new Vector3(player.Transform.position.x, 0, originZ + 3);
					//GameObject portal = (GameObject)Instantiate(Resources.Load("Portal"), portalLocation, player.Transform.rotation);
					//bc.DeleteAfterTimeout(1f, portal);

					//System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate (Collider other, GameObject projectile, IAttack atk)
					//{
					//	//if this objects hit a player attack it
					//	if (other.tag.Equals("Player") || other.tag.Equals("MainPlayer"))
					//	{
					//		atk.SetDefender(other.GetComponent<Player>());
					//		atk.InitiateDrain();
					//		Destroy(projectile);
					//	}
					//};

					//System.Action<GameObject> onStartAction = delegate (GameObject projectile)
					//{
					//	projectile.GetComponent<Rigidbody>().velocity = 2 * new Vector3(0, 0, -15);
					//};


					//bc.SpawnProjectileAfterTimeout(0.25f,4f, "PortalRocket", portalLocation, player.Transform.rotation, attack, onTriggerEnterMethod, onStartAction);
					//bc.SpawnProjectileAfterTimeout(0.5f, 4f, "PortalRocket", portalLocation, player.Transform.rotation, attack, onTriggerEnterMethod, onStartAction);
					//bc.SpawnProjectileAfterTimeout(0.75f, 4f, "PortalRocket", portalLocation, player.Transform.rotation, attack, onTriggerEnterMethod, onStartAction);
				}
			},
			{"Teleport", //this method teleports the player 2 steps in the direction they want
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					//IPlayer enemy = player.Enemies[0];
					//attack.Attacker = player;
					//attack.Defender = enemy;
					//attack.InitiateRecoil();
					//Direction dir = Direction.None;

					//if (Input.GetKey(KeyCode.W))
					//{
					//	dir = Direction.Up;
					//}
					//else if (Input.GetKey(KeyCode.S))
					//{
					//	dir = Direction.Down;
					//}
					//else if (Input.GetKey(KeyCode.A))
					//{
					//	dir = Direction.Left;
					//}
					//else if (Input.GetKey(KeyCode.D))
					//{
					//	dir = Direction.Right;
					//}

					//if (dir != Direction.None)
					//{
					//	player.MoveObject(dir, 2);
					//}
				}
			}
		};
	}
}

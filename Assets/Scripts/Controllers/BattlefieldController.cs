using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Interface;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
	public class BattlefieldController : MonoBehaviour
	{
		public static bool[,,] Battlefields;
		public GameObject[] PlayerObjects;
		public GameObject MainPlayerObject;

		public void Awake()
		{
			if (MainPlayerObject == null)
			{
				MainPlayerObject = GameObject.FindGameObjectWithTag("MainPlayer");
			}

			PlayerObjects = GameObject.FindGameObjectsWithTag("Player");

			InitializeBattlefields(ExchangeController.NumberOfPlayers);
			AssignBattlefields();
		}

		private void InitializeBattlefields(int numBattlefields)
		{
			if (Battlefields == null)
			{
				Battlefields = new bool[numBattlefields, 5, 5];
				Battlefields.Initialize();
			}
			else
			{
				Debug.LogWarning("Trying to initialize Battlefields, but Battlefields is already initialized.");
			}
			
		}

		public void SetBattlefieldState(Battlefield field, int row, int column, bool state)
		{
			if (field == Battlefield.One)
			{
				Battlefields[0, row, column] = state;
			}
			else if (field == Battlefield.Two)
			{
				Battlefields[1, row, column] = state;
			}
		}

		public void SetBattlefieldStateAfterTimout(float timeout, Battlefield field, int row, int column, bool state)
		{
			StartCoroutine(SetBattlefieldStateAfterTimoutCoroutine(timeout, field, row,  column, state));
		}

		private IEnumerator SetBattlefieldStateAfterTimoutCoroutine(float timeout, Battlefield field, int row, int column, bool state)
		{
			yield return new WaitForSeconds(timeout);
			if (field == Battlefield.One)
			{
				Battlefields[0, row, column] = state;
			}
			else if (field == Battlefield.Two)
			{
				Battlefields[1, row, column] = state;
			}
		}

		public bool GetBattlefieldState(Battlefield field, int row, int column)
		{
			if (column < 0 || column > 4 || row < 0 || row > 4)
			{
				return false;
			}

			bool state = false;
			if (field == Battlefield.One)
			{
				state = Battlefields[0, row, column];
			}
			else if (field == Battlefield.Two)
			{
				state = Battlefields[1, row, column];
			}
			else
			{
				throw new ApplicationException("The battlefield inputed has not been implemented yet. Battlefield was: " + field);
			}

			return state;
		}

		private void AssignBattlefields()
		{
			foreach (GameObject playerObject in PlayerObjects)
			{
				Player player = playerObject.GetComponent<Player>();

				switch (player.CurrentBattlefield)
				{
					case Battlefield.One:
						if (PlayerObjects[0] == null)
						{
							PlayerObjects[0] = playerObject;
						}
						break;
					//case Battlefield.Two:
					//	if (PlayerObjects[1] == null)
					//	{
					//		PlayerObjects[1] = playerObject;
					//	}
					//	break;
					//case Battlefield.Three:
					//	if (PlayerObjects[2] == null)
					//	{
					//		PlayerObjects[2] = playerObject;
					//	}
					//	break;
					//case Battlefield.Four:
					//	if (PlayerObjects[3] == null)
					//	{
					//		PlayerObjects[3] = playerObject;
					//	}
					//	break;
				}
			}
		}

		public void DeleteAfterTimeout(float timeout, GameObject gameObjects)
		{
			DeleteAfterTimeout(timeout, new GameObject[] { gameObjects });
		}

		public void DeleteAfterTimeout(float timeout, GameObject[] battlefieldObjects)
		{
			StartCoroutine(DeleteAfterTimeoutCoroutine(timeout, battlefieldObjects));
		}

		public void SpawnAfterTimeout(float timeout, float deletionTimeout, string resourceName, Attack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
			StartCoroutine(SpawnAfterTimeoutCoroutine(timeout, deletionTimeout, resourceName, attack, attackType, zone, rotation));
		}

		public void Spawn(float deletionTimeout, string resourceName, Attack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
			GameObject go = (GameObject)Instantiate(Resources.Load(resourceName), zone, rotation);
			
			IExchangeAttack attackScript = go.GetComponent(attackType) as IExchangeAttack;
			if (attackScript == null)
				attackScript = go.GetComponentInChildren(attackType) as IExchangeAttack;
			attackScript.SetAttack(attack);
			DeleteAfterTimeout(deletionTimeout, go);
		}

		private IEnumerator DeleteAfterTimeoutCoroutine(float timeout, GameObject[] battlefieldObjects)
		{
			yield return new WaitForSeconds(timeout);
			foreach (GameObject battlefieldObject in battlefieldObjects)
			{
				Destroy(battlefieldObject);
			}
		}

		private IEnumerator SpawnAfterTimeoutCoroutine(float timeout, float deletionTimeout, string resourceName, Attack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
			yield return new WaitForSeconds(timeout);
			GameObject go = (GameObject)Instantiate(Resources.Load(resourceName), zone, rotation);

			IExchangeAttack attackScript = go.GetComponent(attackType) as IExchangeAttack;
			if (attackScript == null)
				attackScript = go.GetComponentInChildren(attackType) as IExchangeAttack;
			attackScript.SetAttack(attack);
			DeleteAfterTimeout(deletionTimeout, go);
		}
	}
}

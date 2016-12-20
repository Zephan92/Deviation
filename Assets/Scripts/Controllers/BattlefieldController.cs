using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Interface;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
	//this is a controller for the battlefield
	public class BattlefieldController : MonoBehaviour
	{
		public GameObject[] PlayerObjects;
		public GameObject MainPlayerObject;

		private bool[,,] _battlefields;

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

		//this method initializes the battlefields
		private void InitializeBattlefields(int numBattlefields)
		{
			if (_battlefields == null)
			{
				_battlefields = new bool[numBattlefields, 5, 5];
				_battlefields.Initialize();
			}
			else
			{
				Debug.LogWarning("Trying to initialize Battlefields, but Battlefields is already initialized.");
			}
			
		}

		//sets the specified battlefield state of a particular cell
		public void SetBattlefieldState(Battlefield field, int row, int column, bool state)
		{
			if (field == Battlefield.One)
			{
				_battlefields[0, row, column] = state;
			}
			else if (field == Battlefield.Two)
			{
				_battlefields[1, row, column] = state;
			}
		}

		//sets the specifed battlefield state of a particular cell after a period of time
		public void SetBattlefieldStateAfterTimout(float timeout, Battlefield field, int row, int column, bool state)
		{
			StartCoroutine(SetBattlefieldStateAfterTimoutCoroutine(timeout, field, row,  column, state));
		}

		//coroutine for battlefield state
		private IEnumerator SetBattlefieldStateAfterTimoutCoroutine(float timeout, Battlefield field, int row, int column, bool state)
		{
			yield return new WaitForSeconds(timeout);
			SetBattlefieldState(field, row, column, state);
		}
		
		//returns the battlefield state from the specified battlefield cell
		public bool GetBattlefieldState(Battlefield field, int row, int column)
		{
			if (column < 0 || column > 4 || row < 0 || row > 4)
			{
				return false;
			}

			bool state = false;
			if (field == Battlefield.One)
			{
				state = _battlefields[0, row, column];
			}
			else if (field == Battlefield.Two)
			{
				state = _battlefields[1, row, column];
			}
			else
			{
				Debug.LogError("The battlefield inputed has not been implemented yet. Battlefield was: " + field);
			}

			return state;
		}

		//assign battlefields to players
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

		//delete object after timeout
		public void DeleteAfterTimeout(float timeout, GameObject gameObjects)
		{
			DeleteAfterTimeout(timeout, new GameObject[] { gameObjects });
		}

		//delete objects after timeout
		public void DeleteAfterTimeout(float timeout, GameObject[] battlefieldObjects)
		{
			StartCoroutine(DeleteAfterTimeoutCoroutine(timeout, battlefieldObjects));
		}

		//spawn object after timeout
		public void SpawnAfterTimeout(float timeout, float deletionTimeout, string resourceName, Attack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
			StartCoroutine(SpawnAfterTimeoutCoroutine(timeout, deletionTimeout, resourceName, attack, attackType, zone, rotation));
		}

		//spawn a specified object
		public void Spawn(float deletionTimeout, string resourceName, Attack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
			GameObject go = (GameObject)Instantiate(Resources.Load(resourceName), zone, rotation);
			
			IExchangeAttack attackScript = go.GetComponent(attackType) as IExchangeAttack;
			if (attackScript == null)
				attackScript = go.GetComponentInChildren(attackType) as IExchangeAttack;
			attackScript.SetAttack(attack);
			DeleteAfterTimeout(deletionTimeout, go);
		}

		//coroutine to delete specified objects
		private IEnumerator DeleteAfterTimeoutCoroutine(float timeout, GameObject[] battlefieldObjects)
		{
			yield return new WaitForSeconds(timeout);
			foreach (GameObject battlefieldObject in battlefieldObjects)
			{
				Destroy(battlefieldObject);
			}
		}

		//coroutine to spawn an object after a timeout
		private IEnumerator SpawnAfterTimeoutCoroutine(float timeout, float deletionTimeout, string resourceName, Attack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
			yield return new WaitForSeconds(timeout);
			Spawn(deletionTimeout, resourceName, attack, attackType, zone, rotation);
		}
	}
}

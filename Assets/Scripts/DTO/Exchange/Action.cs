using Assets.Scripts.Controllers;
using Assets.Scripts.Exchange;
using UnityEngine;

namespace Assets.Scripts.Library
{
	public class Action
	{
		//Action Name
		public string Name;

		//Action Attack: Used for calculating health/energy
		public Attack Attack;

		//This is used the ui texture for the action in the 
		public Color ActionTexture;

		//This is how long after an action is used before it can be used again
		public float Cooldown;

		//this is the primary action method run when this action is used
		public System.Action<BattlefieldController, Attack> PrimaryAction;

		public Action(string name, Attack attack, Color attackTexture, string primaryActionName, float cooldown)
		{
			Name = name;
			Attack = attack;
			Cooldown = cooldown;
			ActionTexture = attackTexture;

			if(ActionMethodLibrary.ActionMethodLibraryTable.ContainsKey(primaryActionName))
			{
				//based on the action name, go find the relevant method in the Action Method Library
				PrimaryAction = ActionMethodLibrary.ActionMethodLibraryTable[primaryActionName];
			}
			else
			{
				//otherwise return a generic method and throw an error
				PrimaryAction = ActionMethodLibrary.ActionMethodLibraryTable["default"];
				Debug.LogError(name + " - Action: The \"" + primaryActionName + "\" Action Method was not in the ActionMethod Dictionary");
			}
		}

		//when this method is called, it runs the primary action and passes in the Attack and Battlefield Controller for use in that method
		public void InitiateAttack(BattlefieldController bc)
		{
			PrimaryAction(bc, Attack);
		}
	}
}

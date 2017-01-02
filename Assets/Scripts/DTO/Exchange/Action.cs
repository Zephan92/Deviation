using Assets.Scripts.Controllers;
using Assets.Scripts.Exchange;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using UnityEngine;

namespace Assets.Scripts.Library
{
	public class Action : IAction
	{
		//Action Name
		public string Name { get; set; }

		//Action Attack: Used for calculating health/energy
		public IAttack Attack { get; set; }

		//This is used the ui texture for the action in the 
		public Color ActionTexture { get; set; }

		//This is how long after an action is used before it can be used again
		public float Cooldown { get; set; }

		public IModule ParentModule { get; set; }

		public string PrimaryActionName { get; set; }

		//this is the primary action method run when this action is used
		public System.Action<IBattlefieldController, IAttack, IPlayer> PrimaryAction;

		public Action(string name, IAttack attack, Color actionTexture, string primaryActionName, float cooldown)
		{
			Name = name;
			Attack = attack;
			Cooldown = cooldown;
			ActionTexture = actionTexture;
			PrimaryActionName = primaryActionName;

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
		public void InitiateAttack(IBattlefieldController bc)
		{
			PrimaryAction(bc, Attack, ParentModule.ParentKit.Player);
		}

		public IAction GetRightAction()
		{
			return ParentModule.GetRightAction();
		}

		public IAction GetLeftAction()
		{
			return ParentModule.GetLeftAction();
		}
	}
}

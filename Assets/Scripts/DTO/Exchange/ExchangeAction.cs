using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Library;
using UnityEngine;

namespace Assets.Scripts.DTO.Exchange
{
	public class ExchangeAction : IExchangeAction
	{
		//Action Name
		public string Name { get; set; }

		//Action Attack: Used for calculating health/energy
		public IAttack Attack { get; set; }

		//This is used the ui texture for the action in the 
		public Texture2D ActionTexture { get; set; }

		//This is how long after an action is used before it can be used again
		public float Cooldown { get; set; }

		public IModule ParentModule { get; set; }

		public string PrimaryActionName { get; set; }

		//this is the primary action method run when this action is used
		public System.Action<IBattlefieldController, IAttack, IPlayer> PrimaryAction;

		public ExchangeAction(string name, IAttack attack, Texture2D actionTexture, string primaryActionName, float cooldown)
		{
			Name = name;
			Attack = attack;
			Cooldown = cooldown;
			ActionTexture = actionTexture;
			PrimaryActionName = primaryActionName;

			if(ActionMethodLibrary.ContainsActionMethod(primaryActionName))
			{
				//based on the action name, go find the relevant method in the Action Method Library
				PrimaryAction = ActionMethodLibrary.GetActionMethod(primaryActionName);
			}
			else
			{
				//otherwise return a generic method and throw an error
				PrimaryAction = ActionMethodLibrary.GetActionMethod("default");
				Debug.LogError(name + " - Action: The \"" + primaryActionName + "\" Action Method was not in the ActionMethod Dictionary");
			}
		}

		//when this method is called, it runs the primary action and passes in the Attack and Battlefield Controller for use in that method
		public void InitiateAttack(IBattlefieldController bc)
		{
			PrimaryAction(bc, Attack, ParentModule.ParentKit.Player);
		}
	}
}

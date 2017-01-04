using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class InputWrapper : IInput
	{
		public bool IsCycleActionLeftPressed()
		{
			return Input.GetKeyDown(KeyCode.U);
		}

		public bool IsActionPressed()
		{
			return Input.GetKeyDown(KeyCode.I);
		}

		public bool IsCycleActionRightPressed()
		{
			return Input.GetKeyDown(KeyCode.O);
		}

		public bool IsCycleModuleLeftPressed()
		{
			return Input.GetKeyDown(KeyCode.J);
		}

		public bool IsModulePressed()
		{
			return Input.GetKeyDown(KeyCode.K);
		}

		public bool IsCycleModuleRightPressed()
		{
			return Input.GetKeyDown(KeyCode.L);
		}

		public bool IsPausePressed()
		{
			return Input.GetKeyDown(KeyCode.Escape);
		}

		public bool IsUpPressed()
		{
			return Input.GetKeyDown(KeyCode.W);
		}

		public bool IsDownPressed()
		{
			return Input.GetKeyDown(KeyCode.S);
		}

		public bool IsLeftPressed()
		{
			return Input.GetKeyDown(KeyCode.A);
		}

		public bool IsRightPressed()
		{
			return Input.GetKeyDown(KeyCode.D);
		}
	}
}

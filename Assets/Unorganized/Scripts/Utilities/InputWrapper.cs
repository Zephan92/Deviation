using System;
using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class InputWrapper : IInput
	{
		public bool IsPausePressed()
		{
			return Input.GetKeyDown(KeyCode.Escape);
		}

		public bool IsUpPressed()
		{
			return Input.GetKeyDown(KeyCode.UpArrow);
		}

		public bool IsDownPressed()
		{
			return Input.GetKeyDown(KeyCode.DownArrow);
		}

		public bool IsLeftPressed()
		{
			return Input.GetKeyDown(KeyCode.LeftArrow);
		}

		public bool IsRightPressed()
		{
			return Input.GetKeyDown(KeyCode.RightArrow);
		}

		public bool IsAction_Q_Pressed()
		{
			return Input.GetKeyDown(KeyCode.Q);
		}

		public bool IsAction_W_Pressed()
		{
			return Input.GetKeyDown(KeyCode.W);
		}

		public bool IsAction_E_Pressed()
		{
			return Input.GetKeyDown(KeyCode.E);
		}

		public bool IsAction_R_Pressed()
		{
			return Input.GetKeyDown(KeyCode.R);
		}
	}
}

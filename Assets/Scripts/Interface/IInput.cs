using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Interface
{
	public interface IInput
	{
		bool IsAction_Q_Pressed();
		bool IsAction_W_Pressed();
		bool IsAction_E_Pressed();
		bool IsAction_R_Pressed();
		bool IsPausePressed();
		bool IsUpPressed();
		bool IsDownPressed();
		bool IsLeftPressed();
		bool IsRightPressed();
		
	}
}

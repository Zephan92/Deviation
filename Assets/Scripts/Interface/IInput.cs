using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Interface
{
	public interface IInput
	{
		bool IsCycleActionLeftPressed();
		bool IsActionPressed();
		bool IsCycleActionRightPressed();
		bool IsCycleModuleLeftPressed();
		bool IsModulePressed();
		bool IsCycleModuleRightPressed();
		bool IsPausePressed();
		bool IsUpPressed();
		bool IsDownPressed();
		bool IsLeftPressed();
		bool IsRightPressed();
		
	}
}

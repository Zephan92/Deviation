﻿namespace Assets.Scripts.Enum
{
    public enum ExchangeState
    {
		Awake,
        Setup,
        PreBattle,	//user can interact here
        Begin,
        Battle,     //user can interact here
		End,
        PostBattle,	//user can interact here
		Teardown,
        Paused,     //user can interact here
	}
}

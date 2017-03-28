namespace Assets.Scripts.Interface
{
    public interface IMultiplayerController
    {
		int NumberOfPlayers { get; set; }
		int CurrentRound { get; set; }
		int NumberOfRounds { get; set; }
		int[] Winners { get; set; }

		void StartMultiplayerExchangeInstance();
    }
}

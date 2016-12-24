using Assets.Scripts.Enum;

namespace Assets.Scripts.Interface
{
    public interface IExchangeController
    {
		int NumberOfPlayers { get; }
		Battlefield MainPlayerFieldNumber { get; }
		void UpdateExchangeControlsDisplay();
	}
}


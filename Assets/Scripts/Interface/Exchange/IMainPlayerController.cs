namespace Assets.Scripts.Interface.Exchange
{
	public interface IMainPlayerController
	{
		IInput Input { get; set; }
		IPlayer MainPlayer { get; set; }
		IExchangeController ExchangeController { get; set; }

		void CheckInput();
	}
}

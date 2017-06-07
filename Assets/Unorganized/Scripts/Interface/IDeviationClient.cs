using Deviation.Data.PlayerAccount;

namespace Assets.Scripts.Interface
{
	public interface IDeviationClient
	{
		IPlayerAccount currentPlayer { get; set; }
	}
}

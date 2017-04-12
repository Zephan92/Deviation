
using Deviation.Data.ResourceBag;

namespace Deviation.Data.PlayerAccount
{
	public interface IPlayerAccount
	{
		string AccountName { get; set; }
		string AccountAlias { get; set; }
		IResourceBag ResourceBag { get; set; }
	}
}

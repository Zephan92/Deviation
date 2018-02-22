namespace Assets.Deviation.MasterServer.Scripts.ResourceBank
{
	public abstract class ResourceBankTypeBase
	{
		protected abstract int Common();
		protected abstract int Uncommon();
		protected abstract int Rare();
		protected abstract int Mythic();
		protected abstract int Legendary();

		public int GetCount(Rarity rarity)
		{
			switch (rarity)
			{
				case Rarity.Common:
					return Common();

				case Rarity.Uncommon:
					return Uncommon();

				case Rarity.Rare:
					return Rare();

				case Rarity.Mythic:
					return Mythic();

				case Rarity.Legendary:
					return Legendary();

				default:
					return 0;
			}
		}
	}

	public class ResourceBankTypeGeneral : ResourceBankTypeBase
	{
		private const int Common_Count = 5;
		private const int Uncommon_Count = 3;
		private const int Rare_Count = 2;
		private const int Mythic_Count = 1;
		private const int Legendary_Count = 1;

		protected override int Common()
		{
			return Common_Count;
		}
		protected override int Legendary()
		{
			return Legendary_Count;
		}
		protected override int Mythic()
		{
			return Mythic_Count;
		}
		protected override int Rare()
		{
			return Rare_Count;
		}
		protected override int Uncommon()
		{
			return Uncommon_Count;
		}
	}
}

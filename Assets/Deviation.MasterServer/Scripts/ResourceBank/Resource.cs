namespace Assets.Deviation.MasterServer.Scripts.ResourceBank
{
	public enum Rarity
	{
		Default = -1,
		Common,
		Uncommon,
		Rare,
		Mythic,
		Legendary,
		Count,
	}

	public enum ResourceType
	{
		Base,
	}

	public struct Resource
	{
		public string Name { get; set; }
		public ResourceType Type { get; set; }
		public Rarity Rarity { get; set; }
		public int DropRate { get; set; }

		public Resource(string name, ResourceType type, Rarity rarity, int dropRate)
		{
			Name = name;
			Type = type;
			Rarity = rarity;
			DropRate = dropRate;
		}

		public override string ToString()
		{
			return string.Format("Name: {0}. Type: {1}. Rarity: {2}. DropRate: {3}", Name, Type, Rarity, DropRate);
		}
	}
}

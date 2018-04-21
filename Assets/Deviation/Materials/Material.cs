namespace Assets.Deviation.Materials
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

	public enum MaterialType
	{
		Default = -1,
		Base,
		Special,
		Type,
	}

	public struct Material
	{
		public string Name { get; set; }
		public MaterialType Type { get; set; }
		public Rarity Rarity { get; set; }
		public int DropRate { get; set; }

		public Material(string name, MaterialType type, Rarity rarity, int dropRate)
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

		public bool Equals(Material material)
		{
			return Name == material.Name && Type == material.Type && Rarity == material.Rarity && DropRate == material.DropRate;
		}
	}
}

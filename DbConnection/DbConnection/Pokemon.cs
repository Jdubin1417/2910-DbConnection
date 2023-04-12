namespace DbConnection
{
    public class Pokemon
    {
        public int DexNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public string PrimaryType { get; set; } = string.Empty;
        public string? SecondaryType { get; set; }

        public Pokemon(int dexNumber, string name, int level, string primaryType, string? secondaryType)
        {
            DexNumber = dexNumber;
            Name = name;
            Level = level;
            PrimaryType = primaryType;
            SecondaryType = secondaryType;
        }

        public override string ToString()
        {
            return
                $"{DexNumber},{Name},{Level},{PrimaryType},{SecondaryType}";
        }
    }
}

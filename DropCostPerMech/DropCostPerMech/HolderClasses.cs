namespace DropCostPerMech {
    public class Settings {
        public float percentageOfMechCost = 0.0025f;

        public bool CostByTons = false;
        public int cbillsPerTon = 500;
        public bool someFreeTonnage = false;
        public int freeTonnageAmount = 0;
    }

    public class Fields {
        public static float DropCost = 0;
        public static int LanceTonnage = 0;
        public static string FormattedDropCost = string.Empty;
        public static string FreeTonnageText = string.Empty;
    }
}
namespace DropCostPerMech {
    public class Settings {
        public float percentageOfMechCost = 0.0025f;

        public bool CostByTons = false;
        public int cbillsPerTon = 500;

        public bool someFreeTonnage = false;
        int freeTonnageAmount = 0;
    }

    public class Fields {
        public static float cbill = 0;
    }
}
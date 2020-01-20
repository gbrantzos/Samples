namespace PharmexTargets.Import
{
    public class Target
    {
        public string ItemCode { get; set; }

        public double Quantity { get; set; }
        public double Value { get; set; }

        public Ratio RatioPerCompany { get; set; }
        public MonthPercentage MonthPercentage { get; set; }

        public override string ToString()
            => $"{ItemCode}, qty={Quantity}, value={Value}, ratios: {RatioPerCompany}";
    }
}

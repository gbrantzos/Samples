using System;
using System.Linq;

namespace PharmexTargets.Import
{
    public class MonthPercentage
    {
        public enum Months
        {
            January = 0,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }

        public enum PercentageType
        {
            Quantity = 0,
            Value
        }

        private readonly double[,] values = new double[12, 2];

        public double this[Months month, PercentageType type] => values[(int)month, (int)type];

        public MonthPercentage(double[] quantityPercentages, double[] valuePercentages)
        {
            if (quantityPercentages.Length != 12)
                throw new ArgumentException("Month percentage must be constructed with twelve values (quantity)");
            if (valuePercentages.Length != 12)
                throw new ArgumentException("Month percentage must be constructed with twelve values (value)");

            for (int i = 0; i < 12; i++)
            {
                values[i, 0] = quantityPercentages[i];
                values[i, 1] = valuePercentages[i];
            }

            double sum = 0;
            sum = quantityPercentages.Sum();
            if (sum != 0 && Math.Abs(sum - 100) > 0.01)
                quantityPercentages[6] = Math.Round(100 - quantityPercentages.Where((_, i) => i != 6).Sum(), 2);
            sum = valuePercentages.Sum();
            if (sum != 0 && Math.Abs(sum - 100) > 0.01)
                valuePercentages[6] = Math.Round(100 - valuePercentages.Where((_, i) => i != 6).Sum(), 2);
        }
    }
}

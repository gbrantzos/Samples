using System;
using System.Collections.Generic;
using System.Linq;

namespace PharmexTargets.Import
{
    public class Ratio
    {
        public enum Companies
        {
            Pharmex = 1,
            Papharm
        }

        private readonly double[] values;

        public double this[Companies company] => values[(int)company -1];

        public Ratio(double[] values)
        {
            this.values = values;

            if (values.Length != 2)
                throw new ArgumentException("Ratio per company must be constructed with two values");
            if (((values[0] + values[1]) != 1) && (values[0] + values[1] != 0))
                values[1] = 1 - values[0];
        }

        public override string ToString()
            => $"Pharmex: {this[Companies.Pharmex]:0.00%}, Papharm {this[Companies.Papharm]:0.00%}";

        public static IEnumerable<Companies> AllCompanies()
            => Enum.GetValues(typeof(Ratio.Companies)).Cast<Ratio.Companies>();
    }
}
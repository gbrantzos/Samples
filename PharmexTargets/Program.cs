using System;
using System.IO;
using System.Linq;
using System.Text;
using PharmexTargets.Import;
using PharmexTargets.Persistence;

namespace PharmexTargets
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            const string connectionString = "Server=(local);Database=DDDSample;Trusted_Connection=true";

            const string filePath = @"C:\Source\Sandbox\Workbench\Samples\PharmexTargets\SampleFiles\Targets.2020.xlsx";
            const string year = "2020";

            using (var excelData = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var targetData = new TargetData
                {
                    Year    = year,
                    Targets = new Parser().GetTargets(excelData).ToList(),
                };

                var targetRows = (from target  in targetData.Targets
                                  from month   in MonthPercentage.AllMonths()
                                  from company in Ratio.AllCompanies()
                                  select new TargetRow
                                  {
                                      ItemCode = target.ItemCode,
                                      AmonthCode = targetData.Year + "-M" + ((int)month).ToString("00"),
                                      CompanyID = (int)company,
                                      TargetQuantity = Math.Round(
                                          target.Quantity
                                        * target.RatioPerCompany[company]
                                        * target.MonthPercentage[month, MonthPercentage.PercentageType.Quantity]
                                        * 0.01, 2),
                                      TargetValue = Math.Round(
                                          target.Value
                                        * target.RatioPerCompany[company]
                                        * target.MonthPercentage[month, MonthPercentage.PercentageType.Value]
                                        * 0.01, 2)
                                  })
                           .ToList();

                var dummy = targetData.Targets.First(i => i.ItemCode == "101.02.2ΚΠ.001");
                var dummyRow = targetRows.First(i => i.ItemCode == "101.02.2ΚΠ.001");

                var targetUpdater = new TargetUpdater(connectionString);
                targetUpdater.UpdateTargets(targetRows);
            }

            Console.WriteLine("Press any key to continue!");
            Console.ReadKey();
        }
    }
}

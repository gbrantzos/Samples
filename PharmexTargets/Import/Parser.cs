using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;

namespace PharmexTargets.Import
{
    public interface IParser
    {
        IEnumerable<Target> GetTargets();
    }

    public class Parser : IDisposable, IParser
    {
        private readonly string TargetSheet = "Targets";
        private readonly string PercentagesSheet = "Percentages";

        private readonly Stream excelData;
        private readonly IExcelDataReader excelReader;

        public Parser(string filePath)
        {
            excelData = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            excelReader = ExcelReaderFactory.CreateOpenXmlReader(excelData);
        }

        public IEnumerable<Target> GetTargets()
        {
            var ds = excelReader.AsDataSet(new ExcelDataSetConfiguration
            {
                UseColumnDataType = true,
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });

            if (!ds.Tables.Contains(PercentagesSheet))
                throw new ExcelParserException($"Sheet '{PercentagesSheet}' not found!");

            if (!ds.Tables.Contains(TargetSheet))
                throw new ExcelParserException($"Sheet '{TargetSheet}' not found!");
            var monthPercentages = GetMonthPercentages(ds.Tables[PercentagesSheet]);

            int rows = ds.Tables[TargetSheet].Rows.Count;
            for (int i = 0; i < rows; i++)
            {
                var row = ds.Tables[TargetSheet].Rows[i];
                var itemCode = row["ItemCode"].ToString();
                if (String.IsNullOrEmpty(itemCode)) continue;

                yield return new Target
                {
                    ItemCode = itemCode,
                    Quantity = row.SafeField<double>("Quantity"),
                    Value = row.SafeField<double>("Value"),
                    RatioPerCompany = new Ratio(new double[]
                        {
                            row.SafeField<double>("Pharmex"),
                            row.SafeField<double>("Papharm")
                        }),
                    MonthPercentage = monthPercentages.FirstOrDefault(i => i.ItemCode == itemCode).monthPercentage
                };
            }
        }

        private List<(string ItemCode, MonthPercentage monthPercentage)> GetMonthPercentages(DataTable dataTable)
        {
            var toReturn = new List<(string ItemCode, MonthPercentage monthPercentage)>();
            int rows = dataTable.Rows.Count;
            for (int i = 0; i < rows; i++)
            {
                var row = dataTable.Rows[i];
                var itemCode = row["ItemCode"].ToString();
                if (String.IsNullOrEmpty(itemCode)) continue;

                var monthPercentage = new MonthPercentage(
                    new double[]
                    {
                        row.SafeField<double>("M01_QTY"),
                        row.SafeField<double>("M02_QTY"),
                        row.SafeField<double>("M03_QTY"),
                        row.SafeField<double>("M04_QTY"),
                        row.SafeField<double>("M05_QTY"),
                        row.SafeField<double>("M06_QTY"),
                        row.SafeField<double>("M07_QTY"),
                        row.SafeField<double>("M08_QTY"),
                        row.SafeField<double>("M09_QTY"),
                        row.SafeField<double>("M10_QTY"),
                        row.SafeField<double>("M11_QTY"),
                        row.SafeField<double>("M12_QTY")
                    },
                    new double[]
                    {
                        row.SafeField<double>("M01_VALUE"),
                        row.SafeField<double>("M02_VALUE"),
                        row.SafeField<double>("M03_VALUE"),
                        row.SafeField<double>("M04_VALUE"),
                        row.SafeField<double>("M05_VALUE"),
                        row.SafeField<double>("M06_VALUE"),
                        row.SafeField<double>("M07_VALUE"),
                        row.SafeField<double>("M08_VALUE"),
                        row.SafeField<double>("M09_VALUE"),
                        row.SafeField<double>("M10_VALUE"),
                        row.SafeField<double>("M11_VALUE"),
                        row.SafeField<double>("M12_VALUE")
                    });

                toReturn.Add((itemCode, monthPercentage));
            }
            return toReturn;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    excelReader?.Dispose();
                    excelData?.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

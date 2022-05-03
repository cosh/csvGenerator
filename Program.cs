using System;
using System.Globalization;
using System.IO;

namespace csvGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var numberOfRecords = 10000000;
            var numberOfRecordsPerFile = 200000;
            var numberOfColumns = 99;
            var numberOfDays = 60;
            var endDate = DateTime.Now;
            var startDate = DateTime.Now.AddDays(-1 * numberOfDays);
            var columnPrefix = "c";
            var rootPath = @"E:\temp\csvGenerated\date";

            var diff = endDate - startDate;
            var msDiffPerRecord = diff.TotalMilliseconds / numberOfRecords;
            var prng = new Random();

            var csvSeparator = ",";

            var header = "ts";

            for (int i = 0; i < numberOfColumns; i++)
            {
                header = header + csvSeparator + columnPrefix + i;
            }

            StreamWriter writer = null;
            int count = 0;
            int batch = 0;

            var currentTs = startDate;
            var lastTsDateInFile = currentTs.ToString("yyyy-MM-dd");
            var lastTs = startDate;

            for (int i = 0; i < numberOfRecords; i++)
            {
                lastTs = currentTs;
                currentTs = currentTs.AddMilliseconds(msDiffPerRecord); 

                if ((count % numberOfRecordsPerFile == 0) || (currentTs.Day != lastTs.Day))
                {
                    if (writer != null)
                    {
                        //flush it
                        writer.Flush();
                        writer.Close();
                    }

                    Console.WriteLine($"new batch {batch}");
                    
                    lastTsDateInFile = currentTs.ToString("yyyy-MM-dd");
                    var fileName = String.Concat(lastTsDateInFile + "-batch-", batch, ".csv");

                    //new file
                    var batchFilename = Path.Combine(rootPath, fileName);
                    writer = new StreamWriter(batchFilename, true);
                    writer.WriteLine(header);
                    batch++;
                    count = 0;
                }

                writer.WriteLine(GenerateLine(currentTs, numberOfColumns, prng, csvSeparator));

                count++;
            }

            writer.Flush();
            writer.Close();

            Console.WriteLine("Done.");

            Console.ReadLine();
        }

        private static String GenerateLine(DateTime currentTs, int numberOfColumns, Random prng, string csvSeparator)
        {
            var line = currentTs.ToString("yyyy-MM-dd HH:mm:ss.fff");

            for (int i = 0; i < numberOfColumns; i++)
            {
                line = line + csvSeparator + (prng.NextDouble() * 100).ToString("0.##", CultureInfo.InvariantCulture); ;
            }

            return line;
        }
    }
}

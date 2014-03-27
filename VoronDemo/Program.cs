using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voron;
using Voron.Impl;

namespace VoronDemo
{
    class Program
    {
        private const int TestElementSize = 100;
        private const string TreeName = "TestTree";
        private const int TestElementsCount = 1024 * 1024;

        static void Main(string[] args)
        {
            Console.Write("Generating random {0} byte values...", TestElementSize);
            var testData = GenerateTestData(TestElementsCount).ToList();
            Console.WriteLine("done");
            
            using (var env = new StorageEnvironment(StorageEnvironmentOptions
                .ForPath(Environment.CurrentDirectory + "\\Data" + Guid.NewGuid())))
            {
                using (var tx = env.NewTransaction(TransactionFlags.ReadWrite))
                {
                    env.CreateTree(tx, TreeName);
                    tx.Commit();
                }
                int keyIndex = 0;
                Console.WriteLine("Read/write test with sequential keys");
                Console.WriteLine("----------------------------------");
                PerformanceTest(testData, () => "testElement/" + (keyIndex++), env);
            }
        }

        private static void PerformanceTest(IEnumerable<byte[]> testData,Func<string> keyGeneratorFunc, StorageEnvironment env)
        {
//////////////////////////////////////////////////////////////////////////////////////////
            // write
            var writeBatch = new WriteBatch();
            int index = 1;
            Console.Write("Adding values to WriteBatch...");
            foreach (var testElement in testData)
            {
                var elementStream = new MemoryStream(testElement);
                writeBatch.Add(keyGeneratorFunc(), elementStream, TreeName);
            }
            Console.WriteLine("done");

            var totalWriteTimer = Stopwatch.StartNew();
            Console.Write("Writing the data to Voron storage...");
            env.Writer.Write(writeBatch);
            totalWriteTimer.Stop();
            Console.WriteLine("done");
            Console.WriteLine("Time that took to write 1000000 elements is {0}ms", totalWriteTimer.ElapsedMilliseconds);
            Console.WriteLine("On the average it took {0:0##.#####}ms per item",
                (decimal) totalWriteTimer.ElapsedMilliseconds/TestElementsCount);

            ///////////////////////////////////////////////////////////////////////
            // single threaded read
            using (var snapshot = env.CreateSnapshot())
            {
                Console.Write("Reading test elements (single thread)...");
                var totalReadTimer = Stopwatch.StartNew();
                var readBuffer = new byte[TestElementSize];
                using (var iterator = snapshot.Iterate(TreeName))
                {
                    //if iterator is not empty
                    if (iterator.Seek(Slice.BeforeAllKeys))
                    {
                        do
                        {
                            using (var valueStream = iterator.CreateReaderForCurrent().AsStream())
                            {
                                valueStream.Read(readBuffer, 0, TestElementSize);
                            }
                        } while (iterator.MoveNext());
                    }
                }
                totalReadTimer.Stop();
                Console.WriteLine("done");
                Console.WriteLine("Time that took to read 1000000 elements is {0}ms", totalReadTimer.ElapsedMilliseconds);
                Console.WriteLine("On the average it took {0:0##.#####}ms per item",
                    (decimal) totalReadTimer.ElapsedMilliseconds/TestElementsCount);
            }
        }

        private static IEnumerable<byte[]> GenerateTestData(int testCollectionSize)
        {
            var random = new Random();
            for (int i = 0; i < testCollectionSize; i++)
            {
                var testDataElement = new byte[TestElementSize];
                random.NextBytes(testDataElement);
                yield return testDataElement;
            }
        }
    }
}

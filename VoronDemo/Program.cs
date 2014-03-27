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
        private const int TestElementSize = 128;
        private const string TreeName = "TestTree";
        private const int TestElementsCount = 1024 * 16;

        static void Main(string[] args)
        {
            //in 32bit mode it might be impossible to find continuous blocks of large enough memory
            //if TestElementsCount or TestElementSize are big
            Console.WriteLine("Is running under 64 bits? " + Environment.Is64BitProcess);
            var testData = GenerateTestData(TestElementsCount).ToList();
            
            using (var env = new StorageEnvironment(StorageEnvironmentOptions
                .ForPath(Environment.CurrentDirectory + "\\Data" + Guid.NewGuid())))
            {
                using (var tx = env.NewTransaction(TransactionFlags.ReadWrite))
                {
                    env.CreateTree(tx, TreeName);
                    tx.Commit();
                }
              
                //first write data
                int index = 1;
                var writeBatch = new WriteBatch();
                foreach (var element in testData)
                    writeBatch.Add("element/" + index++,new MemoryStream(element), TreeName);

                var sw = Stopwatch.StartNew();
                env.Writer.Write(writeBatch);
                sw.Stop();

                Console.WriteLine("took {0:##.#####}ms to write {1} elements",sw.ElapsedMilliseconds,TestElementsCount);

                //then read the data
                sw = Stopwatch.StartNew();
                using (var snapshot = env.CreateSnapshot())
                {
                    using (var iterator = snapshot.Iterate(TreeName))
                    {
                        if (iterator.Seek(Slice.BeforeAllKeys))
                        {
                            var buffer = new byte[TestElementSize];
                            do
                            {
                                using (var valueStream = iterator.CreateReaderForCurrent().AsStream())
                                    valueStream.Read(buffer, 0, TestElementSize);
                            } while (iterator.MoveNext());
                        }
                    }
                }
                sw.Stop();
                Console.WriteLine("took {0:##.#####}ms to read {1} elements", sw.ElapsedMilliseconds, TestElementsCount);
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

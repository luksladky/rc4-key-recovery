using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class Log
    {
        public static void v(string s)
        {
            Console.WriteLine(s);
        }

        public static void v(int s)
        {
            Console.WriteLine(s);
        }

        public static void v(double s)
        {
            Console.WriteLine(s);
        }

        public static void ByteArray(byte[] a)
        {
            for (int k = 0; k < a.Length; k++)
            {
                Console.Write("{0:X2}", a[k]);
            }
            Console.WriteLine();
        }

        public static void Key(Key key)
        {
            Console.Write("\n Key: ");
            for (int i = 0; i < key.Length; i++)
            {
                Console.Write("{0:X} ", key[i]);
            }
            Console.WriteLine();
        }

        public static void FrequencyOfKeyByte(int[,] frequencyTable, int keyByte, int treshold)
        {
            Console.WriteLine("\n Frequency of {0}th key byte with minimum frequency {1}", keyByte + 1, treshold);
            for (int j = 0; j < frequencyTable.GetLength(1); j++)
            {
                if (frequencyTable[keyByte, j] > treshold)
                {
                    Console.WriteLine("Val: {0:X2}, Freq: {1} ", j, frequencyTable[keyByte, j]);
                }
            }
            Console.WriteLine();

        }

        public static void FrequencyTable(int[,] frequencyTable, int treshold)
        {
            Console.WriteLine("\n Frequency table");
            for (int i = 0; i < frequencyTable.GetLength(0); i++)
            {
                Console.WriteLine("\n Key byte {0}", i);
                for (int j = 0; j < frequencyTable.GetLength(1); j++)
                {
                    if (frequencyTable[i, j] > treshold)
                    {
                        Console.WriteLine("Val: {0:X2}, Freq: {1}", j, frequencyTable[i, j]);
                    }

                }
            }
            Console.WriteLine();
        }

        public static void WeightTable(double[,] weightsTable, double treshold)
        {
            Console.WriteLine("\n Frequency table");
            for (int i = 0; i < weightsTable.GetLength(0); i++)
            {
                Console.WriteLine("\n Key byte {0}", i);
                for (int j = 0; j < weightsTable.GetLength(1); j++)
                {
                    if (weightsTable[i, j] > treshold)
                    {
                        Console.WriteLine("Val: {0:X2}, Freq: {1}", j, weightsTable[i, j]);
                    }

                }
            }
            Console.WriteLine();
        }

        public static void FrequencyListSortedWithTreshold(List<KeyByteFrequency>[] freqencyList) 
        {
            for (int i = 0; i < freqencyList.Length; i++)
            {
                Console.WriteLine("{0}th key byte", i);
                foreach (KeyByteFrequency keyByteFrequency in freqencyList[i])
                {
                    Console.WriteLine("byte {0:X} - {1}x", keyByteFrequency.keyByte, keyByteFrequency.frequency);
                }
            }
            Console.WriteLine();
        }

        public static void writeOutState(int[] S, int i, int j)
        {
            Console.WriteLine("RC4 permutation, i = {0}, j = {1}", i, j);
            for (int k = 0; k < Constants.N; k++)
            {
                Console.Write("{0:X2} ", S[k]);
                if ((k % 16) == 15) Console.Write("\n");
            }
            Console.WriteLine();
        }

        public static void Combinations(List<int[]> combinations )
        {
            foreach (int[] arrayInt in combinations)
            {
                for (int i = arrayInt.GetLowerBound(0); i <= arrayInt.GetUpperBound(0); i++)
                {
                    Console.Write("{0},", arrayInt.GetValue(i));
                }
                Console.WriteLine();
            }
        }

        public static void Array(int[] a)
        {
            
            for (int i = 0; i < a.Length; i++)
            {
                Console.Write("{0:X} ",a[i]);
            }
            Console.WriteLine();
        }

        public static void Combination(bool[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i])
                    Console.Write("{0} ", i);
            }
            Console.WriteLine();
        }

        public static void WeightsArray(double[] a)
        {
            NumberFormatInfo nfInfo = new NumberFormatInfo();
            nfInfo.NumberDecimalSeparator = ".";

            Console.Write('{');
            for (int i = 0; i < a.Length; i++)
            {
                Console.Write(string.Format(a[i].ToString(nfInfo)));
                if (i != a.Length -1) Console.Write(',');
            }
            Console.Write('}');
            Console.WriteLine();
        }

    }
}

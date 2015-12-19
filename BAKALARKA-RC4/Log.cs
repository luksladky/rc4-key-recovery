using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class Log
    {


        public static void ByteArray(byte[] a)
        {
            for (int k = 0; k < a.Length; k++)
            {
                Console.Write("{0:X2}", a[k]);
            }
        }

        public static void Key(Key key)
        {
            Console.Write("\n Actual key: ");
            for (int i = 0; i < key.Length; i++)
            {
                Console.Write("{0:X} ", key[i]);
            }
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
        }

        public static void writeOutState(int[] S, int i, int j)
        {
            Console.WriteLine("RC4 permutation, i = {0}, j = {1}", i, j);
            for (int k = 0; k < Constants.N; k++)
            {
                Console.Write("{0:X2} ", S[k]);
                if ((k % 16) == 15) Console.Write("\n");
            }
        }

    }
}

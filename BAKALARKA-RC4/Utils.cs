using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class Utils
    {
        public static int ord(char ch)
        {
            return (int)(Encoding.GetEncoding(1252).GetBytes(ch + "")[0]);
        }

        public static char chr(int i)
        {
            byte[] bytes = new byte[1];
            bytes[0] = (byte)i;
            return Encoding.GetEncoding(1252).GetString(bytes)[0];
        }







        public static int getByteLenghth(long x)
        {
            int lenght = 0;
            while (x > 0)
            {
                lenght++;
                x = x / 256;
            }
            return lenght;
        }

    }

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
                Console.Write("{0:X} ",key[i]);
            }
        }

        public static void FrequencyOfKeyByte(int[,] frequencyTable, int keyByte, int treshold)
        {
            Console.WriteLine("\n Frequency of {0}th key byte with minimum frequency {1}", keyByte + 1, treshold);
            for (int j = 0; j < frequencyTable.GetLength(1); j++)
            {
                if (frequencyTable[keyByte,j]>treshold)
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
                Console.WriteLine("\n Key byte {0}",i);
                for (int j = 0; j < frequencyTable.GetLength(1); j++)
                {
                    if (frequencyTable[i, j] > treshold)
                    {
                        Console.WriteLine("Val: {0:X2}, Freq: {1}",j ,frequencyTable[i, j]);
                    }
                    
                }
            }
        }

    }
}

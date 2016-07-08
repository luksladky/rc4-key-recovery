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

        public static List<int[]> getCombinations(int k, int n)
        {
            List<int[]> combinations = new List<int[]>();
            findCombinations(0, 0, new int[k], k, n, combinations);

            return combinations;
        } 

        private static void findCombinations(int index, int inSet, int[] prevSet, int k, int n, List<int[]> combinations)
        {
            if (index == n)
            {
                return;
            }

            if (inSet == k)
            {
                combinations.Add(prevSet);
            }
            else
            {
                int[] arr1 = (int[])prevSet.Clone();
                int[] arr2 = (int[])prevSet.Clone();

                arr1[inSet] = index;

                findCombinations(index + 1, inSet + 1, arr1, k, n, combinations);
                findCombinations(index + 1, inSet, arr2, k, n, combinations);
            }

        }

        public static double[,,] copyArray3D(double[,,] array, int x, int y, int z)
        {
            double[,,] result = new double[x, y, z];
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    for (int k = 0; k < z; k++)
                        result[i, j, k] = array[i, j, k];

            return result;
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
}

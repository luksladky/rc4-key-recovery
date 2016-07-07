using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{   
    public struct KeyByteFrequency
    {
        public int keyByte;
        public int frequency;

        public KeyByteFrequency(int keyByte, int frequency)
        {
            this.keyByte = keyByte;
            this.frequency = frequency;
        }
    }

    public class KeyByteFreqComparer : IComparer<KeyByteFrequency>
    {

        public int Compare(KeyByteFrequency x, KeyByteFrequency y)
        {
            if (x.frequency == y.frequency)
                return 0;
            if (x.frequency > y.frequency)
                return -1;

            return 1;
        }
    }

    public class ArraySumComparer : IComparer<int[]>
    {

        public int Compare(int[] x, int[] y)
        {
            int sumX = 0;
            int sumY = 0;
            for (int i = 0;  i < x.Length; i++)
            {
                sumX += x[i];
                sumY += y[i];
            }

            if (sumX == sumY)
                return 0;
            if (sumX < sumY)
                return -1;

            return 1;
        }
    }

    class BuildKeyTable : Attacks
    {
        private int[] jarr1;
        private int[] jarr2;
        private int[,] keyTable;
        private List<KeyByteFrequency>[] keyFrequencyList;
        private List<int[]> freqListSelections;

        public static int logBuildKeyTableMinFreq = -1;
        public static bool logFrequencyList = false;


        public BuildKeyTable(RC4 cipher) : base(cipher)
        {

            ConstructInvS();
        }


        public void GenerateKeyTable()
        {
            jarr1 = new int[N + 1];
            jarr2 = new int[N + 1];
            keyTable = new int[l, N];

            jarr1[0] = jarr2[0] = 0;
            ConstructInvS();

            for (int y = 0; y < N; y++)
            {
                jarr1[y + 1] = S[y];
                jarr2[y + 1] = InvS[y];
            }
            for (int w = 0; w < l; w++)
            {
                for (int y = 0; y < N; y++)
                {
                    keyTable[w, y] = 0;
                }
            }
            for (int y = 0; y < N; y++)
            {
                keyTable[y % l, mod(jarr1[y + 1] - jarr1[y] - y, N)]++;
                keyTable[y % l, mod(jarr1[y + 1] - jarr2[y] - y, N)]++;
                keyTable[y % l, mod(jarr2[y + 1] - jarr1[y] - y, N)]++;
                keyTable[y % l, mod(jarr2[y + 1] - jarr2[y] - y, N)]++;
            }

            if (logBuildKeyTableMinFreq >= 0)
                Log.FrequencyTable(keyTable, logBuildKeyTableMinFreq);
        }

        private void BuildFrequencyList(int treshold, int maxDepth)
        {
            keyFrequencyList = new List<KeyByteFrequency>[this.l];
            for (int k = 0; k < this.l; k++)
            {
                List<KeyByteFrequency> kthByte = new List<KeyByteFrequency>();
                for (int i = 0; i < N; i++)
                {
                    if (keyTable[k, i] > treshold)
                    {
                        kthByte.Add(new KeyByteFrequency(i, keyTable[k, i]));
                    }
                }
                kthByte.Sort(new KeyByteFreqComparer()); //TODO oriznout maxDepth
                keyFrequencyList[k] = kthByte;
            }

            if (logFrequencyList)
                Log.FrequencyListSortedWithTreshold(keyFrequencyList);
        }

        public bool GuessKeyFromFrequenc(int treshold, int maxDepth)
        {
            int tried = 0;
            bool found = false;
            GenerateKeyTable();
            BuildFrequencyList(treshold, maxDepth);
            //Log.FrequencyListSortedWithTreshold(keyFrequencyList);

            GetTestSelections(l); //try all bytes -> to freqListSelections
            foreach (int[] selection in freqListSelections)
            {
                
                int[] keyBytes = new int[l];
                for (int i = 0; i < l; i++)
                {

                    keyBytes[i] = (keyFrequencyList[i])[selection[i]].keyByte;
                }

                Key testKey = new Key(keyBytes);
                //cipher.VerifyKey(testKey); //TODO real key verification
                //Log.Array(selection);
                //Log.Key(testKey);
                tried++;
                if (testKey.Equals(cipher.K))
                {
                    //Log.Key(testKey);
                    found = true;
                    Console.WriteLine("This is it, {0} tried",tried);
                    return true;
                }
            }
            Console.WriteLine("Key not in keyFrequencyList list with treshold {0}",treshold);
            return false;
            

        }

        private void GetTestSelections(int numberOfBytes)
        {
            freqListSelections = new List<int[]>();
            FindAllSelectionCombinations(0, numberOfBytes, new int[numberOfBytes]);
            freqListSelections.Sort(new ArraySumComparer());

        }

        private void FindAllSelectionCombinations(int index, int limit, int[] selection)
        {
            if (index == limit)
            {
                Log.Array(selection);

                freqListSelections.Add(selection);
                return;
            }
            for (int i = 0; i < keyFrequencyList[index].Count; i++)
            {
                int[] newSelection = (int[]) selection.Clone();
                newSelection[index] = i;
                FindAllSelectionCombinations(index + 1, limit, newSelection);
            }
            selection = null;
        }

        



        public void LinearEquationsTest()
        {
            for (int y = 0; y < 100; y++)
            {
                int sumOfKeyBytes = 0;
                for (int k = 0; k <= y; k++)
                {
                    sumOfKeyBytes += cipher.K[k];
                }
                int fy = mod((y * (y + 1) / 2) + sumOfKeyBytes, N);

                Console.Write("{0}: fy = {1,3}, Sy = {2,3}", y, fy, S[y]);
                if (fy == S[y])
                {
                    Console.Write(" <<<<<");
                }
                Console.WriteLine();
            }


        }


    }
}

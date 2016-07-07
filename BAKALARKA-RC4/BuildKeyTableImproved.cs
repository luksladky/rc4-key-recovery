using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class BuildKeyTableImproved : Attacks
    {
        public BuildKeyTableImproved(RC4 cipher) : base(cipher) { } 



        public static double logBuildKeyTableMinFreq = 0;
        public static bool logFrequencyList = false;


        private int GetWeight(int i, double[] probabilities)
        {
            if (i == -1) return 3;
            double tresholdHigh = 0.05;
            double tresholdLow = 0.008;

            return (probabilities[i] > tresholdHigh) ? 2 : (probabilities[i] < tresholdLow ? 0 : 1);
        }

        public void GenerateKeyTable()
        {
            double[] jSPr = new double[257];
            double[] jInvSPr = new double[257];
            jSPr[0] = 1;
            jInvSPr[0] = 1;

            for (int i = 0; i < N; i++)
            {
                jSPr[i + 1] = Constants.PSj[i];
                jSPr[i + 1] = Constants.PInvSj[i];
            }



            int[] jarrS = new int[N + 1];
            int[]  jarrInvS = new int[N + 1];
            double[,] keyTable = new double[l, N];

            jarrS[0] = jarrInvS[0] = 0;
            ConstructInvS();

            for (int y = 0; y < N; y++)
            {
                jarrS[y + 1] = S[y];
                jarrInvS[y + 1] = InvS[y];
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
                int first = (y == 0) ? 0 : S[y - 1];
                int second = S[y];

                int firstInv = (y == 0) ? 0 : InvS[y - 1];
                int secondInv = InvS[y];


                //if (y == 0 | first >= y & second > y) 
                keyTable[y % l, mod(jarrS[y + 1] - jarrS[y] - y, N)] += GetWeight(y, jSPr) + GetWeight(y-1, jSPr); //jSPr[y+1] * jSPr[y]*100;
                //if (y == 0 | first >= y & secondInv <= y)
                    keyTable[y % l, mod(jarrS[y + 1] - jarrInvS[y] - y, N)] += GetWeight(y, jSPr) +  GetWeight(y-1, jInvSPr); //jSPr[y + 1] * jInvSPr[y]*100;
                //if (y == 0 | firstInv <= y & second >= y)
                    keyTable[y % l, mod(jarrInvS[y + 1] - jarrS[y] - y, N)] += GetWeight(y, jInvSPr) + GetWeight(y-1, jSPr);// jInvSPr[y + 1] * jSPr[y]*100;
                //if (y == 0 | firstInv <= y & secondInv <= y)
                    keyTable[y % l, mod(jarrInvS[y + 1] - jarrInvS[y] - y, N)] += GetWeight(y, jInvSPr) + GetWeight(y-1, jInvSPr); //jInvSPr[y + 1] * jInvSPr[y]* 100;
            }

            if (logBuildKeyTableMinFreq >= 0)
                Log.WeightTable(keyTable, logBuildKeyTableMinFreq);
        }

        public bool GuessKeyFromFrequenc(int treshold, int maxDepth)
        {
            int tried = 0;
            bool found = false;
            GenerateKeyTable();
            //BuildFrequencyList(treshold, maxDepth);
            //Log.FrequencyListSortedWithTreshold(keyFrequencyList);

            //GetTestSelections(l); //try all bytes -> to freqListSelections
            /*foreach (int[] selection in freqListSelections)
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
                    Console.WriteLine("This is it, {0} tried", tried);
                    return true;
                }
            }*/
            Console.WriteLine("Key not in keyFrequencyList list with treshold {0}", treshold);
            return false;
        }

    }
}

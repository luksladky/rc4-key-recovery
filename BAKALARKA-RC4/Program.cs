using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BAKALARKA_RC4
{
    class Program
    {
        static void buildKeyAttact(Key key)
        {
            //Log.Key(key);
            //BuildKeyTable buildKeyTable = new BuildKeyTable(cipher);
            //buildKeyTable.GenerateKeyTable();

            //BuildKeyTableImproved buildKeyTableImp = new BuildKeyTableImproved(cipher);
            //buildKeyTableImp.GenerateKeyTable();
            //Log.Key(key

            //Log.Key(key);
            //Console.WriteLine("^ Actual key ^");


            /*if (buildKeyTable.GuessKeyFromFrequenc(treshold, 1000))
            {
                found++;
            }*/
            //buildKeyTable.LinearEquationsTest();
        }

        static void doStatistics(Statistics statistics, Key key)
        {
            //statistics.testKey(key);
            //statistics.testKeyLinear(key);
            //statistics.testKeyLinearWithDifference(key, Settings.keyLenght);
            //statistics.testSumAllKeyBytes(key);
            statistics.testReducedC(key);
        }

        static bool doCombinedAttack(RC4 cipher, Key key)
        {
            CombinedAttack attack = new CombinedAttack(cipher,key.Length,key.Length);
            return attack.Attack(4,16);
        }

        static double combinedAttackPartOfkeyObtained(RC4 cipher, Key key)
        {
            CombinedAttack attack = new CombinedAttack(cipher, key.Length, key.Length);
            int[] guessedKey = attack.getFirstSuggestions(attack.guessKeyBasedOnJs());

            int correct = 0;
            for (int i = 0; i < key.Length; i++)
            {
                if (guessedKey[i] == key[i]) correct++;
            }

            return (double)correct / key.Length;
        }

        static void Main(string[] args)
        {
            Settings.setLogging();
            Random rnd = new Random(5);
            const int TEST_ROUNDS = Settings.rounds;
            byte keyLength = Settings.keyLenght;

            int treshold = 2;
            int found = 0;

            Key firstKey = new Key(rnd, keyLength);
            //Key firstKey = new Key(new int[] { 11, 21, 31, 41, 51 });

            RC4 cipher = new RC4(firstKey);
            Statistics statistics = new Statistics(cipher);
            
            int roundsCounter = TEST_ROUNDS;
            double probabilitiesSummed = 0;
            int sucessCounter = 0;
            while (roundsCounter > 0)
            {
                if (roundsCounter % (TEST_ROUNDS / 10) == 0)
                {
                    Console.WriteLine(roundsCounter / (TEST_ROUNDS / 10));
                }
               // Key key = new Key(new int[] { 0xA0, 0xB0, 0xC0, 0xD0, 0xE0 });
                Key key = new Key(rnd,keyLength);
                cipher.KSA(key);
                //doStatistics(statistics, key);
                //buildKeyAttact(key);

                if (doCombinedAttack(cipher,key))
                    sucessCounter++;
                //probabilitiesSummed += combinedAttackPartOfkeyObtained(cipher, key);
                roundsCounter--;
            }
            Console.WriteLine((double)sucessCounter / TEST_ROUNDS);

            Console.WriteLine(probabilitiesSummed / TEST_ROUNDS);
            statistics.calculateResults();

            //Console.WriteLine("{0}/{1} found with treshold {2} and key lenght {3}",found, TEST_ROUNDS, treshold, keyLength);

            Console.WriteLine("total depth = {0}", statistics.resutlsTotalDepth);
            /* cipher.Encrypt("Plaintext");
            cipher.GetStateAfterKSA/**/
            Console.Write("dataSumOnPosition = ");
            Log.WeightsArray(statistics.resultsSumOnPosition);/**/
           
            Console.Write("dataSLinear = ");
            Log.WeightsArray(statistics.resultsSLinear);
            /*Console.Write("dataInvSLinear = ");
            Log.WeightsArray(statistics.resultsInvSLinear);/**/

            /*Console.Write("dataAll = ");
            Log.WeightsArray(statistics.resultsSomewhere/**/
            Console.Write("dataS = ");
            Log.WeightsArray(statistics.resultsS);/*
            Console.Write("dataSS = ");
            Log.WeightsArray(statistics.resultsSS);
            Console.Write("dataSSS = ");
            Log.WeightsArray(statistics.resultsSSS);
            Console.Write("dataSSSS = ");
            Log.WeightsArray(statistics.resultsSSSS);/**/
            Console.Write("dataInvS = ");
            Log.WeightsArray(statistics.resultsInvS);
            /*Console.Write("dataInvSInvS = ");
            Log.WeightsArray(statistics.resultsInvSInvS);
            Console.Write("dataInvSInvSInvS = ");
            Log.WeightsArray(statistics.resultsInvSInvSInvS);
            Console.Write("dataInvSInvSInvSInvS = ");
            Log.WeightsArray(statistics.resultsInvSInvSInvSInvS);*/
            //Log.WeightsArray(statistics.resultsSinvS);
            /*Console.Write("dataSomewhere = ");
            Log.WeightsArray(statistics.resultsSomewhere);/**/


        Console.Read();
        }
    }
}

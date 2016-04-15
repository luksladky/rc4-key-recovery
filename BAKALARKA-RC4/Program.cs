using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BAKALARKA_RC4
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Random rnd = new Random(1);
            //RC4.logKeyVerification = true;
            //BuildKeyTable.logBuildKeyTableMinFreq = 3;
            BuildKeyTable.logFrequencyList = false;
            BuildKeyTableImproved.logBuildKeyTableMinFreq = 0.00008;

            const int TEST_ROUNDS = 10000000;

            int roundsCounter = TEST_ROUNDS;
            byte keyLength = 5;
            int treshold = 2;
            int found = 0;
            
            RC4 cipher = new RC4(new Key(rnd,keyLength));
            Statistics statistics = new Statistics(cipher);

            while (roundsCounter > 0)
            {
                
                //RC4.logAfterKSAPerm = true;
                //Key key = new Key(new int[] {106,59,220,65,34});
                Key key = new Key(rnd,keyLength);
                statistics.testKey(key);
                //Key key = new Key("aaaaaa");
                

                /*BuildKeyTableImproved buildKeyTable = new BuildKeyTableImproved(cipher);

                //cipher.Encrypt("Plaintext");
                
                Log.Key(key);
                Console.WriteLine("^ Actual key ^");

                //buildKeyTable.BuildKeyTable();
                if (buildKeyTable.GuessKeyFromFrequenc(treshold, 1000))
                {
                    found++;
                }
                //buildKeyTable.LinearEquationsTest();

                
                //Console.Read();*/


                roundsCounter--;
            }

            statistics.calculateResults();

            //Console.WriteLine("{0}/{1} found with treshold {2} and key lenght {3}",found, TEST_ROUNDS, treshold, keyLength);


            /* cipher.Encrypt("Plaintext");
            cipher.GetStateAfterKSA();*/
            Console.Write("dataS = ");
            Log.WeightsArray(statistics.resultsS);
            Console.Write("dataSS = ");
            Log.WeightsArray(statistics.resultsSS);
            Console.Write("dataSSS = ");
            Log.WeightsArray(statistics.resultsSSS);
            Console.Write("dataSSSS = ");
            Log.WeightsArray(statistics.resultsSSSS);
            Console.Write("dataInvS = ");
            Log.WeightsArray(statistics.resultsInvS);
            Console.Write("dataInvSInvS = ");
            Log.WeightsArray(statistics.resultsInvSInvS);
            Console.Write("dataInvSInvSInvS = ");
            Log.WeightsArray(statistics.resultsInvSInvSInvS);
            Console.Write("dataInvSInvSInvSInvS = ");
            Log.WeightsArray(statistics.resultsInvSInvSInvSInvS);
            //Log.WeightsArray(statistics.resultsSinvS);
            Console.Read();
        }
    }
}

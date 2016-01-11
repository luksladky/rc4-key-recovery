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
            //Attacks.logBuildKeyTableMinFreq = 3;
            Attacks.logFrequencyList = false;

            const int TEST_ROUNDS = 1000;

            int rounds_counter = TEST_ROUNDS;
            byte keyLength = 5;
            int treshold = 2;
            int found = 0;
            while (rounds_counter > 0)
            {
                
                //RC4.logAfterKSAPerm = true;
                //Key key = new Key(new int[] {106,59,220,65,34});
                Key key = new Key(rnd,keyLength);
                //Key key = new Key("aaaaaa");
                RC4 cipher = new RC4(key);

                Attacks attack = new Attacks(cipher);

                //cipher.Encrypt("Plaintext");
                
                //Log.Key(key);
                //Console.WriteLine("^ Actual key ^");

                //attack.BuildKeyTable();
                if (attack.GuessKeyFromFrequenc(treshold, 1000))
                {
                    found++;
                }
                //attack.LinearEquationsTest();

                rounds_counter--;
                //Console.Read();
            }

            Console.WriteLine("{0}/{1} found with treshold {2} and key lenght {3}",found, TEST_ROUNDS, treshold, keyLength);


            /* cipher.Encrypt("Plaintext");
            cipher.GetStateAfterKSA();*/

            Console.Read();
        }
    }
}

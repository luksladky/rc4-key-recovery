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
            RC4.logKeyVerification = true;

            while (true)
            {
                
                //RC4.logAfterKSAPerm = true;
                //Key key = new Key(new int[] {106,59,220,65,34});
                Key key = new Key(rnd,5);
                //Key key = new Key("aaaaaa");
                RC4 cipher = new RC4(key);

                cipher.VerifyKey(key);



                //cipher.Encrypt("Plaintext");
                //cipher.BuildKeyTable();
                //cipher.LinearEquationsTest();
            }



           /* cipher.Encrypt("Plaintext");
            cipher.GetStateAfterKSA();*/


        }
    }
}

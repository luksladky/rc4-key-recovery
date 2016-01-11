using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class RC4 : BaseClass
    {


        protected int i;
        protected int j;

        public Key K;

        protected int rounds;

        public int[] savedS;

        public static bool logEncryption       = false;
        public static bool logAfterKSAPerm     = false;
        public static bool logPRGA             = false;
        public static bool logKeyVerification  = false;

        public RC4(Key key) : base()
        {
            init(key);
            KSA();
        }

        protected void init(Key key)
        {
            
            savedS = new int[N];
            K = key;
        }

        public void KSA(Key key)
        {
            init(key);
            KSA();
        }

        private void KSA()
        {
            PrepareInnerState();
            
            for (int k = 0; k < N; k++)
            {
                savedS[k] = S[k];
            }

            if (logAfterKSAPerm)
            {
                Log.writeOutState(S, i, j);
            }
        }

        private void PrepareInnerState()
        {
            rounds = 0;

            for (i = 0; i < N; i++)
            {
                S[i] = i;
            }

            for (j = i = 0; i < N; i++)
            {
                j = (j + S[i] + K[i]) % N;
                Swap(i, j);
            }

            i = j = 0;
        }

        public int PRGANextByte()
        {
            rounds++;

            i = (i + 1) % N;
            j = (j + S[i]) % N;
            Swap(i,j);
            int t = (S[i] + S[j]) % N;

            if (logPRGA)
            {
                Console.WriteLine("Next byte: {0:X2}, i={1}, j={2}, round {3}",S[t],i,j, rounds);
            }

            return S[t];


        }

        public string Encrypt(string plaintext)
        {
            int data_length = plaintext.Length;
            byte[] ciphertext = new byte[data_length];
            for (int k = 0; k < data_length; k++)
            {
                ciphertext[k] = (byte) (PRGANextByte() ^ Utils.ord(plaintext[k]));
            }

            if (logEncryption)
            {
                Console.Write("encrypted text: ");
                Log.ByteArray(ciphertext);
            }
           

            return Encoding.GetEncoding(1252).GetString(ciphertext);
        }

        public string Decrypt(string ciphertext)
        {
            KSA();
            return Encrypt(ciphertext);
            
        }

        public bool VerifyKey(Key key)
        {
            PrepareInnerState();
            bool samePermutation = true;
            for (int k = 0; k < N; k++)
            {
                if (S[k] != savedS[k])
                {
                    samePermutation = false;
                    break;
                }
            }

            Console.WriteLine("{0}, {1} key", samePermutation, key.ToString());
            if (!samePermutation)
            {
                Log.writeOutState(S,i,j);
                Log.writeOutState(savedS,i,j);
            }

            return samePermutation;
        }

 
        public void GetStateAfterKSA()
        {
            for (int k = 0; k < rounds; k++)
            {
                GetPreviousState();
            }

            Log.writeOutState(S, i, j);
        }

        private void GetPreviousState()
        {
            Swap(i,j);
            j = (N + j - S[i])%N;
            i = (i - 1)% N;
        }
    }
}

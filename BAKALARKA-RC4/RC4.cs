using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class RC4
    {
        protected int N;

        protected int i;
        protected int j;
        protected int[] S;
        protected Key K;

        protected int rounds;

        protected int[] InvS;
        protected int[] savedS;

        public static bool logEncryption       = false;
        public static bool logAfterKSAPerm     = false;
        public static bool logPRGA             = false;
        public static bool logKeyVerification  = false;

        public RC4(Key key)
        {
            init(key);
            KSA();
        }

        protected void init(Key key)
        {
            N = Constants.N;
            S = new int[N];
            savedS = new int[N];
            InvS = new int[N];
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

        public void GetPreviousState()
        {
            Swap(i,j);
            j = (N + j - S[i])%N;
            i = (i - 1)% N;
        }


        protected void ConstructInvS()
        {
            for (int k = 0; k < N; k++)
            {
                InvS[S[k]] = k;
            }
        }


        public void BuildKeyTable()
        {
            int l = K.Length;
            int[] jarr1 = new int[N + 1];
            int[] jarr2 = new int[N + 1];
            int[,] karr = new int[l,N];

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
                    karr[w, y] = 0;
                }
            }
            for (int y = 0; y < N; y++)
            {
                karr[y%l, mod(jarr1[y + 1] - jarr1[y] - y,N)]++;
                karr[y%l, mod(jarr1[y + 1] - jarr2[y] - y,N)]++;
                karr[y%l, mod(jarr2[y + 1] - jarr1[y] - y, N)]++;
                karr[y%l, mod(jarr2[y + 1] - jarr2[y] - y, N)]++;
            }

            Log.Key(K);
            //Log.FrequencyOfKeyByte(karr,0,1);
            Log.FrequencyTable(karr,1);
        }


        public void LinearEquationsTest()
        {
            for (int y = 0; y < 100; y++)
            {
                int sumOfKeyBytes = 0;
                for (int k = 0; k <= y; k++)
                {
                    sumOfKeyBytes += K[k];
                    
                }
                int fy = mod((y*(y + 1)/2) + sumOfKeyBytes,N);

                Console.Write("{0}: fy = {1,3}, Sy = {2,3}",y,fy,S[y]);
                if (fy == S[y])
                {
                    Console.Write(" <<<<<");
                }
                Console.WriteLine();
            }


        }





        int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        private void Swap(int i, int j)
        {

            int tmp = S[i];
            S[i] = S[j];
            S[j] = tmp;
        }

    }
}

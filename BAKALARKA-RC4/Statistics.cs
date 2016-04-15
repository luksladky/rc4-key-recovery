using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class Statistics
    {
        protected RC4 cipher;
        protected int N;

        protected int[] S;
        protected int[] InvS;

        private int[] Scounts;
        private int[] InvScounts;
        
        private int[] SScounts;
        private int[] InvSInvScounts;

        private int[] SSScounts;
        private int[] InvSInvSInvScounts;

        private int[] SSSScounts;
        private int[] InvSInvSInvSInvScounts;

       // private int[] SInvScounts;
        public double[] resultsS;
        public double[] resultsInvS;

        public double[] resultsSS;
        public double[] resultsInvSInvS;

        public double[] resultsSSS;
        public double[] resultsInvSInvSInvS;

        public double[] resultsSSSS;
        public double[] resultsInvSInvSInvSInvS;
        //public double[] resultsSinvS;

        private int iterations;

        public Statistics(RC4 cipher)
        {
            N = Constants.N;
            S = new int[N];
            iterations = 0;
            this.cipher = cipher;
            
            Scounts = new int[N];
            InvScounts = new int[N];

            SScounts = new int[N];
            InvSInvScounts = new int[N];

            SSScounts = new int[N];
            InvSInvSInvScounts = new int[N];

            SSSScounts = new int[N];
            InvSInvSInvSInvScounts = new int[N];

            //SInvScounts = new int[N];


        }

        public void testKey(Key key)
        {
            cipher.KSA(key);
            S = cipher.savedS;
            ConstructInvS();
            iterations++;

            for (int i = 0; i < N; i++)
            {
                if (S[i] == cipher.savedJs[i])
                    Scounts[i]++;
                if (S[S[i]] == cipher.savedJs[i])
                    SScounts[i]++;
                if (S[S[S[i]]] == cipher.savedJs[i])
                    SSScounts[i]++;
                if (S[S[S[S[i]]]] == cipher.savedJs[i])
                    SSSScounts[i]++;
                if (InvS[i] == cipher.savedJs[i])
                    InvScounts[i]++;
                if (InvS[InvS[i]] == cipher.savedJs[i])
                    InvSInvScounts[i]++;
                if (InvS[InvS[InvS[i]]] == cipher.savedJs[i])
                    InvSInvSInvScounts[i]++;
                if (InvS[InvS[InvS[InvS[i]]]] == cipher.savedJs[i])
                    InvSInvSInvSInvScounts[i]++;

                /*if (S[InvS[i]] == cipher.savedJs[i])
                    SInvScounts[i]++;*/
            }
        }

        public void calculateResults()
        {
            resultsS = new double[N];
            resultsInvS = new double[N];

            resultsSS = new double[N];
            resultsInvSInvS = new double[N];

            resultsSSS = new double[N];
            resultsInvSInvSInvS = new double[N];
            
            resultsSSSS = new double[N];
            resultsInvSInvSInvSInvS = new double[N];
            //resultsSinvS = new double[N];

            for (int i = 0; i < N; i++)
            {
                resultsS[i] = ((float) Scounts[i])/iterations;
                resultsSS[i] = ((float)SScounts[i]) / iterations;
                resultsSSS[i] = ((float) SSScounts[i])/iterations;
                resultsSSSS[i] = ((float)SSSScounts[i]) / iterations;
                resultsInvS[i] = ((float)InvScounts[i]) / iterations;
                resultsInvSInvS[i] = ((float)InvSInvScounts[i]) / iterations;
                resultsInvSInvSInvS[i] = ((float)InvSInvSInvScounts[i]) / iterations;
                resultsInvSInvSInvSInvS[i] = ((float)InvSInvSInvSInvScounts[i]) / iterations;
                //resultsSinvS[i] = ((float)SInvScounts[i]) / iterations;
            }
        }



        protected void ConstructInvS()
        {
            InvS = new int[N];
            for (int k = 0; k < N; k++)
            {
                InvS[S[k]] = k;
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class Statistics : BaseClass
    {
        protected RC4 cipher;
        protected int N;
        protected int keyLength;

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

        private int[] SLinearCounts;
        private int[] InvSLinearCounts;

        private int CholdsCount;
        private int CholdsCountAndSiLeqi;

        private int[] foundSumOnPosition;

        private int[] somewhereCounts;

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

        public double[] resultsSLinear;
        public double[] resultsInvSLinear;

        public double[] resultsSomewhere;

        public double[] resultsSumOnPosition;

        private int iterations;

        private double[] wDiffL;
        private double[] wDiff2L;
        private double[] wDiff3L;
        private double[] wDiff4L;
        private double[] wDiff5L;
  
        public Statistics(RC4 cipher)
        {
            N = Constants.N;
            S = new int[N];
            iterations = 0;
            this.cipher = cipher;
            this.keyLength = cipher.K.Length;
            
            Scounts = new int[N];
            InvScounts = new int[N];

            SScounts = new int[N];
            InvSInvScounts = new int[N];

            SSScounts = new int[N];
            InvSInvSInvScounts = new int[N];

            SSSScounts = new int[N];
            InvSInvSInvSInvScounts = new int[N];

            SLinearCounts = new int[N];
            InvSLinearCounts = new int[N];

            //SInvScounts = new int[N];

            somewhereCounts = new int[N];

            foundSumOnPosition = new int[N];

            wDiffL = Weights.getWeightsSumDiff(Settings.keyLenght);
            wDiff2L = Weights.getWeightsSumDiff(2*Settings.keyLenght);
            wDiff3L = Weights.getWeightsSumDiff(2 * Settings.keyLenght);
            wDiff4L = Weights.getWeightsSumDiff(2 * Settings.keyLenght);
            wDiff5L = Weights.getWeightsSumDiff(2 * Settings.keyLenght);

        }

        private void initTest(Key key)
        {
            cipher.KSA(key);
            S = cipher.savedS;
            ConstructInvS();
            iterations++;
        }

        private int[] initKeySequence(Key key)
        {
            int[] keySumZeroTo = new int[N + 1];
            keySumZeroTo[0] = key[0];

            for (int i = 0; i < N; i++)
            {
                keySumZeroTo[i + 1] = mod(keySumZeroTo[i] + key[i + 1], N);
            }
            //Log.Array(keySumZeroTo);

            return keySumZeroTo;
        }

        public void testKeyLinear(Key key)
        {
            initTest(key);
            int[] Ksum = initKeySequence(key);

            for (int i = 0; i < N; i++)
            {
                if (Ksum[i] == mod(S[S[S[i]]] - ((i)*(i+1)/2), N))
                {
                    SLinearCounts[i]++;
                }

                //no bias
                /*if (Ksum[i] == mod(InvS[i] - (i * (i + 1) / 2), N)) {
                    InvSLinearCounts[i]++;
                }*/
            }
        }

        public void testKeyLinearWithDifference(Key key, int diff)
        {
            initTest(key);
            int[] Ksum = initKeySequence(key);

            for (int i1 = 0; i1 < N - diff; i1++)
            {
                int i2 = i1 + diff;

                int KsumCurr = 0;
                for (int j = 0; j < i2 - i1; j++)
                {
                    KsumCurr += key[i1 + j + 1];
                    KsumCurr = mod(KsumCurr, N);
                }

                int Ci1 = S[i1] - (i1) * (i1 + 1) / 2;
                int Ci2 = S[i2] - (i2) * (i2 + 1) / 2;

                int InvCi1 = InvS[i1] - (i1) * (i1 + 1) / 2;
                int InvCi2 = InvS[i2] - (i2) * (i2 + 1) / 2;

                int diffCi = mod(S[i2] - S[i1] - (i2) * (i2 + 1) / 2 + (i1) * (i1 + 1) / 2, N);
                int diffInvCi = mod(InvS[i2] - InvS[i1] - (i2) * (i2 + 1) / 2 + (i1) * (i1 + 1) / 2, N);

                if (diffCi == KsumCurr)
                {
                    CholdsCount++;
                    //if (S[i1] < i1 || S[i2] < i2) CholdsCountAndSiLeqi++;
                    SLinearCounts[i1]++;
                }

                if (diffInvCi == KsumCurr)
                {
                    if (S[i1] < i1 & S[i1] > i2  || S[i2] < i2) CholdsCountAndSiLeqi++;
                    InvSLinearCounts[i1]++;
                }
                //Console.WriteLine("sum {0}, in array {1}", KsumCurr, Ksum[i2] - Ksum[i1]);
             }
        }

        public void testKey(Key key)
        {
            initTest(key);

            for (int i = 0; i < N; i++)
            {
                bool foundSomewhere = false;

                //if (S[i] == cipher.savedJs[i])
                if (S[i] < i)
                {
                    foundSomewhere = true;
                    //if (S[i] > i) 
                        Scounts[i]++;
                }
                if (S[S[i]] == cipher.savedJs[i])
                {
                    //foundSomewhere = true;
                    SScounts[i]++;
                }
                if (S[S[S[i]]] == cipher.savedJs[i])
                {
                    //foundSomewhere = true;
                    SSScounts[i]++;
                }
                if (S[S[S[S[i]]]] == cipher.savedJs[i])
                {
                    //foundSomewhere = true;
                    SSSScounts[i]++;
                }
                //if (InvS[i] == cipher.savedJs[i])
                if (InvS[i] > i)
                {
                    foundSomewhere = true;
                    //if (InvS[i] < i)
                        InvScounts[i]++;
                }
                if (InvS[InvS[i]] == cipher.savedJs[i])
                {
                    //foundSomewhere = true;
                    InvSInvScounts[i]++;
                }
                if (InvS[InvS[InvS[i]]] == cipher.savedJs[i])
                {
                    //foundSomewhere = true;
                    InvSInvSInvScounts[i]++;
                }
                if (InvS[InvS[InvS[InvS[i]]]] == cipher.savedJs[i])
                {
                    //foundSomewhere = true;
                    InvSInvSInvSInvScounts[i]++;
                }

                if (foundSomewhere)
                    somewhereCounts[i]++;
                /*if (S[InvS[i]] == cipher.savedJs[i])
                    SInvScounts[i]++;*/
            }
        }

        public void addWeightsDiff(int diff,double[] weights,double[] probabilities,double prob_th)
        {
            int i1 = 0;
            while (i1 + diff < N & wDiffL[i1] > prob_th)
            {
                int i2 = i1 + diff;
                int diffCi = mod(S[i2] - S[i1] - (i2) * (i2 + 1) / 2 + (i1) * (i1 + 1) / 2, N);

                if (S[i2] >= i2 && S[i1] >= i1)
                    weights[diffCi] += probabilities[i1];
                i1++;
            }
        }

        

    public int findDiv(int result, int times)
    {
        int guess = 0;
        while (mod(guess * times, N) != result)
        {
            guess++;
            if (guess == N)
            {
                return -1;
            }
        }
        return guess;
    }

    public double[] getSumAllKeyBytesCounter(int l) {

            double prob_th = Settings.prob_th;
            var sumGuess = new double[N];

            int index = l - 1;
            int iteration = 1;
            while (Weights.sumS[index] > prob_th)
            {
                bool failed = false;
                if (S[index] >= index)
                {
                    int keySumSuggestion = mod(S[index] - (index + 1) * (index) / 2, N);
                    int guess = keySumSuggestion;
                    if (iteration > 1)
                        guess = findDiv(keySumSuggestion, iteration);

                    if (guess > -1)
                        sumGuess[guess] += Weights.sumS[index];
                }
                index += l;
                iteration++;
            }/**/

            iteration = 1;
            index = l - 1;
            while (Weights.sumSS[index] > prob_th)
            {
                bool failed = false;

                int keySumSuggestion = mod(S[S[index]] - (index + 1) * (index) / 2, N);
                int guess = keySumSuggestion;
                if (iteration > 1)
                    guess = findDiv(keySumSuggestion, iteration);

                if (guess > -1)
                    sumGuess[guess] += Weights.sumSS[index];

                index += l;
                iteration++;
            }

            iteration = 1;
            index = l - 1;
            while (Weights.sumSSS[index] > prob_th)
            {
                bool failed = false;

                int keySumSuggestion = mod(S[S[S[index]]] - (index + 1) * (index) / 2, N);
                int guess = keySumSuggestion;
                if (iteration > 1)
                    guess = findDiv(keySumSuggestion, iteration);

                if (guess > -1)
                    sumGuess[guess] += Weights.sumSSS[index];

                index += l;
                iteration++;
            }/**/

            int i1 = 0;
            while (i1 + l < N & wDiffL[i1] > prob_th)
            {
                int i2 = i1 + l;
                int diffCi = mod(S[i2] - S[i1] - (i2) * (i2 + 1) / 2 + (i1) * (i1 + 1) / 2, N);

                if (S[i2] >= i2 && S[i1] >= i1)
                    sumGuess[diffCi] += wDiffL[i1];/**/
                //sumGuess[diffCi] += Constants.PCC16[i1];
                i1++;
            }


            i1 = 0;
            while (i1 + 2 * l < N & wDiff2L[i1] > prob_th)
            {
                int i2 = i1 + 2 * l;

                int diffCi = mod(S[i2] - S[i1] - (i2) * (i2 + 1) / 2 + (i1) * (i1 + 1) / 2, N);

                if (diffCi % 2 == 0)
                {
                    if (S[i2] >= i2 && S[i1] >= i1)
                        sumGuess[diffCi / 2] += wDiff2L[i1];
                }
                i1++;
            }

            i1 = 0;
            while (i1 + 3 * l < N & wDiff3L[i1] > prob_th)
            {
                int i2 = i1 + 3 * l;

                int diffCi = mod(S[i2] - S[i1] - (i2) * (i2 + 1) / 2 + (i1) * (i1 + 1) / 2, N);

                if (S[i2] >= i2 && S[i1] >= i1)
                {
                    int guess = 0;
                    while (mod(guess * 3, N) != diffCi & guess < N - 1)
                    {
                        guess++;
                    }
                    sumGuess[guess] += wDiff3L[i1];

                }
                i1++;
            }/**/

            return sumGuess;
        }

        public int[] getSumAllKeyBytesSuggestionsSorted(int l)
        {
            double[] sumGuess = getSumAllKeyBytesCounter(l);
            int[] suggestions = new int[Constants.sumAllKeyBytesDepth];
            for (int i = 0; i < Constants.sumAllKeyBytesDepth; i++)
            {
                double max = -1;
                int maxIndex = 0;

                for (int j = 0; j < N; j++)
                {
                    if (sumGuess[j] > max)
                    {
                        max = sumGuess[j];
                        maxIndex = j;
                    }
                }
                suggestions[i] = maxIndex;
                sumGuess[maxIndex] = -1;
            }
            return suggestions;
        }

        public void testSumAllKeyBytes(Key key)
        {
            initTest(key);
            double[] sumGuess = getSumAllKeyBytesCounter(key.Length);

            int Ksum = 0;
            for (int j = 0; j < key.Length; j++)   
                Ksum = mod(Ksum + key[j], N);

            int[] suggestionsForSum = getSumAllKeyBytesSuggestionsSorted(key.Length); 
            for (int i = 0; i < Constants.sumAllKeyBytesDepth; i++)
            {
                if (Ksum == suggestionsForSum[i])
                { 
                    foundSumOnPosition[i]++;
                    return;
                }
            }
        }

        public int getSumOfKeyBytesFromTo(Key key,int from, int to)
        {
            int Ksum = 0;
            for (int j = from; j <= to; j++)
                Ksum = mod(Ksum + key[j], N);
            return Ksum;
        }

        public void testReducedC(Key key)
        {
            initTest(key);


            int l = key.Length;
            int[] s = getSumAllKeyBytesSuggestionsSorted(key.Length);
            int Ksum = getSumOfKeyBytesFromTo(key, 0, l - 1);

            if (Ksum != s[0]) return;
            int[] C = new int[N];

            for (int i = 0; i < N; i++)
            {
                C[i] = mod(S[i] - i * (i + 1) / 2, N);
                //C[i] = getSumOfKeyBytesFromTo(key, 0, i);
            }

            int[,] maximum = new int[l, l];
            double[,,] counter = new double[l,l, N];

            double[,] jGuess = guessKeyBasedOnJs();

            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    counter[i, i, j] = jGuess[i, j];
                    if (counter[i, i, maximum[i,i]] < counter[i, i, j])
                        maximum[i, i] = j;
                }
            }


            int lambda = 0; //number of s to be substraced (minus lambda*s)
            for (int i = 1; i < N; i++)
            {
                if (Weights.sumS[i] >= Settings.prob_th)
                {
                    if (S[i] >= i)
                    {
                        int value = mod(C[i] - lambda * s[0], N);
                        counter[0, i % l, value] += Weights.sumS[i];
                        if (counter[0, i % l, maximum[0, i % l]] < counter[0, i % l, value])
                            maximum[0, i % l] = value;
                    }
                        
                }

                if (Weights.sumSS[i] >= Settings.prob_th)
                {
                    int value = mod(S[S[i]] - i * (i + 1) / 2 - lambda * s[0], N);

                    counter[0, i % l, value] += Weights.sumSS[i];
                    if (counter[0, i % l, maximum[0, i % l]] < counter[0, i % l, value])
                        maximum[0, i % l] = value;
                }

                if (Weights.sumSS[i] >= Settings.prob_th)
                {
                    int value = mod(S[S[i]] - i * (i + 1) / 2 - lambda * s[0], N);

                    counter[0, i % l, value] += Weights.sumSSS[i];
                    if (counter[0, i % l, maximum[0, i % l]] < counter[0, i % l, value])
                        maximum[0, i % l] = value;
                }

                if (i % l == l-1) lambda++;
            }

            for (int diff = 2; diff <= l; diff++)
            {
                double[] w = Weights.getWeightsSumDiff(diff);
                for (int i1 = 0; i1 < N - diff; i1++)
                {
                    if (w[i1] < Settings.prob_th) break;

                    int i2 = i1 + diff;
                    if (S[i2] >= i2 && S[i1] >= i1)
                    {
                        int value = mod(C[i2] - C[i1], N);
                        int firstIndex = (i1 + 1) % l;
                        int secondIndex = i2 % l;

                        //if (firstIndex <= secondIndex) continue;

                        //normalize
                        if (secondIndex < firstIndex)
                        {
                            firstIndex = (i2 + 1) % l;
                            secondIndex = i1 % l;
                            value = mod(s[0] - value);
                        }        

                        //int sum = getSumOfKeyBytesFromTo(key,i1+1,i2);
                        counter[firstIndex, secondIndex,value ] += w[i1];
                        if (counter[firstIndex, secondIndex, maximum[firstIndex, secondIndex]] < counter[firstIndex, secondIndex, value])
                            maximum[firstIndex, secondIndex] = value;
                    }

                }
            }


            int[,] correct = new int[l, l];
            for (int i = 0; i < l; i++)
            {
                for (int j = i; j < l; j++)
                {
                    correct[i, j] = getSumOfKeyBytesFromTo(key, i, j);
                    Console.WriteLine("K[{2}...{3}] --- {0} ma byt {1} ctr: {4}", maximum[i, j], correct[i, j],i,j,counter[i,j,maximum[i,j]]);

                }
            }



            for (   int i = 0; i < l; i++)
            {
                bool found = false;
                for (int nc = 0; nc < 256; nc++)
                {
                    double max = -1;
                    int maxIndex = 0;
                    for (int j = 0; j < N; j++)
                    {
                        if (counter[0,i,j] > max)
                        {
                            maxIndex = j;
                            max = counter[0,i, j];
                        }
                    }
                    counter[0,i, maxIndex] = -1;
                    int Ksumi = getSumOfKeyBytesFromTo(key, 0, i);
                    if (Ksumi == maxIndex)
                    {
                        found = true;
                        Log.v(nc);
                        continue;
                    }

                }
                if (!found)
                    Log.v("...");
            }

            Log.v("NEXT");
    }

        public int J(int round, int nesting, bool inverse)
        {
            if (round == 0) return 0;
            if (round > N || round < 0)
                return -1;

            switch (nesting)
            {
                case 2:
                    return (inverse) ? InvS[InvS[round - 1]] : S[S[round - 1]];
                case 3:
                    return (inverse) ? InvS[InvS[InvS[round - 1]]] : S[S[S[round - 1]]];
                case 4:
                    return (inverse) ? InvS[InvS[InvS[InvS[round - 1]]]] : S[S[S[S[round - 1]]]];
                default:
                    return (inverse) ? InvS[round - 1] : S[round - 1];
            }
        }

        public double weightJ(int round, int nesting, bool inverse)
        {
            if (round == 0) return 0;
            if (round > N || round < 0)
                return 0;
            double weight;
            switch (nesting)
            {
                case 2:
                    weight = (inverse) ? Weights.JInvSInvS[round - 1] : Weights.JSS[round - 1];
                    break;
                case 3:
                    weight = (inverse) ? Weights.JInvSInvSInvS[round - 1] : Weights.JSSS[round - 1];
                    break;
                case 4:
                    weight = (inverse) ? Weights.JInvSInvSInvSInvS[round - 1] : Weights.JSSSS[round - 1];
                    break;
                default:
                    weight = (inverse) ? Weights.JInvS[round - 1] : Weights.JS[round - 1];
                    break;
            }
            return weight;
            /*double tresholdHigh = 0.05;
            double tresholdLow = 0.008;

            return (weight > tresholdHigh) ? 2 : (weight < tresholdLow ? 0 : 1);*/
        }

        public double[,] guessKeyBasedOnJs()
        {
            double[,] guess = new double[keyLength, N];
            //TODO pro vsechna opakovani


            for (int y = 0; y < N; y++) //position
            {

                int first = (y == 0) ? 0 : S[y - 1];
                int second = S[y];

                int firstInv = (y == 0) ? 0 : InvS[y - 1];
                int secondInv = InvS[y];

                int maxnesting = 1;
                //nesting first
                for (int i = 1; i <= maxnesting; i++)
                {
                    //nesting second
                    for (int j = 1; j <= maxnesting; j++)
                    {
                        //all combinations of J
                        if ((i > 1 | second >= y) & (j > 1 | (y == 0 | first >= y - 1)))
                            guess[y % keyLength, mod(J(y + 1, i, false) - J(y, j, false) - y, N)] += weightJ(y + 1, i, false) * weightJ(y, j, false);
                        if ((i > 1 | second >= y) & (j > 1 | (y == 0 | firstInv <= y - 1)))
                            guess[y % keyLength, mod(J(y + 1, i, false) - J(y, j, true) - y, N)] += weightJ(y + 1, i, false) * weightJ(y, j, true);
                        if ((i > 1 | secondInv <= y) & (j > 1 | (y == 0 | first >= y - 1)))
                            guess[y % keyLength, mod(J(y + 1, i, true) - J(y, j, false) - y, N)] += weightJ(y + 1, i, true) * weightJ(y, j, false);
                        if ((i > 1 | secondInv <= y) & (j > 1 | (y == 0 | firstInv <= y - 1)))
                            guess[y % keyLength, mod(J(y + 1, i, true) - J(y, j, true) - y, N)] += weightJ(y + 1, i, true) * weightJ(y, j, true);
                    }
                }
            }

            return guess;
        }

        public void testFilters()
        {

        }

        public int[] getFirstSuggestions(double[,] counter)
        {

            int[] guessedKey = new int[keyLength];
            for (int i = 0; i < keyLength; i++)
            {
                double maxWeight = -1;
                int maxWeightIndex = 0;

                for (int j = 0; j < N; j++)
                {
                    if (counter[i, j] > maxWeight)
                    {
                        maxWeight = counter[i, j];
                        //guess[i, j] = 0;
                        maxWeightIndex = j;
                    }
                }
                guessedKey[i] = maxWeightIndex;

            }



            return guessedKey;
        }

        public void calculateResults()
        {
            Console.WriteLine("{0}/{1}", CholdsCountAndSiLeqi, CholdsCount);
            resultsS = new double[N];
            resultsInvS = new double[N];

            resultsSS = new double[N];
            resultsInvSInvS = new double[N];

            resultsSSS = new double[N];
            resultsInvSInvSInvS = new double[N];
            
            resultsSSSS = new double[N];
            resultsInvSInvSInvSInvS = new double[N];
            //resultsSinvS = new double[N];

            resultsSomewhere = new double[N];

            resultsSLinear = new double[N];
            resultsInvSLinear = new double[N];

            resultsSumOnPosition = new double[N];

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
                resultsSomewhere[i] = ((float) somewhereCounts[i] / iterations);

                resultsSLinear[i] = ((float)SLinearCounts[i] / iterations);
                resultsInvSLinear[i] = ((float)InvSLinearCounts[i] / iterations);

                resultsSumOnPosition[i] = ((float)foundSumOnPosition[i] / iterations);
                //resultsSinvS[i] = ((float)SInvScounts[i]) / iterations;
            }

            double total = 0;
            for (int i = 0; i < N; i++)
            {
                total += resultsSumOnPosition[i];
            }
            Console.Write(total);
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

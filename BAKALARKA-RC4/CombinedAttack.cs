using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    public struct KeyByteWeight
    {
        public int keyByte;
        public double weight;

        public KeyByteWeight(int keyByte, double weight)
        {
            this.keyByte = keyByte;
            this.weight = weight;
        }
    }

    public class KeyByteWeightComparer : IComparer<KeyByteWeight>
    {

        public int Compare(KeyByteWeight x, KeyByteWeight y)
        {
            if (x.weight == y.weight)
                return 0;
            if (x.weight > y.weight)
                return -1;

            return 1;
        }
    }

    class CombinedAttack : Attacks
    {
        int keyLength;
        int m;
        int nc;
        private double[] wDiffL;
        private double[] wDiff2L;
        private double[] wDiff3L;
        int[] sCandidates;
        int s;
        int[] keyJ;
        double[,,] initialCounter;
        double[,] keyJCounter;
        double[,,] keySequenceCounter;
        double[,,] currCounter;
        int[] C;
        int[] InvC;
        List<bool[]> allCombinations;
        int numberOfCombinations;
        double prob_th;

        int[] recoveredKey;

        public CombinedAttack(RC4 cipher, int keyLength, int m) : base(cipher)
        {
            this.keyLength = keyLength;
            this.m = m;
            this.prob_th = Settings.prob_th;

            ConstructInvS();

            wDiffL = Weights.getWeightsSumDiff(Settings.keyLenght);
            wDiff2L = Weights.getWeightsSumDiff(2 * Settings.keyLenght);
            wDiff3L = Weights.getWeightsSumDiff(2 * Settings.keyLenght);
        }

        
        public bool Attack(int m, int nc)
        {
            this.m = m;
            this.nc = nc;
            

            C = new int[N];
            InvC = new int[N];

            allCombinations = new List<bool[]> { };
            numberOfCombinations = Binomial(keyLength, m);
            generateCombinations(keyLength, m);

            for (int i = 0; i < N; i++)
            {
                C[i] = mod(S[i] - i * (i + 1) / 2, N);
                InvC[i] = mod(InvS[i] - i * (i + 1) / 2, N);
            }

            initializeCounters();

            keyJ = getFirstSuggestions(keyJCounter);

            
            for (int d = 0; d < Constants.sumAllKeyBytesDepth; d++)
            {
                s = sCandidates[d];
                //double[,] counters = reduceC(s[d]);
                currCounter = updateCounterWithAllSum();

                for (int c = 0; c < numberOfCombinations; c++)
                {
                    bool[] fixedBytes = allCombinations.ElementAt(c);

                    int[] currFixedKey = new int[l];
                    Array.Copy(keyJ, currFixedKey, l);

                    //fixedBytes = new bool[10] { true, true, false, true, false, false, false, true, false, true };
                    //Log.Combination(fixedBytes);

                    //number of update groups
                    int updateGroupsCount = keyLength / Settings.updateGroupLength;
                    if (keyLength % Settings.updateGroupLength > 0) updateGroupsCount++;

                    List<int[]>[] keySuggestions = new List<int[]>[updateGroupsCount];

                    for (int i = 0; i < updateGroupsCount; i++)
                    {
                        int startingPosition = i * Settings.updateGroupLength;
                        List<int[]> keyPart = new List<int[]> { };

                        updateGroup(startingPosition,startingPosition,fixedBytes, currFixedKey, keyPart);

                        keySuggestions[i] = keyPart;

                        
                        /*for (int j = 0; j < Math.Min(10,keyPart.Count); j++)
                        {
                            bool same = true;
                            for (int k = 0; k < 4; k++)
                            {
                                if (cipher.K[startingPosition + k] != keyPart[j][k])
                                    same = false;
                            }
                            if (same)
                            {
                                Log.v("ano");
                            } else
                            {
                                Log.v("ne");
                            }
                        }/**/
                    }
                    if (testKeys(0, updateGroupsCount, new int[keyLength], keySuggestions))
                        return true;
                }
            }

            Log.v("false");
            return false;
        }

        //public void updateMethod(double[,,] counter,bool[] fixedBytes)

        
        public bool testKeys(int currGroup, int groupsCount,int[] testedKey, List<int[]>[] keyGroups)
        {
            int gl = Settings.updateGroupLength;
            if (currGroup == groupsCount)
            {
                if (testKey(testedKey))
                {
                    recoveredKey = testedKey;
                    Log.Key(new Key(testedKey));
                    return true;
                } 
                
                return false;
            }

            bool found = false;
            for (int i = 0; i < keyGroups[currGroup].Count; i++)
            {
                for (int j = 0; j < Math.Min(gl,keyLength-currGroup* gl); j++)
                {
                    testedKey[j+currGroup*gl] = keyGroups[currGroup][i][j];
                }
                if (testKeys(currGroup + 1, groupsCount, testedKey, keyGroups)) {
                    found = true;
                    break;
                }
            }

            return found;

            
        }    

        public bool testKey(int[] key)
        {
            bool same = true;
            for (int i = 0; i < keyLength; i++)
            {
                if (cipher.K[i] != key[i]) same = false;
            }

            return same;



        }


        public void updateGroup(int startingPosition, int curr_position, bool[] fixPositions, int[] curr_candidates, List<int[]> suggestions)
        {
            
            int index = curr_position;
            int maxPos = Math.Min(startingPosition + Settings.updateGroupLength,keyLength);

            while (index < maxPos)
            {
                if (fixPositions[index])
                    index++;
                else
                    break;                
            }
            if (index >= maxPos) //Add key u posledni
            {
                int[] key = new int[Settings.updateGroupLength];
                for (int i = 0; i < Settings.updateGroupLength; i++)
                {
                    if (i + startingPosition == maxPos) break;
                    key[i] = curr_candidates[i + startingPosition];
                }
                suggestions.Add(key);
            } else //update
            {
                fixPositions[index] = true;
                double[] counter = new double[N];
                int startIndex;
                int endIndex;

                findSumsUsedToUpdate(startingPosition, Math.Min(startingPosition + Settings.updateGroupLength - 1,keyLength-1), index, fixPositions, out startIndex, out endIndex); 
                for (int k = startIndex; k <= index; k++)
                {
                    for (int l = index; l <= endIndex; l++)
                    {
                        //Console.WriteLine("{0} - {1}", k, l);
                        for (int i = 0; i < N; i++)
                        {
                            int suggestion = i;
                            for (int other = k; other <= l; other++)
                            {
                                //sekvence uz na fixlych bytech, staci odecist ty, ktere nejsou ten hledany
                                if (other != index)
                                {
                                    suggestion = mod(suggestion - curr_candidates[other],N);
                                }
                            }
                            //if (k == l)
                            counter[suggestion] += currCounter[k, l, i];
                        } 
                    }
                }

                int[] candidates = selectCandidatesForByte(counter, nc);

                for (int i = 0; i < nc; i++)
                {
                    curr_candidates[index] = candidates[i];
                    updateGroup(startingPosition, index, fixPositions, curr_candidates, suggestions);
                }
            
                fixPositions[index] = false; //for other instances on the stack
            }

            
        }

        /*public double[,] reduceC(int s)
        {
            double[,] counter = new double[l,N];
            int lambda = 0; //number of s to be substraced (minus lambda*s)
            for (int i = 0; i < N; i++)
            {
                counter[i % l, mod(C[i] - lambda * s, N)] += Weights.sumS[i];
                if (i % l == 0) lambda++;
            }
            return counter;
        }*/

        public void initializeCounters()
        {
            sCandidates = guessSumValue();
            keyJCounter = guessKeyBasedOnJs();
            initialCounter = new double[l, l, N];

            for (int i = 0; i < l; i++)
                for (int j = 0; j < N; j++)
                    initialCounter[i, i, j] = keyJCounter[i, j];


            for (int diff = 2; diff <= l; diff++)
            {
                double[] w = Weights.getWeightsSumDiff(diff);
                double[] wInv = Weights.getWeightsSumDiffInv(diff);
                for (int i1 = 0; i1 < N - diff; i1++)
                {
                    int i2 = i1 + diff;
                    int firstIndex = (i1 + 1) % l;
                    int secondIndex = i2 % l;

                    int value = mod(C[i2] - C[i1], N);
                    int valueInv = mod(InvC[i2] - InvC[i1], N);

                    //normalize
                    if (secondIndex < firstIndex)
                    {
                        firstIndex = (i2 + 1) % l;
                        secondIndex = i1 % l;
                        value = mod(sCandidates[0] - value);
                    }

                    if (w[i1] >= Settings.prob_th)
                    {
                        if (S[i2] >= i2 && S[i1] >= i1)
                        {
                            initialCounter[firstIndex, secondIndex, value] += w[i1];
                        }

                    }

                    if (wInv[i1] >= Settings.prob_th)
                    {
                        if (InvS[i2] <= i2 && InvS[i1] <= i1)
                        {
                            initialCounter[firstIndex, secondIndex, valueInv] += w[i1];
                        }

                    }

                }
            }
        }

        public double[,,] updateCounterWithAllSum()
        {
            double[,,] counter = Utils.copyArray3D(initialCounter, l, l, N);            

            int lambda = 0; //number of s to be substraced (minus lambda*s)
            for (int i = 0; i < N; i++)
            {
                if (Weights.sumS[i] >= Settings.prob_th)
                    if (S[i] >= i)
                    {
                        int value = mod(C[i] - lambda * s, N);
                        counter[0, i % l, value] += Weights.sumS[i];
                    }

                if (Weights.sumSS[i] >= Settings.prob_th)
                {
                    int value = mod(S[S[i]] - i * (i + 1) / 2 - lambda * s, N);

                    counter[0, i % l, value] += Weights.sumSS[i];
                }/**/

                if (Weights.sumSS[i] >= Settings.prob_th)
                {
                    int value = mod(S[S[S[i]]] - i * (i + 1) / 2 - lambda * s, N);

                    counter[0, i % l, value] += Weights.sumSSS[i];
                }/**/

                if (i % l == l - 1) lambda++;
            }
            return counter;
        }

        //tady je Log
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
            //Log.Key(cipher.K);
            //Log.Key(new Key(guessedKey));


            return guessedKey;
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

        public int[] guessSumValue()
        {
            int l = keyLength;
            var sumGuess = new double[N];
            double prob_th = Settings.prob_th;

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
                    sumGuess[diffCi] += wDiffL[i1];
                i1++;
            }/**/

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
            }/**/

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

            int[] result = new int[Constants.sumAllKeyBytesDepth];
            double max = -1;
            int maxIndex = 0;

            for (int i = 0; i < Constants.sumAllKeyBytesDepth; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (sumGuess[j] > max)
                    {
                        max = sumGuess[j];
                        maxIndex = j;
                    }
                }
                sumGuess[maxIndex] = -1;
                result[i] = maxIndex;
            }
            return result;
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

        public double weightJ(int round,int nesting,bool inverse)
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
                    weight = (inverse) ? Weights.JInvS[round-1] : Weights.JS[round-1];
                    break;
            }
            return weight;
            /*double tresholdHigh = 0.05;
            double tresholdLow = 0.008;

            return (weight > tresholdHigh) ? 2 : (weight < tresholdLow ? 0 : 1);*/
        }

        public double[,] guessKeyBasedOnJs()
        {
            double[,] guess = new double[keyLength,N];
            //TODO pro vsechna opakovani


            for (int y = 0; y < N; y++) //position
            {

                int first = (y == 0) ? 0 : S[y - 1];
                int second = S[y];

                int firstInv = (y == 0) ? 0 : InvS[y - 1];
                int secondInv = InvS[y];

                //nesting first
                for (int i = 1; i <= 4; i++)
                {
                    //nesting second
                    for (int j = 1; j <= 4; j++)
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

        /*public int[] solveLinear(int[,] A, int[] b)
        {
            for ()
        }*/

        public int[] selectCandidatesForByte(double[] counter, int nc)
        {
            int[] candidates = new int[nc];

            List<KeyByteWeight> bytesWithWeights = new List<KeyByteWeight>();
            for (int i = 0; i < N; i++)
            {
                if (counter[i] > prob_th)
                {
                    bytesWithWeights.Add(new KeyByteWeight(i, counter[i]));
                }
            }
            bytesWithWeights.Sort(new KeyByteWeightComparer());

            for (int i = 0; i < Math.Min(bytesWithWeights.Count, nc); i++)
            {
                candidates[i] = bytesWithWeights[i].keyByte;
            }

            return candidates;
        }

        public void guessKeyOnPositions(bool[] positions, int m)
        {
            //Log.Combination(positions);

            /*int nextPosition = 0;
            int[] guessedKey = guessKeyBasedOnJs();
            for (int i = 0; i< m; i++)
            {   
                while (!positions[i]) nextPosition++;
            }*/
        }


        public void findSumsUsedToUpdate(int from, int to, int position, bool[] fixedBytes, out int startIndex, out int endIndex)
        {
            startIndex = position;
            endIndex = position;

            while (startIndex > from && fixedBytes[startIndex])
                startIndex--;
            if (!fixedBytes[startIndex]) startIndex++;

            if (to == 10)
                startIndex += 0;

            while (endIndex < to && fixedBytes[endIndex])
                endIndex++;
            if (!fixedBytes[endIndex]) endIndex--;         
        }

        public void generateCombinations(int n, int k)
        {
            combinations(0, 0, k, new bool[n]);
        }

        public void combinations(int index, int filled,int limit, bool[] selection)
        {
            if (filled == limit)
            {
                //guessKeyOnPositions(selection, limit);
                //Log.Combination(selection);
                bool[] combination = new bool[keyLength];
                Array.Copy(selection, combination, keyLength);
                allCombinations.Add(combination);
                return;
            }
            if (index == selection.Length)
                return;
            selection[index] = true;
            combinations(index + 1, filled + 1,limit, selection);
            selection[index] = false;
            combinations(index + 1, filled, limit, selection);
        }

        public int Binomial(int n, int k)
        {
            if (k > n || n < 0)
            {
                return 0;
            }
            int result = 1;
            if (k == n || k == 0)
            {
                return 1;
            }
            int kk = k;

            if (k > (n - k)) //n over k = n over n-k
            {
                kk = n - k;
            }

            //n! / (k! *(n-k)!) = n*(n-1)*...*(n-k+1)	/ (1*2*...*k)
            for (int i = 0; i < kk; i++)
            {
                result = result * (n - i);
                result = result / (i + 1);
            }

            return result;
        }

    }
}
    
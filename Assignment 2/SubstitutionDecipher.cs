using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_2
{
    /// <summary>
    /// Wesley Slates
    /// ID: 013472730
    /// CECS 478 HONORS
    /// Assignment #2
    /// 
    /// 
    /// Class that provides method to break substitution cipher and all necessary helper methods.
    /// </summary>
    class SubstitutionDecipher
    {

        static Random random = new Random();


        /// <summary>
        /// Returns the best key using the caesar cipher
        /// </summary>
        /// <param name="str">Ciphertext.</param>
        /// <returns></returns>
        public static Dictionary<char, char> CheckCaesar(String str)
        {
            Quadgram_Analysis analysis = new Quadgram_Analysis();

            Dictionary<char, char> key = null;

            double bestFitness = -99999;

            String alphabet = "abcdefghijklmnopqrstuvwxyz";
            for (int amt = 1; amt < 26; amt++)
            {
                //shift each character of the string "amt" number of letters
                String shiftedString = Shift(str, amt);

                //test the fitness of this shifted string
                double curFitness = analysis.TestKeyFitness(GetQuadgrams(shiftedString));

                if (curFitness > bestFitness)
                {
                    //builds a key from this shift if it's the best we've seen
                    key = new Dictionary<char, char>();

                    //we know the key for the shift, so we're going to go trhough the alphabet and
                    //shift each character the correct amount to build a key
                    for (int i = 0; i < 26; i++)
                    {
                        //a bit of a hack, but to utilize the shift() method on characters,
                        //converts the char to a string, and converts it back to a char array
                        //since it's only a character, we know the answer is in index 0
                        char[] shiftedChar = Shift(alphabet[i].ToString(), amt).ToCharArray();

                        key.Add(alphabet[i], shiftedChar[0]);

                    }
                    bestFitness = curFitness;
                }
            }
            return key;

        }

        /// <summary>
        /// Shifts each character of a string by a certain number of characters.
        /// Shifting a string by 1 would shift each character 1 letter further in the alphabet, wrapping
        /// z around to a.
        /// </summary>
        /// <param name="str">String to be shifted.</param>
        /// <param name="amt">Amount to shift each character.</param>
        /// <returns></returns>
        private static String Shift(String str, int amt)
        {
            String returnStr = "";
            str = str.ToLower();
            foreach (char c in str)
            {
                if (c != ' ')
                {
                    //casting character to int gets its ascii value
                    int ascii = (int)c;

                    //97 is where lowercase alphabet starts in ascii
                    //so if the amount shifts it past that, we are going to loop around
                    if (ascii - amt < 97)
                    {
                        ascii = 123 - (97 - (ascii - amt));
                    }
                    else
                    {
                        //otherwise, just subtract it
                        ascii -= amt;
                    }

                    //casting an int to a char gets the char of that ascii value
                    returnStr += (char)ascii;
                }
            }
            return returnStr;
        }

        /// <summary>
        /// Deciphers ciphertext using a hill climbing algorithm.
        /// 
        /// Algorithm orginally explained at
        /// http://practicalcryptography.com/cryptanalysis/stochastic-searching/cryptanalysis-simple-substitution-cipher/
        /// 
        /// Has a difference that rather than randomly switching letters, it switches letters iteratively.
        /// Read somewhere that was slightly better, also gives a clearer point where to stop.
        /// </summary>
        /// <param name="encStr"></param>
        /// <returns>Returns a char-char dictionary giving the algorithm's
        /// best guess for the key. Each key relates to it's corresponding
        /// ciphertext character. </returns>
        public static Dictionary<char, char> Decipher(String encStr)
        {
            HashSet<String> testedKeys = new HashSet<string>();

            Quadgram_Analysis analysis = new Quadgram_Analysis();
            Dictionary<char, char> retKey = null;

            //we're going to get the best ceasar key and fitness for future use
            Dictionary<char, char> caesarKey = CheckCaesar(encStr);
            double caesarFitness = analysis.TestKeyFitness(GetQuadgrams(DecipherMessageUsingKey(caesarKey, encStr)));

            //best fitness we've encountered
            double bestFitness = -99999;

            //number of times we've encountered the best fitness
            int bestFitnesses = 0;

            for (int k = 0; k < 1000; k++)
            {
                //generate a random key and check its fitness
                Dictionary<char, char> key = GenerateRandomKey();

                double fitness = analysis.TestKeyFitness(GetQuadgrams(DecipherMessageUsingKey(key, encStr)));

                //determines if we've found a better key for this key
                bool betterKeyFound;

                do
                {

                    betterKeyFound = false;

                    /*
                     * We're going to loop through the key and test all possible "shuffles" of this key,
                     * in which we switch each letter with every other letter and see what the fitness is.
                     * If the fitness is better, then we will go with that key and continue switching letters.
                     */
                    for (int i = 0; i < 25; i++)
                    {
                        for (int j = 1; j < 26; j++)
                        {
                            //switch letters
                            KeyValuePair<char, char> temp = key.ElementAt(i);
                            KeyValuePair<char, char> temp1 = key.ElementAt(j);
                            key[temp.Key] = temp1.Value;
                            key[temp1.Key] = temp.Value;

                            //get the fitness of this new message
                            double newFitness = analysis.TestKeyFitness(GetQuadgrams(DecipherMessageUsingKey(key, encStr)));


                            if (newFitness > fitness)
                            {
                                betterKeyFound = true;
                                fitness = newFitness;

                                if (fitness > bestFitness)
                                {
                                    bestFitness = fitness;
                                    bestFitnesses = 1;
                                    retKey = key;
                                }
                                else if (fitness == bestFitness)
                                {
                                    /*if we've hit the same best fitness 3 times and it's 
                                     * better than any caesar fitness we've seen, we'll assume that's
                                     * probably the best guess, and we'll end the algorithm.
                                     */
                                    if (++bestFitnesses == 3 && bestFitness > caesarFitness)
                                    {
                                        return key;
                                    }
                                }
                            }
                            else
                            {
                                /*
                                 * Otherwise, we did not get a better key with this switch, so let's
                                 * revert the changes back.
                                 */
                                key[temp.Key] = temp.Value;
                                key[temp1.Key] = temp1.Value;
                            }

                        }


                    }
                    /*
                     * If it ISN'T a caesar cipher, the caesar fitness should be terrible
                     * and we should've found a better fitness through this algorithm
                     * by now. Thus, if we've tried 650 keys and NONE of them are better than 
                     * the caesar cipher....it's probably a caesar cipher.
                     */
                    if ((bestFitness < caesarFitness))
                    {
                        return caesarKey;
                    }

                } while (betterKeyFound);
            }

            return retKey;

        }

        /// <summary>
        /// Generates a random key for a substitution cipher.
        /// </summary>
        /// <returns>Returns a char-char dictionary, where the key is each letter of the alphabet,
        /// and the value to each key is its relative cipher character</returns>
        private static Dictionary<char, char> GenerateRandomKey()
        {

            Dictionary<char, char> key = new Dictionary<char, char>();

            String alphabet = "abcdefghijklmnopqrstuvwxyz";

            int pos = 0;

            while (key.Count < 26)
            {
                /*
                 * To generate a "random" character, we're going to pick random
                 * number between 97(inclusive) and 123(exclusive) since those
                 * are the ascii values for the lower case alphabet.
                 */
                char letter = (char)random.Next(97, 123);

                //this checks if a value is in the dictionary, and if so, what it key is
                //returns null character if not in dictionary
                var letterKey = key.FirstOrDefault(value => value.Value == letter).Key;

                //makes sure it has a key
                if (letterKey == '\0')
                {
                    key.Add(alphabet[pos], letter);
                    pos++;
                }
            }

            return key;
        }

        /// <summary>
        /// Returns a string version of the key, in which the first letter translates to 
        /// 'a', the second is 'b', the third is 'c', etc.
        /// </summary>
        /// <param name="key">Key to the ciphertext.</param>
        /// <returns>Returns a string of the key.</returns>
        private static String ReadKey(Dictionary<char, char> key)
        {
            String retString = "";
            foreach (KeyValuePair<char, char> kvp in key)
            {
                retString += kvp.Value;
            }

            return retString;
        }

        /// <summary>
        /// Returns the deciphered message of an ciphered string using the given key.
        /// </summary>
        /// <param name="key">Key to the cipher.</param>
        /// <param name="message">Ciphertext to be deciphered.</param>
        /// <returns></returns>
        public static String DecipherMessageUsingKey(Dictionary<char, char> key, String message)
        {
            String retString = "";

            foreach (char c in message)
            {
                retString += key[c];
            }

            return retString;
        }

        /// <summary>
        /// Returns a list of each quadgram in the string it receives.
        /// A quadgram is simply a four letter string:
        ///     Ex: Wesley
        ///     Quadgrams: WESL ESLE SLEY
        /// </summary>
        /// <param name="message">Message to count quadgrams on</param>
        /// <returns>List of quadgrams in sequential order.</returns>
        private static List<String> GetQuadgrams(String message)
        {
            List<String> quadgrams = new List<String>();

            for (int i = 0; i < message.Length - 5; i++)
            {

                quadgrams.Add(message.Substring(i, 4));

            }

            return quadgrams;
        }

        

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Assignment_2
{
    /// <summary>
    /// Wesley Slates
    /// ID: 013472730
    /// CECS 478 HONORS
    /// Assignment #2
    /// 
    /// 
    /// Class that analyzes plain text using quadgram analysis to determine whether or not a string is 
    /// close to english or not.
    /// </summary>
    class Quadgram_Analysis
    {
        private string[] lines;

        //dictionary for quadgrams and their probabilities.
        private Dictionary<string, double> logProb = new Dictionary<string, double>();
        private double total = 0;


        //got the quadgrams from this source: https://people.sc.fsu.edu/~jburkardt/datasets/ngrams/ngrams.html
        public Quadgram_Analysis()
        {
            lines = File.ReadAllLines(@"english_quadgrams.txt");
            Read();
        }

        /// <summary>
        /// Reads quadgrams and builds dictionary using data. Dictionaries in C# are built
        /// using hash tables, so getting a value is O(1).
        /// </summary>
        private void Read()
        { 
            //text file doesn't give total quadgrams, need to get that
            foreach (String line in lines)
            {
                String[] subStrings = line.Split(' ');
                total += Convert.ToInt32(subStrings[1]);
            }

            //get probabily of each word and get the log10 of it.
            foreach (String line in lines)
            {
                String[] subStrings = line.Split(' ');
                logProb.Add(subStrings[0], System.Math.Log10(Convert.ToInt32(subStrings[1])/total));
            }
        }

        /// <summary>
        /// Tests the fitness of plaintext using its quadgrams.
        /// </summary>
        /// <param name="quadgrams">List of each of the plaintext's quadgrams. </param>
        /// <returns>Returns fitness value. Higher values are better. </returns>
        public double TestKeyFitness(List<String> quadgrams)
        {
            double sum = 0;
            foreach(String s in quadgrams)
            {
                //if the quadgram is not found, just give it the lowest possible value.
                if (!logProb.TryGetValue(s.ToUpper(), out double val))
                {
                    sum+=System.Math.Log10(1 / total);
                }
                else
                    sum+= logProb[s.ToUpper()];
            }

            return sum;
            
        }
        
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assignment_2
{
    class Program
    {
        static void Main(string[] args)
        {

            String[] lines = File.ReadAllLines(@"Input.txt");
            foreach (String s in lines)
            {
                
                String response = null;

                do
                {
                    Console.WriteLine("Deciphering:      " + s);
                    Console.WriteLine("Deciphering....Please wait.");
                    Console.WriteLine("Deciphered text: " + Decipher(s.Replace(" ", String.Empty)));
                    Console.Write("Is that correct? (Y/N): ");
                    response = Console.ReadLine().ToLower();
                    Console.WriteLine(response);
                } while (response.Equals("n"));
                
                
            }
           


            
            
            Console.ReadLine();
        }

        public static String  Decipher (String encStr)
        {
            Quadgram_Analysis analysis = new Quadgram_Analysis();
            
            Dictionary<char, char> parentKey = GenerateRandomKey();

            HashSet<String> testedKeys = new HashSet<String>();
            testedKeys.Add(ReadKey(parentKey));

            double fitness = analysis.TestKeyFitness(GetQuadgrams(DecipherMessageUsingKey(parentKey, encStr))) ;

            String message = encStr.ToString();

            double highest = -999999;
            double absHighest = -99999;
            int tries = 0;
            int abs = 0;

            String thisMessage = null;
            String retMessage = null;

            while (abs<15)
            {
                
                thisMessage = null;
                
                for (int i = 0; i<1000; i++)
                {
                    Dictionary<char, char> childKey = new Dictionary<char, char>(parentKey);
                    
                    do
                    {
                       
                        SwapTwoLetters(childKey);
                       
                    } while (testedKeys.Contains(ReadKey(childKey)));
                    
                    

                    String currentMessage = DecipherMessageUsingKey(childKey, message);


                    double newFitness = 0;


                    newFitness = analysis.TestKeyFitness(GetQuadgrams(currentMessage));

                    
                    if (newFitness > fitness)
                    {
                        fitness = newFitness;
                        parentKey = new Dictionary<char, char>(childKey);
                        thisMessage = currentMessage;
                    }

                    testedKeys.Add(ReadKey(childKey));
                }
                //Console.WriteLine(fitness);
                if (fitness>highest)
                {
                    tries = 0;
                    highest = fitness;
                    retMessage = thisMessage;
                    Console.WriteLine(thisMessage +  " " + fitness);
                    if (fitness>absHighest)
                    {
                        absHighest = fitness;
                        abs++;
                        Console.WriteLine(abs);
                    }
                } else if (fitness==highest || fitness < highest)
                {
                    tries++;
                    //Console.WriteLine(++tries);
                    do
                    {
                        parentKey = GenerateRandomKey();
                        
                    } while (testedKeys.Contains(ReadKey(parentKey)));

                    
                    fitness = analysis.TestKeyFitness(GetQuadgrams(DecipherMessageUsingKey(parentKey, encStr)));

                    //stuck at local max
                    if (tries > 100)
                    {
                        Console.WriteLine("hit local max");
                        highest = -99999;
                    }


                }
               
                //tries++;
                
            }

            return retMessage;
            
        }


        public static Dictionary<char, char> GenerateRandomKey()
        {
            Random rand = new Random();
            Dictionary<char, char> key = new Dictionary<char, char>();
            String alphabet = "abcdefghijklmnopqrstuvwxyz";
            int pos = 0;
            while(key.Count<26)
            {
                char letter = (char)rand.Next(97, 123);

                var letterKey = key.FirstOrDefault(value => value.Value == letter).Key;
                if (letterKey == '\0')
                {
                    key.Add(alphabet[pos], letter);
                    pos++;
                }
            }

            return key;
        }

        public static String ReadKey(Dictionary<char, char> key)
        {
            String retString = "";
            foreach (KeyValuePair<char, char> kvp in key)
            {
                retString += kvp.Value;
            }

            return retString;
        }
        public static void SwapTwoLetters(Dictionary<char, char> key)
        {
            
            Random random = new Random(Guid.NewGuid().GetHashCode());
            char rand1 = (char)random.Next(97, 123);
            char rand2;

            do
            {
                rand2 = (char)random.Next(97, 123);
            } while (rand1 == rand2);
            

            char temp = key[rand1];
            char temp2 = key[rand2];
            key[rand1] = temp2;
            key[rand2] = temp;

        }

        public static String DecipherMessageUsingKey(Dictionary<char, char> key, String message)
        {
            String retString = "";
            foreach (char c in message)
            {
                retString += key[c];
            }

            return retString;
        }
    
        public static List<String> GetQuadgrams(String message)
        {
            List<String> quadgrams = new List<String>();
            
            for (int i = 0; i< message.Length-5; i++)
            {
                
                quadgrams.Add( message.Substring(i, 4)); 
                
            }

            return quadgrams;
        }
        
        
    }
}

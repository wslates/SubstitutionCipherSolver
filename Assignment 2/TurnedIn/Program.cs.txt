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
    /// Main program. 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //Increases Console.ReadLine() input from 254 characters
            Console.SetIn(new StreamReader(Console.OpenStandardInput(8192)));

            String answer;
            do
            {
                Console.Write("Choose an option:\n \t{0}\n \t{1}\n \t{2}\n \t{3}\n{4}",
                                "1. Decrypt assignment cipher text.",
                                "2. [HONORS] Decrypt ciphertext using key.",
                                "3. [HONORS] Encrypt plaintext using key.",
                                "4. Quit.",
                                "Enter selection (1, 2, 3, or 4): ");
                answer = Console.ReadLine();

                if (answer.Equals("1"))
                {
                    ReadandDecryptAssignment();
                }
                else if (answer.Equals("2"))
                {
                    Console.WriteLine(DecryptUserCipher());
                } else if (answer.Equals("3"))
                {
                    Console.WriteLine(EncryptUserCipher());
                }else if (!answer.Equals("3"))
                {
                    Console.Write("\n{0}", "Please enter a correct input. ");
                }
            } while (!answer.Equals("4"));
            
        }

        /// <summary>
        /// Decrypts assignment ciphertext.
        /// </summary>
        public static void ReadandDecryptAssignment()
        {
            String[] lines = {  "fqjcb rwjwj vnjax bnkhj whxcq nawjv nfxdu mbvnu ujbbf nnc",
                                "oczmz vmzor jocdi bnojv dhvod igdaz admno ojbzo rcvot jprvi oviyv aozmo cvooj ziejt dojig toczrdnzno jahvi fdiyv xcdzq zoczn zxjiy",
                                "ejitp spawa qleji taiul rtwll rflrl laoat wsqqj atgac kthls iraoa twlpl qjatw jufrh lhuts qataq itats aittkstqfj cae",
                                "iyhqz ewqin azqej shayz niqbe aheum hnmnj jaqii yuexq ayqkn jbeuq iihed yzhni ifnun sayiz yudhesqshu qesqa iluym qkque aqaqm oejjs " +
                                "hqzyu jdzqa diesh niznj jayzy uiqhq vayzq shsnj jejjz nshnahnmyt isnae sqfun dqzew qiead zevqi zhnjq shqze udqai jrmtq uishq ifnun " +
                                "siiqa suoij qqfni syyle iszhnbhmei squih nimnx hsead shqmr udquq uaqeu iisqe jshnj oihyy snaxs hqihe lsilu ymhni tyz" };

            Console.WriteLine("\nPlease be aware, this algorithm may take a couple tries to get the complete answer.");
            Console.WriteLine("#4 doesn't fully solve due to old english, but it gets close enough.");
            foreach (String s in lines)
            {
                String response = null;
                do
                {
                    Console.Write("\n\nCipher Text: {0}\nDeciphering....Please Wait.\n", s);
                    Dictionary<char, char> key = SubstitutionDecipher.Decipher(s.Replace(" ", String.Empty));
                    String strKey = "";
                    String strCipher = "";
                    foreach (KeyValuePair<char, char> kvp in key)
                    {
                        strCipher += kvp.Key;
                        strKey += kvp.Value;
                    }
                    Console.WriteLine("Cipher letters: " + strCipher);
                    Console.WriteLine("Key           : " + strKey);
                    
                    Console.WriteLine("Deciphered text: " + SubstitutionDecipher.DecipherMessageUsingKey(key, s.Replace(" ", String.Empty)));
                    Console.Write("Is that correct? (Y/N): ");
                    response = Console.ReadLine().ToLower();
                } while (response.Equals("n"));

            }
        }

        /// <summary>
        /// Prompts user for key and plaintext and encrypts the plaintext using the cipher
        /// </summary>
        /// <returns>Returns encrypted ciphertext.</returns>
        public static String EncryptUserCipher()
        {
            Dictionary<char, char> key;

            String cipherText = "";
            Console.Write("\nPlease enter the plaintext: ");

            String plainText = Console.ReadLine().ToLower();

            //remove any punctuation, symbols, or spaces
            plainText = new String(plainText.Where(c => char.IsLetter(c)).ToArray()).ToLower();

            Console.Write("\nPlease enter the key as a string like \"abcdefghijklmnopqrstuvwxyz\", where the first letter corresponds to the letter 'a' in the ciphertext, the second to 'b'.... the last to 'z'\n");

            String alphabet = "abcdefghijklmnopqrstuvwxyz";

            bool issue = false;
            do
            {
                issue = false;
                key = new Dictionary<char, char>();
                Console.Write("Key: ");
                String userKey = Console.ReadLine().ToLower();

                //check if key is long enough, we want a full 26 letter key
                if (userKey.Length<26 || userKey.Length>26)
                {
                    issue = true;
                    Console.WriteLine("Key too short or long; current length: " + userKey.Length);
                } else
                {
                    //build key
                    int pos = 0;
                    foreach (char c in userKey)
                    {
                        try
                        {
                            key.Add(c, alphabet[pos++]);
                        } catch (ArgumentException e)
                        {
                            issue = true;
                            Console.WriteLine("Error: repeating character in key.");
                            break;
                        }
                    }
                    
                }
            } while (issue);

            foreach (char c in plainText)
            {
                char letterKey = key.FirstOrDefault(value => value.Value == c).Key;
                cipherText += letterKey;
                if (cipherText.Replace(" ", String.Empty).Length%5 == 0) { cipherText += " "; }
            }

            return cipherText;
        }
        /// <summary>
        /// Prompts user for ciphertext and corresponding key, and decrypts the ciphertext.
        /// </summary>
        /// <returns>Returns a string corresponding to the deciphered plain text.</returns>
        public static String DecryptUserCipher()
        {
            Dictionary<char, char> key = new Dictionary<char, char>();
            bool issue = false;
            
            //get ciphertext
            Console.Write("Please enter the ciphertext:");
            String cipherText = Console.ReadLine();
            
            //remove any punctuation, symbols, or spaces
            cipherText = new String(cipherText.Where(c => char.IsLetter(c)).ToArray()).ToLower();

            do
            {
                Console.Write("\nPlease enter the key as a string like \"abcdefghijklmnopqrstuvwxyz\", where the first letter corresponds to the letter 'a' in the ciphertext, the second to 'b'.... the last to 'z'\n");

                Console.Write("Key: ");
                String userKey = Console.ReadLine();
                userKey = new String(userKey.Where(c => char.IsLetter(c)).ToArray()).ToLower();

                //check if key is long enough, we want a full 26 letter key
                if (userKey.Length<26 || userKey.Length>26)
                {
                    Console.WriteLine("Key too short or long; current length: " + userKey.Length);
                    issue = true;
                } else
                {
                    //build key
                    issue = false;
                    int pos = 0;
                    String alphabet = "abcdefghijklmnopqrstuvwxyz";

                    foreach (char c in userKey)
                    {
                        key.Add(c, alphabet[pos++]);
                    }
                }
                
            } while (issue);
  
            return SubstitutionDecipher.DecipherMessageUsingKey(key, cipherText);
            
        }
        
        
    }
}

// Author: Omid Kashefi, Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mehrdad Senobari at 2010-March-08
//

namespace SCICT.Utility.IO
{
    ///<summary>
    /// Load words, usage frequency and POS tag from dictionary file
    ///</summary>
    public class DictionaryWordFreqPOSLoader : DictionaryLoader
    {
        ///<summary>
        /// Pars line's content
        ///</summary>
        ///<param name="word">Extracted word</param>
        ///<param name="freq">Extracted word's usage frequency</param>
        ///<param name="pos">Extracted word's POS tag</param>
        ///<returns>True if word successfully extracted, otherwise False</returns>
        public bool NextTerm(out string word, out int freq, out string pos)
        {
            word = "";
            freq = 0;
            pos = "";

            string line;
            if (!NextLine(out line))
            {
                return false;
            }
            
            string[] terms = line.Split('\t');

            if (terms.Length >= 3)
            {
                if (terms[0].Length > 0 && terms[1].Length > 0 && terms[2].Length > 0)
                {
                    word = terms[0];
                    pos = terms[2];

                    if (int.TryParse(terms[1], out freq))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="word">word</param>
        ///<param name="freq">word's usage frequency</param>
        ///<param name="pos">word's POS tag</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddTerm(string word, int freq, string pos)
        {
            if (!AddLine(word + "\t" + freq.ToString() + "\t" + pos))
            {
                return false;
            }

            return true;
        }
        
        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="word">word</param>
        ///<param name="freq">word's usage frequency</param>
        ///<param name="pos">word's POS tag</param>
        ///<param name="fileName">File name</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddTerm(string word, int freq, string pos, string fileName)
        {
            if (!AddLine(word + "\t" + freq.ToString() + "\t" + pos, fileName))
            {
                return false;
            }

            return true;
        }

    }

}

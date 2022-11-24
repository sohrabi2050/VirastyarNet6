// Author: Omid Kashefi, Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mehrdad Senobari at 2010-March-08
//

namespace SCICT.Utility.IO
{
    ///<summary>
    /// Load words and usage frequency from dictionary file
    ///</summary>
    public class DictionaryWordFreqLoader : DictionaryLoader
    {
        ///<summary>
        /// Pars line's content
        ///</summary>
        ///<param name="word">Extracted word</param>
        ///<param name="freq">Extracted word's usage frequency</param>
        ///<returns>True if word successfully extracted, otherwise False</returns>
        public bool NextTerm(out string word, out int freq)
        {
            word = "";
            freq = 0;

            string line;
            if (!NextLine(out line))
            {
                return false;
            }

            string[] terms = line.Split('\t');

            if (terms.Length >= 2)
            {
                if (terms[0].Length > 0 && terms[1].Length > 0)
                {
                    word = terms[0];

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
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddTerm(string word, int freq)
        {
            if (!AddLine(word + "\t" + freq.ToString()))
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
        ///<param name="fileName">File name</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddTerm(string word, int freq, string fileName)
        {
            if (!AddLine(word + "\t" + freq.ToString(), fileName))
            {
                return false;
            }

            return true;
        }
    }

}

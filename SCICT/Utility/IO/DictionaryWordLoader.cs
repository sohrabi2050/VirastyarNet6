// Author: Omid Kashefi, Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mehrdad Senobari at 2010-March-08
//

namespace SCICT.Utility.IO
{
    ///<summary>
    /// Load words from dictionary file
    ///</summary>
    public class DictionaryWordLoader : DictionaryLoader
    {
        ///<summary>
        /// Next dictionary term
        ///</summary>
        ///<param name="word">Extracted word</param>
        ///<returns>True if word successfully extracted, False if EOF</returns>
        public virtual bool NextTerm(out string word)
        {
            word = "";

            while (true)
            {
                string line;
                if (!NextLine(out line))
                {
                    return false;
                }

                string[] terms = line.Split('\t');

                if (terms.Length >= 1)
                {
                    if (terms[0].Length > 0)
                    {
                        word = terms[0];

                        return true;
                    }
                }
            }
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="word">word</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public virtual bool AddTerm(string word)
        {
            if (!AddLine(word))
            {
                return false;
            }

            return true;
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="word">word</param>
        ///<param name="fileName">File name</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public virtual bool AddTerm(string word, string fileName)
        {
            if (!AddLine(word, fileName))
            {
                return false;
            }

            return true;
        }
    }

}

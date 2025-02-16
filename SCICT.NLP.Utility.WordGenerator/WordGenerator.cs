// Author: Omid Kashefi 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi at 2010-March-08
//


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SCICT.NLP.Persian.Constants;
using System.Text;
using System.Timers;
using System.IO;
using SCICT.NLP.Persian;

namespace SCICT.NLP.Utility.WordGenerator
{
    ///<summary>
    /// Indicates the methodes of generating respelling suggestions by insering or omitting a letter, substitution of a letter with other letters and transposing two adjacent letters
    ///</summary>
    [Flags]
    public enum RespellingGenerationType
    {
        ///<summary>
        /// Transposition of two adjacent letters
        ///</summary>
        Transpose = 1, // Math.Pow(2,0)
        ///<summary>
        /// insertion of one letter
        ///</summary>
        Insert = Transpose * 2,
        ///<summary>
        /// Omission of one letter
        ///</summary>
        Delete = Insert * 2,
        ///<summary>
        /// Substitution of two letters
        ///</summary>
        Substitute = Delete * 2
    }

    public class LetterMatrix
    {
        private List<string> m_CharacterMatrix = new List<string>();
        private List<string> m_UpperCharacterMatrix = new List<string>();

        private string m_FirstLine = "ضصثقفغعهخحجچ";
        private string m_SecondLine = "شسیبلاتنمکگ  ";
        private string m_ThirdLine = "ظطزرذدئو    ";

        private string m_UpperFirsLine = "پ          پ";
        private string m_UpperSecondLine = "     آ     ژپ";
        private string m_UpperThirdLine = "  ژؤإأء     ";

        private Dictionary<char, char[]> m_suggestionDic = new Dictionary<char, char[]>();

        public LetterMatrix()
        {
            Init();
        }

        private void Init()
        {
            m_UpperCharacterMatrix.Add(m_UpperFirsLine);
            m_UpperCharacterMatrix.Add(m_UpperSecondLine);
            m_UpperCharacterMatrix.Add(m_UpperThirdLine);

            m_CharacterMatrix.Add(m_FirstLine);
            m_CharacterMatrix.Add(m_SecondLine);
            m_CharacterMatrix.Add(m_ThirdLine);

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 12; ++j)
                {
                    char[] cArry = {m_CharacterMatrix[i].ToCharArray()[j], m_UpperCharacterMatrix[i].ToCharArray()[j]};

                    foreach (char c in cArry)
                    {
                        if (c != ' ')
                        {
                            if (!m_suggestionDic.ContainsKey(c))
                            {
                                List<char> charList = new List<char>();
                                charList.AddRange(AdjcentCharacterPeaker(i, j));
                                charList.AddRange(AdjcentUpperCharacterPeaker(i, j));
                                charList.Remove(c);
                                //charList.Add(PseudoSpace.ZWNJ);
                                charList = charList.Distinct().ToList();

                                m_suggestionDic.Add(c, charList.ToArray());
                            }
                            else
                            {
                                List<char> charList = new List<char>();
                                charList.AddRange(m_suggestionDic[c]);
                                charList.AddRange(AdjcentCharacterPeaker(i, j));
                                charList.AddRange(AdjcentUpperCharacterPeaker(i, j));
                                charList.Remove(c);
                                //charList.Add(PseudoSpace.ZWNJ);
                                charList = charList.Distinct().ToList();

                                m_suggestionDic[c] = charList.ToArray();
                            }
                        }
                    }
                }
            }

            StreamWriter writer = new StreamWriter("./matrix.dat");
            StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<char, char[]> pair in m_suggestionDic)
            {
                sb.Append("Dic.Add('" + pair.Key + "', new char[] {" );
                for (int i = 0; i < pair.Value.Length; ++i)
                {
                    sb.Append("'" + pair.Value[i] + "'");
                    if (i < pair.Value.Length - 1)
                    {
                        sb.Append(", ");
                    }
                    else
                    {
                        sb.Append("});");
                    }
                }
                writer.WriteLine(sb.ToString());
                sb.Remove(0, sb.Length);
            }
            writer.Close();
        }

        private char[] AdjcentCharacterPeaker(int i, int j)
        {
            int rowMin = 0;
            int rowMax = 3;
            int colMin = 0;
            int colMax = 12;

            int iStep = 1;
            int jStep = 2;

            List<char> charList = new List<char>();
            char c;

            
            if (j - jStep >= colMin)
            {
                c = m_CharacterMatrix[i].ToCharArray()[j - jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (j + jStep < colMax)
            {
                c = m_CharacterMatrix[i].ToCharArray()[j + jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (i - iStep >= rowMin)
            {
                c = m_CharacterMatrix[i - iStep].ToCharArray()[j];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (i + iStep < rowMax)
            {
                c = m_CharacterMatrix[i + iStep].ToCharArray()[j];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (j - jStep >= colMin && i - iStep >= rowMin)
            {
                c = m_CharacterMatrix[i - iStep].ToCharArray()[j - jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (j + jStep < colMax && i + iStep < rowMax)
            {
                c = m_CharacterMatrix[i + iStep].ToCharArray()[j + jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (i - iStep >= rowMin && j + jStep < colMax)
            {
                c = m_CharacterMatrix[i - iStep].ToCharArray()[j + jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (i + iStep < rowMax && j - jStep >= colMin)
            {
                c = m_CharacterMatrix[i + iStep].ToCharArray()[j - jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }

            if (i < rowMax && j >= colMin && i >= rowMin && j < colMax)
            {
                c = m_CharacterMatrix[i].ToCharArray()[j];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }

            return charList.ToArray();

        }
        private char[] AdjcentUpperCharacterPeaker(int i, int j)
        {
            int rowMin = 0;
            int rowMax = 3;
            int colMin = 0;
            int colMax = 12;

            int iStep = 1;
            int jStep = 1;

            List<char> charList = new List<char>();
            char c;

            if (j - jStep >= colMin)
            {
                c = m_UpperCharacterMatrix[i].ToCharArray()[j - jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (j + jStep < colMax)
            {
                c = m_UpperCharacterMatrix[i].ToCharArray()[j + jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (i - iStep >= rowMin)
            {
                c = m_UpperCharacterMatrix[i - iStep].ToCharArray()[j];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (i + iStep < rowMax)
            {
                c = m_UpperCharacterMatrix[i + iStep].ToCharArray()[j];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (j - jStep >= colMin && i - iStep >= rowMin)
            {
                c = m_UpperCharacterMatrix[i - iStep].ToCharArray()[j - jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (j + jStep < colMax && i + iStep < rowMax)
            {
                c = m_UpperCharacterMatrix[i + iStep].ToCharArray()[j + jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (i - iStep >= rowMin && j + jStep < colMax)
            {
                c = m_UpperCharacterMatrix[i - iStep].ToCharArray()[j + jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }
            if (i + iStep < rowMax && j - jStep >= colMin)
            {
                c = m_UpperCharacterMatrix[i + iStep].ToCharArray()[j - jStep];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }

            if (i < rowMax && j >= colMin && i >= rowMin && j < colMax)
            {
                c = m_UpperCharacterMatrix[i].ToCharArray()[j];
                if (c != ' ')
                {
                    charList.Add(c);
                }
            }

            return charList.ToArray();
        }

        public char[] GetPossibleCharacters(char c)
        {
            return m_suggestionDic[c];
        }
    }

        ///<summary>
    /// Word Generator Class, This class generates respelling suggestions in given edit distance for a word
    ///</summary>
    public static class WordGenerator
    {
        //private static LetterMatrix s_LetterMatrix = new LetterMatrix();
        //private static Dictionary<char, char[]> m_substitutionMatrix = new Dictionary<char, char[]>();

        private static long m_accuracy = 1;
        private const int MaxSuggestionCount = 500000;

        /*
        static WordGenerator()
        {
            m_substitutionMatrix.Add('ض', new char[] { 'ص', 'ش', 'س', 'پ' });
            m_substitutionMatrix.Add('پ', new char[] { 'ص', 'ش', 'س', 'ض', 'ج', 'گ', 'چ', 'ژ' });
            m_substitutionMatrix.Add('ص', new char[] { 'ض', 'ث', 'س', 'ی', 'ش', 'پ' });
            m_substitutionMatrix.Add('ث', new char[] { 'ص', 'ق', 'ی', 'ب', 'س' });
            m_substitutionMatrix.Add('ق', new char[] { 'ث', 'ف', 'ب', 'ل', 'ی' });
            m_substitutionMatrix.Add('ف', new char[] { 'ق', 'غ', 'ل', 'ا', 'ب', 'آ' });
            m_substitutionMatrix.Add('غ', new char[] { 'ف', 'ع', 'ا', 'ت', 'ل', 'آ' });
            m_substitutionMatrix.Add('ع', new char[] { 'غ', 'ه', 'ت', 'ن', 'ا', 'آ' });
            m_substitutionMatrix.Add('ه', new char[] { 'ع', 'خ', 'ن', 'م', 'ت' });
            m_substitutionMatrix.Add('خ', new char[] { 'ه', 'ح', 'م', 'ک', 'ن' });
            m_substitutionMatrix.Add('ح', new char[] { 'خ', 'ج', 'ک', 'گ', 'م' });
            m_substitutionMatrix.Add('ج', new char[] { 'ح', 'چ', 'گ', 'ک', 'پ', 'ژ' });
            m_substitutionMatrix.Add('چ', new char[] { 'ج', 'گ', 'ژ', 'پ' });
            m_substitutionMatrix.Add('ش', new char[] { 'س', 'ض', 'ظ', 'ط', 'ص', 'پ' });
            m_substitutionMatrix.Add('س', new char[] { 'ش', 'ی', 'ص', 'ط', 'ض', 'ز', 'ث', 'ظ', 'پ', 'ژ' });
            m_substitutionMatrix.Add('ی', new char[] { 'س', 'ب', 'ث', 'ز', 'ص', 'ر', 'ق', 'ط', 'ژ', 'ؤ' });
            m_substitutionMatrix.Add('ب', new char[] { 'ی', 'ل', 'ق', 'ر', 'ث', 'ذ', 'ف', 'ز', 'ؤ', 'إ', 'ژ' });
            m_substitutionMatrix.Add('ل', new char[] { 'ب', 'ا', 'ف', 'ذ', 'ق', 'د', 'غ', 'ر', 'آ', 'إ', 'أ', 'ؤ' });
            m_substitutionMatrix.Add('ا', new char[] { 'ل', 'ت', 'غ', 'د', 'ف', 'ئ', 'ع', 'ذ', 'أ', 'ء', 'إ', 'آ' });
            m_substitutionMatrix.Add('آ', new char[] { 'ل', 'ت', 'غ', 'د', 'ف', 'ئ', 'ع', 'ذ', 'ا', 'أ', 'ء', 'إ' });
            m_substitutionMatrix.Add('ت', new char[] { 'ا', 'ن', 'ع', 'ئ', 'غ', 'و', 'ه', 'د', 'آ', 'ء', 'أ' });
            m_substitutionMatrix.Add('ن', new char[] { 'ت', 'م', 'ه', 'و', 'ع', 'خ', 'ئ', 'ء' });
            m_substitutionMatrix.Add('م', new char[] { 'ن', 'ک', 'خ', 'ه', 'ح', 'و' });
            m_substitutionMatrix.Add('ک', new char[] { 'م', 'گ', 'ح', 'خ', 'ج' });
            m_substitutionMatrix.Add('گ', new char[] { 'ک', 'ج', 'ح', 'چ', 'ژ', 'پ' });
            m_substitutionMatrix.Add('ژ', new char[] { 'گ', 'چ', 'ج', 'پ', 'ط', 'ر', 'ی', 'س', 'ب', 'ز', 'ؤ' });
            m_substitutionMatrix.Add('ظ', new char[] { 'ط', 'ش', 'س' });
            m_substitutionMatrix.Add('ط', new char[] { 'ظ', 'ز', 'س', 'ش', 'ی', 'ژ' });
            m_substitutionMatrix.Add('ز', new char[] { 'ط', 'ر', 'ی', 'س', 'ب', 'ؤ', 'ژ' });
            m_substitutionMatrix.Add('ر', new char[] { 'ز', 'ذ', 'ب', 'ی', 'ل', 'ژ', 'إ', 'ؤ' });
            m_substitutionMatrix.Add('ؤ', new char[] { 'ز', 'ذ', 'ب', 'ی', 'ل', 'ر', 'ژ', 'إ' });
            m_substitutionMatrix.Add('ذ', new char[] { 'ر', 'د', 'ل', 'ب', 'ا', 'ؤ', 'أ', 'آ', 'إ' });
            m_substitutionMatrix.Add('إ', new char[] { 'ر', 'د', 'ل', 'ب', 'ا', 'ذ', 'ؤ', 'أ', 'آ' });
            m_substitutionMatrix.Add('د', new char[] { 'ذ', 'ئ', 'ا', 'ل', 'ت', 'إ', 'ء', 'آ', 'أ' });
            m_substitutionMatrix.Add('أ', new char[] { 'ذ', 'ئ', 'ا', 'ل', 'ت', 'د', 'إ', 'ء', 'آ' });
            m_substitutionMatrix.Add('ئ', new char[] { 'د', 'و', 'ت', 'ا', 'ن', 'أ', 'آ', 'ء' });
            m_substitutionMatrix.Add('ء', new char[] { 'د', 'و', 'ت', 'ا', 'ن', 'ئ', 'أ', 'آ' });
            m_substitutionMatrix.Add('و', new char[] { 'ئ', 'ن', 'ت', 'م', 'ء' });
        }
        */
        
        private static string[] GetWordsWithOneEditDistanceSubstitute(string word, params char[] alphabet)
        {
            try
            {
                List<string> suggestion = new List<string>();

                foreach (char c in alphabet)
                {
                    for (int i = 0; i < word.Length; ++i)
                    {
                        suggestion.Add(word.Remove(i, 1).Insert(i, c.ToString()));
                    }
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { word };
        }
        
        /*
        private static string[] GetWordsWithOneEditDistanceSubstitute(string word)
        {
            try
            {
                List<string> suggestion = new List<string>();

                for (int i = 0; i < word.Length; ++i)
                {
                    foreach (char c in m_substitutionMatrix[word[i]])
                    {
                        suggestion.Add(word.Remove(i, 1).Insert(i, c.ToString()));
                    }
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { word };
        }
        */

        private static string[] GetWordsWithOneEditDistanceSubstitute(string word)
        {
            try
            {
                List<string> suggestion = new List<string>();

                foreach (char c in PersianAlphabets.AlphabetWithPseudoSpace)
                {
                    for (int i = 0; i < word.Length; ++i)
                    {
                        suggestion.Add(word.Remove(i, 1).Insert(i, c.ToString()));
                    }
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { "" };
        }
        private static string[] GetWordsWithOneEditDistanceTranspose(string word)
        {
            try
            {
                List<string> suggestion = new List<string>();

                for (int i = 0; i < word.Length - 1; ++i)
                {
                    suggestion.Add(word.Remove(i, 1).Insert(i + 1, word[i].ToString()));
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { word };
        }
        private static string[] GetWordsWithOneEditDistanceDelete(string word)
        {
            try
            {
                List<string> suggestion = new List<string>();

                for (int i = 0; i < word.Length; ++i)
                {
                    suggestion.Add(word.Remove(i, 1));
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { word };
        }
        private static string[] GetWordsWithOneEditDistanceInsert(string word, params char[] alphabet)
        {
            try
            {
                List<string> suggestion = new List<string>();

                foreach (char c in alphabet)
                {
                    for (int i = 0; i < word.Length + 1; ++i)
                    {
                        suggestion.Add(word.Insert(i, c.ToString()));
                    }
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { word };
        }
        private static string[] GetWordsWithOneEditDistanceInsert(string word)
        {
            try
            {
                List<string> suggestion = new List<string>();

                foreach (char c in PersianAlphabets.AlphabetWithPseudoSpace)
                {
                    for (int i = 0; i < word.Length + 1; ++i)
                    {
                        suggestion.Add(word.Insert(i, c.ToString()));
                    }
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { word };
        }

        private static string[] GetWordsWithOneEditDistance(string word)
        {
            try
            {
                List<string> suggestion = new List<string>();

                suggestion.AddRange(GetWordsWithOneEditDistanceDelete(word));
                suggestion.AddRange(GetWordsWithOneEditDistanceSubstitute(word));
                suggestion.AddRange(GetWordsWithOneEditDistanceInsert(word));
                suggestion.AddRange(GetWordsWithOneEditDistanceTranspose(word));

                //Added on 28-March-2010
                //Commented on 30-March-2010
                //suggestion.Add(word);

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { word };
        }
        private static string[] GetWordsWithOneEditDistance(string word, RespellingGenerationType respellingType)
        {
            try
            {
                List<string> suggestion = new List<string>();

                if ((respellingType & RespellingGenerationType.Delete) != 0)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistanceDelete(word));
                }
                if ((respellingType & RespellingGenerationType.Substitute) != 0)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistanceSubstitute(word));
                }
                if ((respellingType & RespellingGenerationType.Insert) != 0)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistanceInsert(word));
                }
                if ((respellingType & RespellingGenerationType.Transpose) != 0)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistanceTranspose(word));
                }

                //Added on 28-March-2010
                //Commented on 30-March-2010
                //suggestion.Add(word);

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { word };
        }
        private static string[] GetWordsWithOneEditDistance(string word, RespellingGenerationType respellingType, params char[] alphabet)
        {
            try
            {
                List<string> suggestion = new List<string>();

                if ((respellingType & RespellingGenerationType.Delete) != 0)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistanceDelete(word));
                }
                if ((respellingType & RespellingGenerationType.Substitute) != 0)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistanceSubstitute(word, alphabet));
                }
                if ((respellingType & RespellingGenerationType.Insert) != 0)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistanceInsert(word, alphabet));
                }
                if ((respellingType & RespellingGenerationType.Transpose) != 0)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistanceTranspose(word));
                }

                //Added on 28-March-2010
                //Commented on 30-March-2010
                //suggestion.Add(word);

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { word };
        }

        private static string[] GetWordsTillAnyEditDistance(string[] word, int editDistance)
        {
            try
            {
                List<string> suggestion = new List<string>();

                if (editDistance == 1)
                {
                    foreach (string localWord in word)
                    {
                        suggestion.AddRange(GetWordsWithOneEditDistance(localWord));
                    }
                }
                else
                {
                    foreach (string localWord in word)
                    {
                        suggestion.AddRange(GetWordsTillAnyEditDistance(GetWordsWithOneEditDistance(localWord), editDistance - 1));
                    }
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[0];
        }
        private static string[] GetWordsTillAnyEditDistance(string[] word, int editDistance, RespellingGenerationType respellingType)
        {
            try
            {
                List<string> suggestion = new List<string>();

                if (editDistance == 1)
                {
                    foreach (string localWord in word)
                    {
                        suggestion.AddRange(GetWordsWithOneEditDistance(localWord, respellingType));
                    }
                }
                else
                {
                    foreach (string localWord in word)
                    {
                        suggestion.AddRange(GetWordsTillAnyEditDistance(GetWordsWithOneEditDistance(localWord), editDistance - 1, respellingType));
                    }
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[0];
        }
        private static string[] GetWordsTillAnyEditDistance(string[] word, int editDistance, RespellingGenerationType respellingType, params char[] alphabet)
        {
            try
            {
                List<string> suggestion = new List<string>();

                if (editDistance == 1)
                {
                    foreach (string localWord in word)
                    {
                        suggestion.AddRange(GetWordsWithOneEditDistance(localWord, respellingType, alphabet));
                    }
                }
                else
                {
                    foreach (string localWord in word)
                    {
                        suggestion.AddRange(GetWordsTillAnyEditDistance(GetWordsWithOneEditDistance(localWord, respellingType, alphabet), editDistance - 1, respellingType, alphabet));
                    }
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[0];
        }
        
        ///<summary>
        /// Generates respelling suggestions in given edit distance for a word
        ///</summary>
        ///<param name="word">Word</param>
        ///<param name="editDistance">Edit Distance</param>
        ///<returns>Respelling Suggestions</returns>
        public static string[] GenerateRespelling(string word, int editDistance)
        {
            try
            {
                Stopwatch time = new Stopwatch();
                time.Start();
                List<string> suggestion = new List<string>();

                if (editDistance == 1)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistance(word));
                }
                else
                {
                    suggestion.AddRange(GetWordsTillAnyEditDistance(GetWordsWithOneEditDistance(word), editDistance - 1));
                }
                time.Stop();
                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions for any edit distance: {0}", ex.Message));
            }

            return new string[] { word };
        }

        ///<summary>
        /// Generates respelling suggestions in given edit distance for a word
        ///</summary>
        ///<param name="word">Word</param>
        ///<param name="editDistance">Edit Distance</param>
        ///<param name="respellingType">Type of generating respelling, logically OR desired types</param>
        ///<returns>Respelling Suggestions</returns>
        public static string[] GenerateRespelling(string word, int editDistance, RespellingGenerationType respellingType)
        {
            try
            {
                List<string> suggestion = new List<string>();

                if (editDistance == 1)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistance(word, respellingType));
                }
                else
                {
                    suggestion.AddRange(GetWordsTillAnyEditDistance(GetWordsWithOneEditDistance(word), editDistance - 1, respellingType));
                }

                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions for any edit distance: {0}", ex.Message));
            }

            return new string[] { word };
        }

        ///<summary>
        /// Generates respelling suggestions in given edit distance for a word
        ///</summary>
        ///<param name="word">Word</param>
        ///<param name="editDistance">Edit Distance</param>
        ///<param name="respellingType">Type of generating respelling, logically OR desired types</param>
        ///<param name="alphabet">List of charachters used to generate respelling</param>
        ///<returns>Respelling Suggestions</returns>
        public static string[] GenerateRespelling(string word, int editDistance, RespellingGenerationType respellingType, params char[] alphabet)
        {
            try
            {
                List<string> suggestion = new List<string>();

                if (editDistance == 1)
                {
                    suggestion.AddRange(GetWordsWithOneEditDistance(word, respellingType, alphabet));
                }
                else
                {
                    suggestion.AddRange(GetWordsTillAnyEditDistance(GetWordsWithOneEditDistance(word, respellingType, alphabet), editDistance - 1, respellingType, alphabet));
                }

                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions for any edit distance: {0}", ex.Message));
            }

            return new string[] { word };
        }

        /// <summary>
        /// Accuracy of generating homophone words, It can be a number between 0 to 1 which 0 means minimum accuracy but fastest and 1 means maximum accuracy but slowest
        /// </summary>
        /// <param name="p"></param>
        public static void SetAccuracy(double p)
        {
            if (p <= 0 || p > 1)
            {
                p = 1;
            }

            m_accuracy = (long)(MaxSuggestionCount * p);
        }

        ///<summary>
        /// Generate homophone words of given word, homophone words are those that can pronounce the same
        ///</summary>
        ///<param name="word">Word</param>
        ///<returns>Homophone Words</returns>
        public static string[] GenerateHomophoneWords(string word)
        {
            try
            {
                List<string> suggestion = new List<string>();
                suggestion.Add(word);

                foreach (char[] homoPhoneFamily in PersianHomophoneLetters.AllHomophones)
                {
                    suggestion.AddRange(GenerateHomophoneWords(suggestion.ToArray(), homoPhoneFamily));
                }

                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in generating suggestions: {0}", ex.Message));
            }

            return new string[] { word };
        }

        private static string[] GenerateHomophoneWords(string[] word, char[] homophones)
        {
            List<string> suggestion = new List<string>(word);
            string[] tmpSuggestion;

            int maxWordLength = word[0].Length;
            for (int i = 0; i < maxWordLength; ++i)
            {
                tmpSuggestion = suggestion.ToArray();
                foreach (string localWord in tmpSuggestion)
                {
                    if (Array.IndexOf(homophones, localWord[i]) != -1)
                    {
                        suggestion.AddRange(GetHomophonePermutation(localWord, homophones, i));
                    }

                    if (suggestion.Count > m_accuracy)
                    {
                        break;
                    }
                }
            }

            return suggestion.ToArray();
        }
        private static string[] GetHomophonePermutation(string word, char[] homophones, int startIndex)
        {
            List<string> suggestion = new List<string>();

            foreach (char c in homophones)
            {
                if (word[startIndex] != c)
                {
                    suggestion.Add(word.Remove(startIndex, 1).Insert(startIndex, c.ToString()));

                    //if (!suggestion.Contains(tmp))
                    //{
                    //    suggestion.Add(tmp);
                    //}
                }
            }

            return suggestion.Distinct().ToArray();
        }
        private static void AddNonIteratedValues(List<string> suggestion, string[] p)
        {
            foreach (string str in p)
            {
                if (!suggestion.Contains(str))
                {
                    suggestion.Add(str);
                }
            }
        }

    }
}

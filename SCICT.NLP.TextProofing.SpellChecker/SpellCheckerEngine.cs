// Virastyar
// http://www.virastyar.ir
// Copyright (C) 2011 Supreme Council for Information and Communication Technology (SCICT) of Iran
// 
// This file is part of Virastyar.
// 
// Virastyar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Virastyar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Virastyar.  If not, see <http://www.gnu.org/licenses/>.
// 
// Additional permission under GNU GPL version 3 section 7
// The sole exception to the license's terms and requierments might be the
// integration of Virastyar with Microsoft Word (any version) as an add-in.

using SCICT.NLP.Morphology.Inflection;
using SCICT.NLP.Morphology.Inflection.Conjugation;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility.Parsers;
using SCICT.NLP.Utility.StringDistance;
using SCICT.NLP.Utility.WordContainer;
using SCICT.NLP.Utility.WordGenerator;

namespace SCICT.NLP.TextProofing.SpellChecker
{
    ///<summary>
    /// configuration Class of Spell Checker
    ///</summary>
    public class SpellCheckerConfig
    {
        ///<summary>
        /// The absolute path of stem's file.
        ///</summary>
        public string StemPath;
        ///<summary>
        /// The absolute path of dictionary file.
        ///</summary>
        public string DicPath;
        ///<summary>
        /// Indicates the Maximum Edit Distance that searched for finding suggestions
        ///</summary>
        public int    EditDistance;
        ///<summary>
        /// Number of Suggestions
        ///</summary>
        public int    SuggestionCount;

        ///<summary>
        /// Constructor Class
        ///</summary>
        public SpellCheckerConfig()
        {

        }

        ///<summary>
        /// Constructor Class
        ///</summary>
        ///<param name="dicPath">Absolute path of dictionary file</param>
        ///<param name="editDist">Maximum Edit Distance that searched for finding suggestions</param>
        ///<param name="sugCnt">Number of Suggestions</param>
        public SpellCheckerConfig(string dicPath, int editDist, int sugCnt)
        {
            this.DicPath = dicPath;
            this.EditDistance = editDist;
            this.SuggestionCount = sugCnt;
        }
    }

    public class DictionaryProblemException : Exception
    {

    } 

    ///<summary>
    /// Spell Checker Engine
    /// This Class find and rank respelling suggestions for a incorrectly spelled word
    ///</summary>
    public class SpellCheckerEngine
    {
        #region Members 

        private const int MMeanTermLength = 30;

        //private Dictionary<string, int> Entry = new Dictionary<string, int>();
        AutoCompleteWordContainerTree m_wordContainer;
        WordContainerTree m_ignoreList;

        ///<summary>
        /// Indicates the Maximum Edit Distance that searched for finding suggestions
        ///</summary>
        public int EditDistance { get; private set; } // = 2;
        ///<summary>
        /// Number of Suggestions
        ///</summary>
        public int SuggestionCount{ get; private set; } //= 7;
        ///<summary>
        /// The absolute path of dictionary file.
        ///</summary>
        public string DictionaryFileName { get; private set; }//;
        ///<summary>
        /// The absolute path of stem's file.
        ///</summary>
        public string StemFileName { get; private set; }//;
        
        ///<summary>
        /// Number of dictionary words
        ///</summary>
        public long DicWordCount
        { 
            get
            {
                if (this.m_wordContainer == null)
                {
                    return 0;
                }

                return this.m_wordContainer.DictionaryWordsCount;
            } 
        }

        private Dictionary<string, int> m_localFreqCatche = new Dictionary<string, int>();

        protected Dictionary<string, string> m_rankingDetail = new Dictionary<string, string>();

        #endregion
        
        #region Protected Members
  
        //protected string currentWord   = null;
        protected bool   m_isInitialized = false;
        protected const int MaxWordLengthToCheck = 11;
        
        #endregion

        #region Constructor

        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="config">Spellchecker Configuration</param>
        public SpellCheckerEngine(SpellCheckerConfig config)
        {
            Init(config);
        }

        public SpellCheckerEngine(string dicPath, string stemPath, int editDist, int sugCnt)
        {
            SpellCheckerConfig config = new SpellCheckerConfig();
            config.DicPath = dicPath;
            config.EditDistance = editDist;
            config.StemPath = stemPath;
            config.SuggestionCount = sugCnt;

            Init(config);
        }
        /*
        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="config">Spellchecker Configuration</param>
        ///<param name="affixCheckForNewWords">Accept affix combination for further added words</param>
        public SpellCheckerEngine(SpellCheckerConfig config, bool affixCheckForNewWords)
        {
            //Init(config);
            Init(config, affixCheckForNewWords);
        }
        */

        private void Init(SpellCheckerConfig config)
        {
            this.EditDistance = config.EditDistance;
            this.DictionaryFileName = config.DicPath;
            this.SuggestionCount = config.SuggestionCount;
            this.StemFileName = config.StemPath;

            WordContainerTreeConfig wordContainerConfig = new WordContainerTreeConfig();
            wordContainerConfig.DictionaryFileName = this.DictionaryFileName;
            wordContainerConfig.SuggestionCount = this.SuggestionCount;

            try
            {
                this.m_wordContainer = new AutoCompleteWordContainerTree(wordContainerConfig);
                if (!CheckDictionaryCorrectness(DictionaryFileName, m_wordContainer.DictionaryWordsCount))
                {
                    throw new DictionaryProblemException();
                }

                this.m_ignoreList = new WordContainerTree();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                VerbInfoContainer verbInfoContainer = new VerbInfoContainer();
                verbInfoContainer.LoadStemFile(StemFileName);

                Conjugator conjugator = new Conjugator(verbInfoContainer);
                string[] conjugations = conjugator.Conjugate(ENUM_VERB_TYPE.SADE | ENUM_VERB_TYPE.PISHVANDI | ENUM_VERB_TYPE.INFINITIVE);

                foreach (string verb in conjugations)
                {
                    this.m_wordContainer.AddWordBlind(verb, 0, PersianPOSTag.V);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            this.m_isInitialized = true;
        }
        
        /*
        private void Init(SpellCheckerConfig config, bool affixCheckForNewWords)
        {
            this.EditDistance = config.EditDistance;
            this.DictionaryFileName = config.DicPath;
            this.SuggestionCount = config.SuggestionCount;

            WordContainerTreeConfig wordContainerConfig = new WordContainerTreeConfig();
            wordContainerConfig.DictionaryFileName = this.DictionaryFileName;
            wordContainerConfig.SuggestionCount = this.SuggestionCount;

            this.m_autoCompleteWordContainer = new AutoCompleteWordContainerTree(wordContainerConfig, affixCheckForNewWords);

            this.m_isInitialized = true;
        }
        */

        #endregion

        #region Private Methods

        #region Ranking

        /*
        private Dictionary<string, int> CatchFrequency(string[] words)
        {

            Dictionary<string, int> count = new Dictionary<string, int>();
            int freq = 0;

            foreach (string word in words)
            {
                if (!count.ContainsKey(word))
                {
                    freq = WordFrequency(word);
                    count.Add(word, freq);
                }
            }

            return count;
        }
        */
        
        private Dictionary<string, int> CatchFrequency(string[] words)
        {
            List<string> userWords = new List<string>();
            Dictionary<string, int> count = new Dictionary<string, int>();
            int freq;

            foreach (string word in words)
            {
                if (!count.ContainsKey(word))
                {
                    freq = WordFrequency(word);
                    if (freq == 0)
                    {
                        userWords.Add(word);
                    }
                    else
                    {
                        count.Add(word, freq);
                    }
                }
            }

            //double val = count.Count > 0 ? count.Values.Average() + (count.Values.Max() - count.Values.Average()) / 1.4 : 1;
            double val = count.Count > 0 ? count.Values.Average() / 1.4 : 1;

            foreach(string str in userWords)
            {
                count.Add(str, (int)val);
            }

            return count;
        }

        private Dictionary<string, int> CatchFrequencyWithAffixConsideration(string[] words, out long avg)
        {
            avg = 0;

            Dictionary<string, int> count = new Dictionary<string, int>();
            int freq = 0;
            PersianPOSTag posTag;

            PersianSuffixLemmatizer suffixRecognizer = new PersianSuffixLemmatizer(false, true);

            foreach (string word in words)
            {
                if (!count.ContainsKey(word))
                {
                    if (IsRealWord(word, out freq, out posTag))
                    {
                        avg += freq;
                        count.Add(word, freq);
                    }
                    else
                    {
                        ReversePatternMatcherPatternInfo[] patternInfoArray = suffixRecognizer.MatchForSuffix(word);
                        foreach (ReversePatternMatcherPatternInfo patternInfo in patternInfoArray)
                        {
                            if (IsRealWord(patternInfo.BaseWord, out freq, out posTag))
                            {
                                avg += freq;
                                count.Add(word, freq);
                                break;
                            }
                        }
                    }
                }
            }

            avg = avg / words.Length;

            return count;
        }

        private Dictionary<string, int> CatchFrequencyMitigator(string[] words, out long localAvg, out long localSum)
        {

            List<string> userEnteredWords = new List<string>();

            int tmplocalAvg = 0, freq;

            foreach (string word in words)
            {
                if (!this.m_localFreqCatche.ContainsKey(word))
                {
                    freq = WordFrequency(word);
                    if (freq == 0)
                    {
                        userEnteredWords.Add(word);
                    }
                    else
                    {
                        tmplocalAvg += freq;

                        this.m_localFreqCatche.Add(word, freq);
                    }
                }
                else
                {
                    tmplocalAvg += this.m_localFreqCatche[word];
                }
            }

            localSum = tmplocalAvg;
            tmplocalAvg = tmplocalAvg / (words.Length - userEnteredWords.Count);

            foreach (string word in userEnteredWords)
            {
                if (!this.m_localFreqCatche.ContainsKey(word))
                {
                    localSum += tmplocalAvg;
                    this.m_localFreqCatche.Add(word, tmplocalAvg);
                }
            }


            localAvg = localSum / Convert.ToInt64(words.Length);

            return this.m_localFreqCatche;
        }

        private void SortByCount(string[] words)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                Dictionary<string, int> localCatch = CatchFrequency(words);

                string temp;
                for (int i = 0; i < words.Length - 1; ++i)
                {
                    for (int j = i + 1; j < words.Length; ++j)
                    {
                        if (localCatch[words[i]] < localCatch[words[j]])
                        {
                            temp = words[i];
                            words[i] = words[j];
                            words[j] = temp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void HybridSort(string baseWord, string[] words)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                //Dictionary<string, int> frequency = CatchFrequencyMitigator(words, out freqAvg, out freqSum);
                Dictionary<string, int> frequency = CatchFrequency(words);

                
                double freqSum; 
                try
                {
                    freqSum = frequency.Count > 0 ? frequency.Values.Sum() : 1;
                }
                catch(OverflowException)
                {
                    freqSum = 1;
                }

                double freqAvg;
                try
                {
                    freqAvg = frequency.Count > 0 ? frequency.Values.Average() : 0;
                }
                catch (OverflowException)
                {
                    freqAvg = 0;
                }


                Dictionary<string, double> similarity = CacheSimilarities(baseWord, words);
                double simSum;
                try
                {
                    simSum = similarity.Count > 0 ? similarity.Values.Sum() : 1;
                }
                catch (OverflowException)
                {
                    simSum = 1;
                }

                double simAvg;
                try
                {
                    simAvg = similarity.Count > 0 ? similarity.Values.Average() : 0;

                }
                catch (OverflowException)
                {
                    simAvg = 0;
                }

                double uniOrderizerCo = freqAvg / simAvg;

                double similarityDeviation = StandardDeviation(similarity, simAvg);
                //double simEffectCo = 1 / (similarityDeviation);

                double freqDeviation = StandardDeviation(frequency, freqAvg);
                //double freqEffectCo = 1 / (freqDeviation / freqAvg);
                 

                Dictionary<string, double> hybridMeasure = new Dictionary<string, double>();
                foreach(KeyValuePair<string, double> pair in similarity)
                {
                    hybridMeasure.Add(pair.Key, (2 - similarityDeviation) * (pair.Value) + .8 * (frequency[pair.Key] / freqSum));

                    //hybridMeasure.Add(pair.Key, (pair.Value / simSum) * (frequency[pair.Key] / freqSum));
                    //hybridMeasure.Add(pair.Key, ((pair.Value * uniOrderizerCo) + localCatch[pair.Key]) * pair.Value);

                    //hybridMeasure.Add(pair.Key, (pair.Value * uniOrderizerCo) * simEffectCo + frequency[pair.Key] * pair.Value * (1 - similarityDeviation));

                    #region Add ranking detail

                    if (!m_rankingDetail.ContainsKey(pair.Key))
                    {
                        m_rankingDetail.Add(pair.Key, frequency[pair.Key].ToString() + ", " + pair.Value.ToString());
                    }

                    #endregion

                }

                string temp;
                for (int i = 0; i < words.Length - 1; ++i)
                {
                    for (int j = i + 1; j < words.Length; ++j)
                    {
                        if (hybridMeasure[words[i]] < hybridMeasure[words[j]])
                        {
                            temp = words[i];
                            words[i] = words[j];
                            words[j] = temp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static long CalcSessionAvgFreq(Dictionary<string, int> freqDic)
        {
            long avg = 0;

            foreach(KeyValuePair<string, int> pair in freqDic)
            {
                avg += pair.Value;
            }

            return avg / freqDic.Count;
        }

        private static double StandardDeviation(Dictionary<string, double> dictionary, double avg)
        {
            double deviation = 0;

            foreach(KeyValuePair<string, double> pair in dictionary)
            {
                deviation += Math.Pow((pair.Value - avg), 2);
            }

            deviation = Math.Sqrt(deviation / dictionary.Count);

            return deviation;
        }

        private static double StandardDeviation(Dictionary<string, int> dictionary, double avg)
        {
            double deviation = 0;

            foreach (KeyValuePair<string, int> pair in dictionary)
            {
                deviation += Math.Pow((pair.Value - avg), 2);
            }

            deviation = Math.Sqrt(deviation / dictionary.Count);

            return deviation;
        }

        private void SortBySimilarityMeasure(string baseWord, string[] words)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                //Stopwatch stopWatch = new Stopwatch();
                //stopWatch.Start();

                Dictionary<string, double> similarity = CacheSimilarities(baseWord, words);
                string temp;
                for (int i = 0; i < words.Length - 1; ++i)
                {
                    for (int j = i + 1; j < words.Length; ++j)
                    {
                        if (similarity[words[i]] < similarity[words[j]])
                        {
                            temp = words[i];
                            words[i] = words[j];
                            words[j] = temp;
                        }
                    }
                }

                //stopWatch.Stop();
                //if (words.Length > 70)
                //{
                //    using (StreamWriter writer = new StreamWriter("c:/SpellCorrector/sort.log", true))
                //    {
                //        writer.WriteLine(DateTime.Now.ToString() + " - Current Word: " + this.currentWord + " - Word Count: " + words.Length + " - Sort Time: " + stopWatch.ElapsedMilliseconds.ToString());
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static Dictionary<string, double> CacheSimilarities(string baseWord, string[] words)
        {
           Dictionary<string, double> similarity = new Dictionary<string, double>();
            
            StringDistanceLayout editDistance = new StringDistanceLayout();

          
            foreach (string word in words)
            {
                if (!similarity.ContainsKey(word))
                {
                    double sim = editDistance.GetWordSimilarity(baseWord, word, StringDistanceAlgorithm.Kashefi);
                    similarity.Add(word, sim);
                }
            }

            return similarity;
        }

        #endregion

        private string[] GetAllsuggestion(string word, int editdistance)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                List<string> suggestion = new List<string>();

                if (!IsRealWord(word))
                {
                    suggestion.AddRange(ExtractRealWords(WordGenerator.GenerateRespelling(word, editdistance)));
                }
                else
                {
                    suggestion.AddRange(ExtractRealWords(WordGenerator.GenerateRespelling(word, 1)));
                    //suggestion.Add(word);
                }

                return suggestion.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Reconfigure the engine
        /// </summary>
        /// <param name="sc">Spellchecker Configuration</param>
        /// <returns></returns>
        public bool Reconfigure(SpellCheckerConfig sc)
        {
            this.EditDistance = sc.EditDistance;
            this.SuggestionCount = sc.SuggestionCount;

            if (this.DictionaryFileName != sc.DicPath)
            {
                this.DictionaryFileName = sc.DicPath;

                this.m_wordContainer = null;

                WordContainerTreeConfig actc = new WordContainerTreeConfig();
                actc.DictionaryFileName = this.DictionaryFileName;
                actc.SuggestionCount = this.SuggestionCount;
                
                try
                {
                    this.m_wordContainer = new AutoCompleteWordContainerTree(actc);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                VerbInfoContainer verbInfoContainer = new VerbInfoContainer();
                verbInfoContainer.LoadStemFile(StemFileName);

                Conjugator conjugator = new Conjugator(verbInfoContainer);
                string[] conjugations = conjugator.Conjugate(ENUM_VERB_TYPE.SADE | ENUM_VERB_TYPE.PISHVANDI | ENUM_VERB_TYPE.INFINITIVE);

                foreach (string verb in conjugations)
                {
                    this.m_wordContainer.AddWordBlind(verb, 0, PersianPOSTag.V);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        
        /// <summary>
        /// Append another dictionary
        /// </summary>
        /// <param name="fileName">dictionary file name</param>
        ///<returns>True if dictionary is successfully added, otherwise False</returns>
        public bool AppendDictionary(string fileName)
        {
            try
            {
                if (!this.m_isInitialized)
                {
                    throw new Exception("Speller Engine Must be Initialized!");
                }

                int extractedWordCount = this.m_wordContainer.AppendDictionary(fileName);
                return CheckDictionaryCorrectness(fileName, extractedWordCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///<summary>
        /// Check Dictionary Correctness
        ///</summary>
        ///<param name="fileName">File name</param>
        ///<param name="wordCounts">Extracted words count</param>
        ///<returns></returns>
        public static bool CheckDictionaryCorrectness(string fileName, long wordCounts)
        {
            long numberOfExpectedWords = new FileInfo(fileName).Length / MMeanTermLength;
            if (wordCounts < 0.8 * numberOfExpectedWords)
            {
                return false;
            }

            return true;
        }

        ///<summary>
        /// Remove all words from dictionary
        ///</summary>
        ///<exception cref="Exception"></exception>
        public void ClearDictionary()
        {
            try
            {
                if (!this.m_isInitialized)
                {
                    throw new Exception("Speller Engine Must be Initialized!");
                }

                DictionaryFileName = "";

                this.m_wordContainer.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add a word to Ignore List
        /// </summary>
        /// <param name="word">word to be ignored</param>
        ///<returns>True if word is successfully added, otherwise False</returns>
        public bool AddToIgnoreList(string word)
        {
            try
            {
                if (!this.m_isInitialized)
                {
                    throw new Exception("Speller Engine Must be Initialized!");
                }

                return m_ignoreList.AddWordBlind(word);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Add Word to Dictionary

        /// <summary>
        /// Add a correct word to dictionary
        /// </summary>
        /// <param name="word">New word</param>
        /// <param name="freq">Usage frequency of word</param>
        ///<returns>True if word is successfully added, otherwise False</returns>
        public bool AddToDictionary(string word)
        {
            try
            {
                if (!this.m_isInitialized)
                {
                    throw new Exception("Speller Engine Must be Initialized!");
                }

                return AddToDictionary(word, 0, PersianPOSTag.UserPOS);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add a correct word to dictionary
        /// </summary>
        /// <param name="word">New word</param>
        /// <param name="fileName">File name</param>
        ///<returns>True if word is successfully added, otherwise False</returns>
        public bool AddToDictionary(string word, string fileName)
        {
            try
            {
                if (!this.m_isInitialized)
                {
                    throw new Exception("Speller Engine Must be Initialized!");
                }

                return AddToDictionary(word, 0, PersianPOSTag.UserPOS, fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add a correct word to dictionary
        /// </summary>
        /// <param name="word">New word</param>
        /// <param name="freq">Usage frequency of word</param>
        /// <param name="posTag">POS tag of word</param>
        ///<returns>True if word is successfully added, otherwise False</returns>
        public bool AddToDictionary(string word, int freq, PersianPOSTag posTag)
        {
            try
            {
                if (!this.m_isInitialized)
                {
                    throw new Exception("Speller Engine Must be Initialized!");
                }

                return this.m_wordContainer.AddWord(word, freq, posTag);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add a correct word to dictionary
        /// </summary>
        /// <param name="word">New word</param>
        /// <param name="freq">Usage frequency of word</param>
        /// <param name="posTag">POS tag of word</param>
        /// <param name="fileName">File name</param>
        ///<returns>True if word is successfully added, otherwise False</returns>
        public bool AddToDictionary(string word, int freq, PersianPOSTag posTag, string fileName)
        {
            try
            {
                if (!this.m_isInitialized)
                {
                    throw new Exception("Speller Engine Must be Initialized!");
                }

                return this.m_wordContainer.AddWord(word, freq, posTag, fileName);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Remove a word from dictionary
        /// </summary>
        /// <param name="word">input word</param>
        public void RemoveFromDictionary(string word)
        {
            try
            {
                if (!this.m_isInitialized)
                {
                    throw new Exception("Speller Engine Must be Initialized!");
                }

                this.m_wordContainer.RemoveWord(word);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check if the word is correct (exists in dictionary)
        /// </summary>
        /// <param name="word">Word</param>
        /// <returns>True if word is correct, Otherwise False</returns>
        public virtual bool IsRealWord(string word)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                if (this.m_wordContainer.Contain(word))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check if the word is in ignore list
        /// </summary>
        /// <param name="word">Word</param>
        /// <returns>True if word is exist, Otherwise False</returns>
        public virtual bool IsInIgnoreList(string word)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                if (this.m_ignoreList.Contain(word))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check if the word is correct (exists in dictionary)
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="freq">Frequency</param>
        /// <param name="posTag">POS tag</param>
        /// <returns>True if word is correct, Otherwise False</returns>
        ///<exception cref="Exception"></exception>
        public virtual bool IsRealWord(string word, out int freq, out PersianPOSTag posTag)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {

                //if (this.m_autocompletewordcontainer.contain(word, out isbaseword))
                //{
                //    return true;
                //}

                if (this.m_wordContainer.Contain(word, out freq, out posTag))
                {
                    return true;
                }


                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get All dictionary's words
        /// </summary>
        /// <returns></returns>
        public string[] GetAllWords()
        {
            return this.m_wordContainer.GetAllWords();
        }

        /// <summary>
        /// Complete the rest of incomplete word
        /// </summary>
        /// <param name="subWord">Incomplete word</param>
        /// <returns>Completed words start with incomplete word</returns>
        public string[] CompleteWord(string subWord)
        {
            return this.m_wordContainer.Complete(subWord);
        }

        /// <summary>
        /// Complete the rest of incomplete word
        /// </summary>
        /// <param name="subWord">Incomplete word</param>
        /// <param name="count">Number of returned suggestions</param>
        /// <returns>Completed words start with incomplete word</returns>
        public string[] CompleteWord(string subWord, int count)
        {
            return this.m_wordContainer.Complete(subWord, count);
        }

        #region Ranking

        /// <summary>
        /// Return most frequent word from list of words
        /// </summary>
        /// <param name="words">list of words</param>
        /// <returns>Most frequent word</returns>
        public string   SortSuggestions(string[] words)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                if (words.Length <= 0)
                {
                    return "";
                }
                if (words.Length == 1)
                {
                    return words[0];
                }

                SortByCount(words);

                return words[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Sort a list of words ordered by usage frequency
        /// </summary>
        /// <param name="words">list of words</param>
        /// <param name="suggestionCount">Number of returned suggestions</param>
        /// <returns>Sorted list</returns>
        public string[] SortSuggestions(string[] words, int suggestionCount)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            if (words == null)
            {
                return new string[0];
            }
            if (words.Length <= 0)
            {
                return new string[0];
            }
            if (words.Length == 1)
            {
                return words;
            }

            string[] suggestions = new string[Math.Min(suggestionCount, words.Length)];

            try
            {
                SortByCount(words);

                for (int i = 0; i < suggestions.Length; ++i)
                {
                    suggestions[i] = words[i];
                }

                return suggestions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// return most string similar word of list with givven word
        /// </summary>
        /// <param name="baseWord">Base word used to compare (Usually incorrect word)</param>
        /// <param name="words">List of words (Usually list of respelling suggestions)</param>
        /// <returns>most similar word</returns>
        public string   SortSuggestions(string baseWord, string[] words)
        {
            if (baseWord == null) throw new ArgumentNullException("baseWord");
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                if (words.Length <= 0)
                {
                    return "";
                }
                if (words.Length == 1)
                {
                    return words[0];
                }

                //SortBySimilarityMeasure(baseWord, words);
                HybridSort(baseWord, words);

                return words[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// return most string similar word of list with givven word
        /// </summary>
        /// <param name="baseWord">Base word used to compare (Usually incorrect word)</param>
        /// <param name="words">List of words (Usually list of respelling suggestions)</param>
        /// <param name="freq">Word's frequency</param>
        /// <returns>most similar word</returns>
        protected string SortSuggestions(string baseWord, string[] words, Dictionary<string, int> freq)
        {
            if (baseWord == null) throw new ArgumentNullException("baseWord");
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                if (words.Length <= 0)
                {
                    return "";
                }
                if (words.Length == 1)
                {
                    return words[0];
                }

                this.m_localFreqCatche = freq;
                //SortBySimilarityMeasure(baseWord, words);
                HybridSort(baseWord, words);


                return words[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sort a list of words ordered by string similarity
        /// </summary>
        /// <param name="baseWord">Base word used to compare (Usually incorrect word)</param>
        /// <param name="words">List of words (Usually list of respelling suggestions)</param>
        /// <param name="suggestionCount">Number of returned suggestions</param>
        /// <returns>Sorted list</returns>
        public string[] SortSuggestions(string baseWord, string[] words, int suggestionCount)
        {

            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            if (words == null)
            {
                return new string[0];
            }
            if (words.Length <= 0)
            {
                return new string[0];
            }
            //if (words.Length == 1)
            //{
            //    return words;
            //}

            int sugLength = suggestionCount;

            if (words.Length < suggestionCount)
            {
                sugLength = words.Length;
            }

            if (sugLength <= 0)
            {
                sugLength = 1;
            }

            string[] suggestions = new string[sugLength];

            try
            {
                //SortBySimilarityMeasure(baseWord, words);
                HybridSort(baseWord, words);

                for (int i = 0; i < Math.Min(suggestions.Length, words.Length); ++i)
                {
                    suggestions[i] = words[i];
                }

                return suggestions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sort a list of words ordered by string similarity
        /// </summary>
        /// <param name="baseWord">Base word used to compare (Usually incorrect word)</param>
        /// <param name="words">List of words (Usually list of respelling suggestions)</param>
        /// <param name="freq">Word frequency</param>
        /// <param name="suggestionCount">Number of returned suggestions</param>
        /// <returns>Sorted list</returns>
        protected string[] SortSuggestions(string baseWord, string[] words, Dictionary<string, int> freq, int suggestionCount)
        {

            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            if (words == null)
            {
                return new string[0];
            }
            if (words.Length <= 0)
            {
                return new string[0];
            }
            if (words.Length == 1)
            {
                return words;
            }

            string[] suggestions = new string[Math.Min(suggestionCount, words.Length)];

            try
            {
                this.m_localFreqCatche = freq;
                
                HybridSort(baseWord, words);

                for (int i = 0; i < suggestions.Length; ++i)
                {
                    suggestions[i] = words[i];
                }

                return suggestions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        /// <summary>
        /// Return dictionary (correct) words
        /// </summary>
        /// <param name="words">List of words</param>
        /// <returns>List of dictionary (correct) words</returns>
        public string[] ExtractRealWords(string[] words)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                List<string> suggestion = new List<string>();
                foreach (string word in words)
                {
                    if (this.IsRealWord(word) || word.Length == 0)
                    {
                        suggestion.Add(word);
                        //if (!suggestion.Contains(word))
                        //{
                        //    suggestion.Add(word);
                        //}
                    }
                }

                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Return dictionary words including POS
        /// </summary>
        /// <param name="words">List of words</param>
        /// <param name="freqDic">Frequency</param>
        /// <param name="posDic">POS tag</param>
        /// <returns>List of dictionary words</returns>
        public string[]  ExtractRealWords(string[] words, out Dictionary<string, int> freqDic, out Dictionary<string, PersianPOSTag> posDic)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                posDic = new Dictionary<string, PersianPOSTag>();
                freqDic = new Dictionary<string, int>();
                List<string> suggestions = new List<string>();

                PersianPOSTag posTag;
                int freq;
                foreach (string word in words)
                {
                    if (this.IsRealWord(word, out freq, out posTag) || word.Length == 0)
                    {
                        if (!suggestions.Contains(word))
                        {
                            suggestions.Add(word);
                            posDic.Add(word, posTag);
                            freqDic.Add(word, freq);
                        }
                    }
                }

                return suggestions.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get all correct respelling suggestions of an incorrect word including their POS tag
        /// </summary>
        /// <param name="word">(Incorrect) Word</param>
        /// <param name="freqDic">Frequency</param>
        /// <param name="posDic">POS Tag</param>
        /// <returns>List of correct respellings</returns>
        public string[] SpellingSuggestions(string word, out Dictionary<string, int> freqDic, out Dictionary<string, PersianPOSTag> posDic)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            try
            {
                freqDic = new Dictionary<string, int>();
                posDic = new Dictionary<string, PersianPOSTag>();

                if (!IsRealWord(word))
                {
                    return ExtractRealWords(WordGenerator.GenerateRespelling(word, this.EditDistance), out freqDic, out posDic).Distinct().ToArray();
                }

                return new string[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Get a list of sorted (ranked) correct respelling suggestions of an incorrect word by similarity
        /// </summary>
        /// <param name="word">Wrong Word</param>
        /// <param name="suggestionCount">Number of returned suggestions</param>
        /// <returns>Sorted list of correct respelling suggestions</returns>
        public virtual string[] RankedSpellingSuggestions(string word, int suggestionCount)
        {
            ///omid: recheck
            
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            List<string> suggestion = new List<string>();

            try
            {
                ///Double Check
                int effectiveEditDistance = word.Length > MaxWordLengthToCheck ? 1 : this.EditDistance;

                #region Homophone respelling suggestoins

                if (word.Length > MaxWordLengthToCheck)
                {
                    WordGenerator.SetAccuracy(0.1);
                }
                else
                {
                    WordGenerator.SetAccuracy(0.5);
                }

                suggestion.AddRange(ExtractRealWords(WordGenerator.GenerateHomophoneWords(word)).ToArray());

                #endregion

                suggestion.AddRange(SortSuggestions(word, GetAllsuggestion(word, effectiveEditDistance),
                                                    Math.Abs(suggestionCount - suggestion.Count)));

                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get a list of correct respelling and homophone suggestions of an incorrect word by similarity
        /// </summary>
        /// <param name="word">Wrong Word</param>
        /// <returns>Sorted list of correct respelling suggestions</returns>
        public virtual string[] SpellingSuggestions(string word)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            List<string> suggestion = new List<string>();

            try
            {
                #region Homophone respelling suggestoins

                if (word.Length > MaxWordLengthToCheck)
                {
                    WordGenerator.SetAccuracy(0.1);
                }
                else
                {
                    WordGenerator.SetAccuracy(0.5);
                }

                suggestion.AddRange(ExtractRealWords(WordGenerator.GenerateHomophoneWords(word).ToArray()));

                #endregion

                int effectiveEditDistance = word.Length > MaxWordLengthToCheck ? 1 : this.EditDistance;

                for (int i = 1; i <= effectiveEditDistance; ++i)
                {
                    suggestion.AddRange(GetAllsuggestion(word, i));

                    //if (suggestion.Count > SuggestionCount / 2)
                    if (suggestion.Count > 0)
                    {
                        break;
                    }
                }

                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get a list of correct respelling and homophone suggestions of an incorrect word by similarity
        /// </summary>
        /// <param name="word">Wrong Word</param>
        /// <returns>Sorted list of correct respelling suggestions</returns>
        public virtual string[] SpellingSuggestions2(string word)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            List<string> suggestion = new List<string>();

            try
            {
                #region Homophone respelling suggestoins

                if (word.Length > MaxWordLengthToCheck)
                {
                    WordGenerator.SetAccuracy(0.1);
                }
                else
                {
                    WordGenerator.SetAccuracy(0.5);
                }

                suggestion.AddRange(ExtractRealWords(WordGenerator.GenerateHomophoneWords(word).ToArray()));

                #endregion

                int effectiveEditDistance = word.Length > MaxWordLengthToCheck ? 1 : this.EditDistance;

                if (word.Length >= 6)
                {
                    string initialSubSequence = word.Substring(0, 5);
                    string complementarySubSequence = word.Substring(5, word.Length - 5);

                    for (int i = 1; i <= effectiveEditDistance; ++i)
                    {
                        string[] initialPartVariation = WordGenerator.GenerateRespelling(initialSubSequence, i);
                        string[] secondSeq = new string[] {complementarySubSequence};
                        string[] reSpellingVariation = MultiplyStrings(initialPartVariation, secondSeq).ToArray();

                        suggestion.AddRange(ExtractRealWords(reSpellingVariation));
                        
                        if (suggestion.Count > 0)
                        {
                            break;
                        }
                    }

                    suggestion.AddRange(CompleteWord(initialSubSequence));
                }
                else
                {
                    for (int i = 1; i <= effectiveEditDistance; ++i)
                    {

                        suggestion.AddRange(GetAllsuggestion(word, i));

                        //if (suggestion.Count > SuggestionCount / 2)
                        if (suggestion.Count > 0)
                        {
                            break;
                        }
                    }
                }
               
                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get a list of correct respelling and homophone suggestions of an incorrect word by similarity
        /// </summary>
        /// <param name="word">Wrong Word</param>
        /// <returns>Sorted list of correct respelling suggestions</returns>
        public virtual string[] SpellingSuggestions3(string word, int editDistance)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            List<string> suggestion = new List<string>();

            try
            {
                if (editDistance == 1)
                {
                    #region Homophone respelling suggestoins

                    if (word.Length > MaxWordLengthToCheck)
                    {
                        WordGenerator.SetAccuracy(0.1);
                    }
                    else
                    {
                        WordGenerator.SetAccuracy(0.5);
                    }

                    suggestion.AddRange(ExtractRealWords(WordGenerator.GenerateHomophoneWords(word).ToArray()));

                    #endregion
                }

                suggestion.AddRange(WordGenerator.GenerateRespelling(word, editDistance));

                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Returns a sequence of strings gained from concatenating all the strings 
        /// in <paramref name="first"/> with all the strings in <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The sequence of strings to form the left-hand-side of the concatenations.</param>
        /// <param name="second">The sequence of strings to form the right-hand-side of the concatenations.</param>
        private static IEnumerable<string> MultiplyStrings(IEnumerable<string> first, IEnumerable<string> second)
        {
            List<string> result = new List<string>();

            foreach (string str1 in first)
            {
                foreach (string str2 in second)
                {
                    result.Add(str1 + str2);
                }
            }

            return result;
        }


        /// <summary>
        /// Get a list of correct respelling and homophone suggestions of an incorrect word by similarity
        /// </summary>
        /// <param name="word">Wrong Word</param>
        /// <param name="editDistance">Edit distance</param>
        /// <returns>Sorted list of correct respelling suggestions</returns>
        public virtual string[] SpellingSuggestions(string word, int editDistance)
        {
            if (!this.m_isInitialized)
            {
                throw new Exception("Speller Engine Must be Initialized!");
            }

            List<string> suggestion = new List<string>();

            try
            {

                #region Homophone respelling suggestoins

                WordGenerator.SetAccuracy(0.1);
                suggestion.AddRange(ExtractRealWords(WordGenerator.GenerateHomophoneWords(word).ToArray()));

                #endregion

                suggestion.AddRange(GetAllsuggestion(word, editDistance));

                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///<summary>
        /// Return word's frequency
        ///</summary>
        ///<param name="word">word</param>
        ///<returns></returns>
        public int WordFrequency(string word)
        {
            return m_wordContainer.WordFrequency(word);
        }

        ///<summary>
        /// Return word's POS tag
        ///</summary>
        ///<param name="word">word</param>
        ///<returns>POS tag</returns>
        public PersianPOSTag WordPOS(string word)
        {
            return m_wordContainer.WordPOS(word);
        }

        ///<summary>
        /// Save Loaded Dictionary to File
        ///</summary>
        ///<param name="fileName">File name</param>
        public void SaveDistionaryToFile(string fileName)
        {
            m_wordContainer.SaveDictionaryToFile(fileName);
        }

        #endregion

        #region Protected Methods

        protected double CalcAvgFreq()
        {
            if (m_wordContainer != null)
            {
                return (double) m_wordContainer.FreqSummation / m_wordContainer.DictionaryWordsCount;
            }

            return 0;
        }

        protected double CalcAvgFreq(double[] freqs)
        {
            if (freqs.Length == 1)
            {
                return freqs[0];
            }

            double mult;
            List<double> list = new List<double>();
            
            for (int i = 0; i < freqs.Length; i += 2)
            {
                if (i < freqs.Length - 1)
                {
                    mult = Math.Sqrt(freqs[i] * freqs[i + 1]);
                    if (double.IsNaN(mult))
                    {
                        mult = new double[] {freqs[i], freqs[i + 1]}.Average();
                    }
                    list.Add(mult);
                }
                else
                {
                    list.Add(freqs[i]);
                }
            }

            return CalcAvgFreq(list.ToArray());
        }

        protected double CalcAvgFreq(string[] words)
        {

            //List<double> list = new List<double>();
            //foreach(string str in words)
            //{
            //    list.Add(m_wordContainer.WordFrequency(str));
            //}

            //return CalcAvgFreq(list.ToArray());

            long avg;
            Dictionary<string, int> frq = CatchFrequencyWithAffixConsideration(words, out avg);

            return avg;
        }

        #endregion

    }
}

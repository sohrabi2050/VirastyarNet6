// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.BatchCharacterRefinement
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using SCICT.NLP.Persian;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCICT.VirastyarInlineVerifiers
{
    public class BatchCharacterRefinement
  {
    private const string LabelRefineLetter = "اصلاح حروف";
    private const string LabelRefineDigit = "اصلاح ارقام";
    private const string LabelRefineHalfSpace = "اصلاح نویسه\u200Cی نیم\u200Cفاصله";
    private const string LabelRefineErab = "اصلاح اعراب";
    private const string LabelRefineHalfSpacePosition = "حذف و جابجایی نیم\u200Cفاصله\u200Cها";
    private const string LabelConvertHeYe = "اصلاح «ه\u200Cی» یا «هٔ»";
    private readonly Dictionary<BatchCharacterRefinement.RefinementTypes, long> m_dicRefinementStats = new Dictionary<BatchCharacterRefinement.RefinementTypes, long>();

    public Dictionary<BatchCharacterRefinement.RefinementTypes, long> RefinementStats => this.m_dicRefinementStats;

    public BatchVerificationInstance PerformBatchCharacterRefinement(
      string text,
      IEnumerable<char> charsToIgnore,
      FilteringCharacterCategory notIgnoredCategories,
      bool refineHalfSpacePositioning,
      bool normalizeHeYe,
      bool shortHeYeToLong,
      bool longHeYeToShort)
    {
      this.ResetStats();
      string str1 = this.BatchCharacterRefinementBase(text, charsToIgnore, notIgnoredCategories, refineHalfSpacePositioning, normalizeHeYe, shortHeYeToLong, longHeYeToShort);
      string str2 = this.StatsticsMessage();
      return new BatchVerificationInstance()
      {
        Result = str1,
        Message = str2
      };
    }

    private string BatchCharacterRefinementBase(
      string text,
      IEnumerable<char> charsToIgnore,
      FilteringCharacterCategory notIgnoredCategories,
      bool refineHalfSpacePositioning,
      bool normalizeHeYe,
      bool shortHeYeToLong,
      bool longHeYeToShort)
    {
      HashSet<char> ignoreList = new HashSet<char>(charsToIgnore);
      FilteringCharacterCategory ignoreCats = notIgnoredCategories ^ (FilteringCharacterCategory.Kaaf | FilteringCharacterCategory.Yaa | FilteringCharacterCategory.HalfSpace | FilteringCharacterCategory.ArabicDigit | FilteringCharacterCategory.Erab);
      string str1 = text;
      if (refineHalfSpacePositioning)
      {
        int numChanges;
        str1 = StringUtil.NormalizeSpacesAndHalfSpacesInWord(str1, out numChanges);
        Dictionary<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStats;
        (dicRefinementStats = this.m_dicRefinementStats)[BatchCharacterRefinement.RefinementTypes.RefineHalfSpacePosition] = dicRefinementStats[BatchCharacterRefinement.RefinementTypes.RefineHalfSpacePosition] + (long) numChanges;
      }
      FilterResultsWithStats resultsWithStats = StringUtil.FilterPersianWordWithStats(str1, ignoreList, ignoreCats);
      if (str1 != resultsWithStats.Result)
      {
        str1 = resultsWithStats.Result;
        Dictionary<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStats1;
        (dicRefinementStats1 = this.m_dicRefinementStats)[BatchCharacterRefinement.RefinementTypes.RefineDigit] = dicRefinementStats1[BatchCharacterRefinement.RefinementTypes.RefineDigit] + (long) resultsWithStats.NumDigits;
        Dictionary<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStats2;
        (dicRefinementStats2 = this.m_dicRefinementStats)[BatchCharacterRefinement.RefinementTypes.RefineErab] = dicRefinementStats2[BatchCharacterRefinement.RefinementTypes.RefineErab] + (long) resultsWithStats.NumErabs;
        Dictionary<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStats3;
        (dicRefinementStats3 = this.m_dicRefinementStats)[BatchCharacterRefinement.RefinementTypes.RefineHalfSpace] = dicRefinementStats3[BatchCharacterRefinement.RefinementTypes.RefineHalfSpace] + (long) resultsWithStats.NumHalfSpaces;
        Dictionary<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStats4;
        (dicRefinementStats4 = this.m_dicRefinementStats)[BatchCharacterRefinement.RefinementTypes.RefineLetter] = dicRefinementStats4[BatchCharacterRefinement.RefinementTypes.RefineLetter] + (long) resultsWithStats.NumLetters;
      }
      if (longHeYeToShort || shortHeYeToLong || normalizeHeYe)
      {
        WordTokenInfo[] array = new WordTokenizer(WordTokenizerOptions.ReturnPunctuations | WordTokenizerOptions.ReturnWhitespaces).Tokenize(str1).ToArray<WordTokenInfo>();
        StringBuilder stringBuilder1 = new StringBuilder();
        for (int index1 = 0; index1 < array.Length; ++index1)
        {
          WordTokenInfo wordTokenInfo = array[index1];
          string word1 = (string) null;
          int num1 = -1;
          if (wordTokenInfo.Value.Length > 0 && char.IsLetter(wordTokenInfo.Value[0]))
          {
            for (int index2 = index1 + 1; index2 < array.Length; ++index2)
            {
              if (array[index2].Value.Length > 0)
              {
                char c = array[index2].Value[0];
                if (char.IsLetter(c))
                {
                  word1 = array[index2].Value;
                  num1 = index2;
                  break;
                }
                if (c == '\r' || c == '\n')
                  break;
              }
            }
            if (normalizeHeYe)
            {
              if (word1 != null && HeYe.IsTwoWordsFormingLongHeYe(wordTokenInfo.Value, word1))
              {
                int num2 = StringUtil.LastWordCharIndex(wordTokenInfo.Value);
                StringBuilder stringBuilder2 = new StringBuilder(wordTokenInfo.Value);
                stringBuilder2.Remove(num2, wordTokenInfo.Value.Length - num2);
                if (longHeYeToShort)
                  stringBuilder2.Insert(num2, HeYe.StandardShortHeYe);
                else
                  stringBuilder2.Insert(num2, HeYe.StandardLongHeYe);
                Dictionary<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStats;
                (dicRefinementStats = this.m_dicRefinementStats)[BatchCharacterRefinement.RefinementTypes.RefineHeYe] = dicRefinementStats[BatchCharacterRefinement.RefinementTypes.RefineHeYe] + 1L;
                stringBuilder1.Append(stringBuilder2.ToString());
                index1 = num1;
              }
              else
              {
                string str2 = !longHeYeToShort ? (!shortHeYeToLong ? StringUtil.NormalizeShortHeYe(StringUtil.NormalizeLongHeYe(wordTokenInfo.Value)) : StringUtil.ConvertShortHeYeToLong(StringUtil.NormalizeLongHeYe(wordTokenInfo.Value))) : StringUtil.ConvertLongHeYeToShort(StringUtil.NormalizeShortHeYe(wordTokenInfo.Value));
                if (str2 != wordTokenInfo.Value)
                {
                  Dictionary<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStats;
                  (dicRefinementStats = this.m_dicRefinementStats)[BatchCharacterRefinement.RefinementTypes.RefineHeYe] = dicRefinementStats[BatchCharacterRefinement.RefinementTypes.RefineHeYe] + 1L;
                }
                stringBuilder1.Append(str2);
              }
            }
            else if (longHeYeToShort)
            {
              if (word1 != null && HeYe.IsTwoWordsFormingLongHeYe(wordTokenInfo.Value, word1))
              {
                int num3 = StringUtil.LastWordCharIndex(wordTokenInfo.Value);
                StringBuilder stringBuilder3 = new StringBuilder(wordTokenInfo.Value);
                stringBuilder3.Remove(num3, wordTokenInfo.Value.Length - num3);
                stringBuilder3.Insert(num3, HeYe.StandardShortHeYe);
                Dictionary<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStats;
                (dicRefinementStats = this.m_dicRefinementStats)[BatchCharacterRefinement.RefinementTypes.RefineHeYe] = dicRefinementStats[BatchCharacterRefinement.RefinementTypes.RefineHeYe] + 1L;
                stringBuilder1.Append(stringBuilder3.ToString());
                index1 = num1;
              }
              else
              {
                string str3 = StringUtil.ConvertLongHeYeToShort(wordTokenInfo.Value);
                if (str3 != wordTokenInfo.Value)
                {
                  Dictionary<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStats;
                  (dicRefinementStats = this.m_dicRefinementStats)[BatchCharacterRefinement.RefinementTypes.RefineHeYe] = dicRefinementStats[BatchCharacterRefinement.RefinementTypes.RefineHeYe] + 1L;
                }
                stringBuilder1.Append(str3);
              }
            }
            else if (shortHeYeToLong)
            {
              string str4 = StringUtil.ConvertShortHeYeToLong(wordTokenInfo.Value);
              if (str4 != wordTokenInfo.Value)
              {
                Dictionary<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStats;
                (dicRefinementStats = this.m_dicRefinementStats)[BatchCharacterRefinement.RefinementTypes.RefineHeYe] = dicRefinementStats[BatchCharacterRefinement.RefinementTypes.RefineHeYe] + 1L;
              }
              stringBuilder1.Append(str4);
            }
          }
          else
            stringBuilder1.Append(wordTokenInfo.Value);
        }
        str1 = stringBuilder1.ToString();
      }
      return str1;
    }

    protected void ResetStats()
    {
      this.m_dicRefinementStats.Clear();
      foreach (BatchCharacterRefinement.RefinementTypes key in Enum.GetValues(typeof (BatchCharacterRefinement.RefinementTypes)))
        this.m_dicRefinementStats.Add(key, 0L);
    }

    private string StatsticsMessage()
    {
      StringBuilder stringBuilder = new StringBuilder();
      long num = 0;
      foreach (KeyValuePair<BatchCharacterRefinement.RefinementTypes, long> dicRefinementStat in this.m_dicRefinementStats)
      {
        num += dicRefinementStat.Value;
        stringBuilder.AppendLine(string.Format("{0}: {1}", (object) BatchCharacterRefinement.GetRefinementTypeLabel(dicRefinementStat.Key), (object) ParsingUtils.ConvertNumber2Persian(dicRefinementStat.Value.ToString())));
      }
      return string.Format("{0}: {1}{2}{2}", (object) "تعداد کل نویسه\u200Cهای اصلاح شده", (object) ParsingUtils.ConvertNumber2Persian(num.ToString()), (object) Environment.NewLine) + (object) stringBuilder;
    }

    private static string GetRefinementTypeLabel(
      BatchCharacterRefinement.RefinementTypes refinementType)
    {
      switch (refinementType)
      {
        case BatchCharacterRefinement.RefinementTypes.RefineLetter:
          return "اصلاح حروف";
        case BatchCharacterRefinement.RefinementTypes.RefineDigit:
          return "اصلاح ارقام";
        case BatchCharacterRefinement.RefinementTypes.RefineHalfSpace:
          return "اصلاح نویسه\u200Cی نیم\u200Cفاصله";
        case BatchCharacterRefinement.RefinementTypes.RefineErab:
          return "اصلاح اعراب";
        case BatchCharacterRefinement.RefinementTypes.RefineHalfSpacePosition:
          return "حذف و جابجایی نیم\u200Cفاصله\u200Cها";
        case BatchCharacterRefinement.RefinementTypes.RefineHeYe:
          return "اصلاح «ه\u200Cی» یا «هٔ»";
        default:
          return "";
      }
    }

    public enum RefinementTypes
    {
      RefineLetter,
      RefineDigit,
      RefineHalfSpace,
      RefineErab,
      RefineHalfSpacePosition,
      RefineHeYe,
    }
  }
}

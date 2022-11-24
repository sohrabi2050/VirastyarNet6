// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.SpellCheckerInlineVerifier
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using SCICT.NLP;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.NLP.Utility;
using SCICT.Utility.SpellChecker;
using System;
using System.IO;
using System.Text;

namespace SCICT.VirastyarInlineVerifiers
{
  public class SpellCheckerInlineVerifier : NGramVerifierBase
  {
    private readonly PersianSpellChecker m_engine;
    private readonly SessionLogger m_sessionLogger;
    private SpaceCorrectionState m_lastSCS;
    private string[] m_lastSugs;
    private string m_curWordCombToCheck;
    private string m_curWord0;
    private string m_curWord1;
    private string m_curWord2;
    private readonly bool m_isPrespellChecker;
    private readonly bool m_doLog;
    private readonly string m_logFileName;
    private readonly StreamWriter m_logStream;

    public SpellCheckerInlineVerifier(
      bool isPrespellChecker,
      PersianSpellChecker engine,
      SessionLogger sessionLogger,
      string logFileName)
      : base(1, 1)
    {
      this.m_engine = engine;
      this.m_sessionLogger = sessionLogger;
      this.m_isPrespellChecker = isPrespellChecker;
      if (string.IsNullOrEmpty(logFileName))
        return;
      this.m_logFileName = logFileName;
      try
      {
        this.m_logStream = File.CreateText(this.m_logFileName);
        this.m_logStream.AutoFlush = true;
        this.m_doLog = true;
      }
      catch
      {
        this.m_doLog = false;
        throw;
      }
    }

    ~SpellCheckerInlineVerifier()
    {
      try
      {
        if (this.m_logStream == null)
          return;
        this.m_logStream.Close();
      }
      catch
      {
      }
      finally
      {
        // ISSUE: explicit finalizer call
        //base.Finalize();
      }
    }

    public SpellCheckerInlineVerifier(
      bool isPrespellChecker,
      PersianSpellChecker engine,
      SessionLogger sessionLogger)
      : base(1, 1)
    {
      this.m_engine = engine;
      this.m_sessionLogger = sessionLogger;
      this.m_isPrespellChecker = isPrespellChecker;
    }

    public SpellCheckerInlineVerifier(bool isPrespellChecker, PersianSpellChecker engine)
      : this(isPrespellChecker, engine, new SessionLogger())
    {
    }

    protected override VerificationInstance CheckNGramWordsList(
      TokenInfo[] readItems,
      int mainItemIndex)
    {
      if (string.IsNullOrEmpty(readItems[mainItemIndex].Value))
        return (VerificationInstance) null;
      this.m_curWord1 = readItems[mainItemIndex].Value;
      this.m_curWord0 = mainItemIndex > 0 ? readItems[mainItemIndex - 1].Value : "";
      this.m_curWord2 = mainItemIndex < readItems.Length - 1 ? readItems[mainItemIndex + 1].Value : "";
      SuggestionType suggestionType;
      SpaceCorrectionState spaceCorrectionState;
      try
      {
        string[] suggestions;
        if (this.m_isPrespellChecker)
        {
          if (this.m_engine.OnePassCorrection(this.m_curWord1, this.m_curWord0, this.m_curWord2, this.m_engine.SuggestionCount, out suggestions, out suggestionType, out spaceCorrectionState))
            return (VerificationInstance) null;
        }
        else
        {
          if (this.m_doLog)
            this.m_logStream.WriteLine("{0,-15}  {1,-15}  {2,-15}", (object) this.m_curWord0, (object) this.m_curWord1, (object) this.m_curWord2);
          if (this.m_engine.CheckSpelling(this.m_curWord1, this.m_curWord0, this.m_curWord2, this.m_engine.SuggestionCount, out suggestions, out suggestionType, out spaceCorrectionState))
          {
            if (this.m_doLog)
            {
              this.m_logStream.WriteLine("{}");
              this.m_logStream.WriteLine("-------------------------------");
            }
            return (VerificationInstance) null;
          }
          if (this.m_doLog)
          {
            StringBuilder stringBuilder = new StringBuilder("{");
            foreach (string str in suggestions)
              stringBuilder.Append(str).Append("  ");
            stringBuilder.Append("}");
            this.m_logStream.WriteLine(stringBuilder.ToString());
            this.m_logStream.WriteLine("-------------------------------");
          }
        }
        if (!this.m_isPrespellChecker)
          suggestions = this.m_sessionLogger.Sort(suggestions);
        this.m_lastSugs = suggestions;
        this.m_lastSCS = spaceCorrectionState;
      }
      catch (Exception ex)
      {
        return (VerificationInstance) null;
      }
      VerificationTypes verTypes = suggestionType != SuggestionType.Green ? VerificationTypes.Error : VerificationTypes.Warning;
      this.m_curWordCombToCheck = this.m_curWord1;
      int index1;
      int index2;
      switch (spaceCorrectionState)
      {
        case SpaceCorrectionState.SpaceInsertationLeft:
        case SpaceCorrectionState.SpaceInsertationLeftSerrially:
          this.m_curWordCombToCheck = this.m_curWord1 + (object) ' ' + this.m_curWord2;
          index1 = mainItemIndex;
          index2 = mainItemIndex + 1;
          break;
        case SpaceCorrectionState.SpaceInsertationRight:
        case SpaceCorrectionState.SpaceInsertationRightSerrially:
          this.m_curWordCombToCheck = this.m_curWord0 + (object) ' ' + this.m_curWord1;
          index1 = mainItemIndex - 1;
          index2 = mainItemIndex;
          break;
        default:
          index1 = mainItemIndex;
          index2 = mainItemIndex;
          break;
      }
      return new VerificationInstance(readItems[index1].Index, readItems[index2].EndIndex - readItems[index1].Index + 1, verTypes, this.m_lastSugs);
    }

    protected override WordTokenizerOptions TokenizerOptions => WordTokenizerOptions.ReturnPunctuations;

    protected override bool IsProperWord(string word) => !string.IsNullOrEmpty(word) && StringUtil.IsArabicWord(word.Trim()) && !StringUtil.IsWhiteSpace(word) && !StringUtil.IsHalfSpace(word);

    protected override bool NeedRefinedStrings => true;
  }
}

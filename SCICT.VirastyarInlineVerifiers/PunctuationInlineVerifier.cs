// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.PunctuationInlineVerifier
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using SCICT.NLP;
using SCICT.NLP.TextProofing.Punctuation;
using SCICT.NLP.Utility;
using System.Collections.Generic;
using System.Text;

namespace SCICT.VirastyarInlineVerifiers
{
  public class PunctuationInlineVerifier : StateMachineVerifierBase
  {
    private readonly PunctuationCheckerEngine m_punctuationCheckerEngine;
    private string[] m_lastSugs;
    private HashSet<PunctuationInlineVerifier.HistoryItem> m_history;

    public PunctuationInlineVerifier(PunctuationCheckerEngine engine) => this.m_punctuationCheckerEngine = engine;

    protected override bool InitParagraph(string par)
    {
      this.m_punctuationCheckerEngine.InitInputString(par);
      return true;
    }

    protected override VerificationInstance FindNextPattern()
    {
      this.m_punctuationCheckerEngine.FindMistake();
      if (!this.m_punctuationCheckerEngine.IsErrorFound())
        return (VerificationInstance) null;
      int mistakeIndex = this.m_punctuationCheckerEngine.GetMistakeIndex();
      int mistakeLength = this.m_punctuationCheckerEngine.GetMistakeLength();
      this.m_lastSugs = this.m_punctuationCheckerEngine.GetMultiSubstitutionString();
      this.m_punctuationCheckerEngine.SkipMistake();
      return new VerificationInstance(mistakeIndex, mistakeLength, VerificationTypes.Error, this.m_lastSugs);
    }

    protected override bool NeedRefinedStrings => true;

    public string BatchConvert(string text) => this.BatchConvert(text, out int _);

    public string BatchConvert(string text, out int numChanges)
    {
      StringBuilder stringBuilder = new StringBuilder();
      numChanges = 0;
      foreach (TokenInfo paragraph in StringUtil.ExtractParagraphs(text, true))
      {
        if (paragraph.Value.Contains("\n") || paragraph.Value.Contains("\r"))
        {
          stringBuilder.Append(paragraph.Value);
        }
        else
        {
          int numChanges1;
          stringBuilder.Append(this.BatchConvertPar(paragraph.Value, out numChanges1));
          numChanges += numChanges1;
        }
      }
      return stringBuilder.ToString();
    }

    public string BatchConvertPar(string text, out int numChanges)
    {
      if (this.m_history == null)
        this.m_history = new HashSet<PunctuationInlineVerifier.HistoryItem>();
      else
        this.m_history.Clear();
      numChanges = 0;
      this.m_punctuationCheckerEngine.InitInputString(text);
      this.m_punctuationCheckerEngine.FindMistake();
      while (this.m_punctuationCheckerEngine.IsErrorFound())
      {
        int mistakeIndex = this.m_punctuationCheckerEngine.GetMistakeIndex();
        int mistakeLength = this.m_punctuationCheckerEngine.GetMistakeLength();
        if (this.m_history.Contains(new PunctuationInlineVerifier.HistoryItem()
        {
          Context = this.m_punctuationCheckerEngine.GetCorrectedString(),
          Index = mistakeIndex,
          Length = mistakeLength
        }))
        {
          this.m_punctuationCheckerEngine.SkipMistake();
        }
        else
        {
          this.m_punctuationCheckerEngine.CorrectMistake(0);
          ++numChanges;
        }
        this.m_punctuationCheckerEngine.FindMistake();
      }
      return this.m_punctuationCheckerEngine.GetCorrectedString();
    }

    private class HistoryItem
    {
      public int Index;
      public int Length;
      public string Context;

      public override bool Equals(object obj)
      {
        if (object.ReferenceEquals(obj, (object) this))
          return true;
        return obj is PunctuationInlineVerifier.HistoryItem historyItem && this.Index == historyItem.Index && this.Length == historyItem.Length && this.Context == historyItem.Context;
      }

      public override int GetHashCode() => this.Index.GetHashCode() + this.Length.GetHashCode() + this.Context.GetHashCode();
    }
  }
}

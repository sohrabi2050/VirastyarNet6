// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.NGramVerifierBase
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using SCICT.NLP;
using SCICT.NLP.Utility;
using SCICT.Utility;
using System.Collections.Generic;

namespace SCICT.VirastyarInlineVerifiers
{
  public abstract class NGramVerifierBase : InlineVerifierBase
  {
    protected int m_numWordsBefore;
    protected int m_numWordsAfter;
    protected WordTokenizer m_wordTokenizer;
    private readonly RoundQueue<TokenInfo> m_roundq;

    protected NGramVerifierBase(int numWordsBefore, int numWordsAfter)
    {
      this.m_numWordsAfter = numWordsAfter;
      this.m_numWordsBefore = numWordsBefore;
      this.m_roundq = new RoundQueue<TokenInfo>(this.m_numWordsBefore, this.m_numWordsAfter);
    }

    protected abstract VerificationInstance CheckNGramWordsList(
      TokenInfo[] readItems,
      int mainItemIndex);

    protected abstract WordTokenizerOptions TokenizerOptions { get; }

    protected abstract bool IsProperWord(string word);

    public override List<VerificationInstance> VerifyParagraph(string par)
    {
      List<VerificationInstance> verificationInstanceList = new List<VerificationInstance>();
      if (this.m_wordTokenizer == null)
        this.m_wordTokenizer = new WordTokenizer(this.TokenizerOptions);
      this.m_roundq.Clear();
      TokenInfo[] items;
      int mainItemIndex;
      foreach (WordTokenInfo wordTokenInfo in this.m_wordTokenizer.Tokenize(par))
      {
        if (!this.IsProperWord(wordTokenInfo.Value))
        {
          this.m_roundq.BlockEntry();
          while (this.m_roundq.ReadNextWordLists(out items, out mainItemIndex))
          {
            VerificationInstance verificationInstance = this.CheckNGramWordsList(items, mainItemIndex);
            if (verificationInstance != null && verificationInstance.IsValid)
              verificationInstanceList.Add(verificationInstance);
          }
        }
        else
        {
          this.m_roundq.AddItem((TokenInfo) wordTokenInfo);
          if (this.m_roundq.ReadNextWordLists(out items, out mainItemIndex))
          {
            VerificationInstance verificationInstance = this.CheckNGramWordsList(items, mainItemIndex);
            if (verificationInstance != null && verificationInstance.IsValid)
              verificationInstanceList.Add(verificationInstance);
          }
        }
      }
      this.m_roundq.BlockEntry();
      while (this.m_roundq.ReadNextWordLists(out items, out mainItemIndex))
      {
        VerificationInstance verificationInstance = this.CheckNGramWordsList(items, mainItemIndex);
        if (verificationInstance != null && verificationInstance.IsValid)
          verificationInstanceList.Add(verificationInstance);
      }
      return verificationInstanceList;
    }
  }
}

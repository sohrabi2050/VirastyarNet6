// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.InlinePinglishVerifier
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using SCICT.NLP;
using SCICT.NLP.Utility;

namespace SCICT.VirastyarInlineVerifiers
{
  public class InlinePinglishVerifier : NGramVerifierBase
  {
    private readonly SCICT.NLP.Utility.PinglishConverter.PinglishConverter m_pinglishConverter;

    public InlinePinglishVerifier(SCICT.NLP.Utility.PinglishConverter.PinglishConverter pinglishConverter)
      : base(0, 0)
    {
      this.m_pinglishConverter = pinglishConverter;
    }

    protected override WordTokenizerOptions TokenizerOptions => WordTokenizerOptions.TreatNumberNonArabicCharCombinationAsOneWords;

    protected override bool IsProperWord(string word) => StringUtil.IsPinglishWord(word);

    protected override bool NeedRefinedStrings => false;

    protected override VerificationInstance CheckNGramWordsList(
      TokenInfo[] readItems,
      int mainItemIndex)
    {
      string str = readItems[mainItemIndex].Value;
      string[] sugs = new string[0];
      try
      {
        sugs = this.m_pinglishConverter.SuggestFarsiWords(str, true);
      }
      catch
      {
      }
      return sugs.Length == 0 ? (VerificationInstance) null : new VerificationInstance(readItems[mainItemIndex].Index, readItems[mainItemIndex].Length, VerificationTypes.Information, sugs);
    }
  }
}

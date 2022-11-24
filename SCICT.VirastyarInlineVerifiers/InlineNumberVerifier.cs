// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.InlineNumberVerifier
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using SCICT.NLP.Utility.Parsers;
using SCICT.NLP.Utility.PersianParsers;
using System.Collections.Generic;

namespace SCICT.VirastyarInlineVerifiers
{
    public class InlineNumberVerifier : ShrinkingVerifierBase
  {
    private readonly PersianRealNumberParser m_perianRealNumberParser = new PersianRealNumberParser();
    private readonly DigitizedNumberParser m_digitizedNumberParser = new DigitizedNumberParser();
    private List<IPatternInfo> m_lstPatterns;

    protected override bool NeedRefinedStrings => true;

    protected override VerificationInstance FindPattern(string content)
    {
      this.m_lstPatterns = this.FindNumberPatterns(content);
      if (this.m_lstPatterns == null || this.m_lstPatterns.Count <= 0)
        return (VerificationInstance) null;
      IPatternInfo lstPattern1 = this.m_lstPatterns[0];
      List<string> stringList = new List<string>();
      foreach (IPatternInfo lstPattern2 in this.m_lstPatterns)
        stringList.AddRange((IEnumerable<string>) InlineNumberVerifier.CreateSuggestions(lstPattern2));
      return new VerificationInstance(lstPattern1.Index, lstPattern1.Length, VerificationTypes.Information, stringList.ToArray());
    }

    private List<IPatternInfo> FindNumberPatterns(string content)
    {
      List<IPatternInfo> numberPatterns = new List<IPatternInfo>();
      numberPatterns.Clear();
      IPatternInfo patternInfo1 = (IPatternInfo) null;
      foreach (IPatternInfo patternInfo2 in this.m_perianRealNumberParser.FindAndParse(content))
      {
        if (patternInfo1 == null)
        {
          patternInfo1 = patternInfo2;
          numberPatterns.Add(patternInfo2);
        }
        else if (patternInfo2.Index == patternInfo1.Index)
        {
          if (patternInfo2.Length == patternInfo1.Length)
            numberPatterns.Add(patternInfo2);
          else
            break;
        }
        else
          break;
      }
      using (IEnumerator<DigitizedNumberPatternInfo> enumerator = this.m_digitizedNumberParser.FindAndParse(content).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          IPatternInfo current = (IPatternInfo) enumerator.Current;
          if (patternInfo1 != null)
          {
            if (current.Index >= patternInfo1.Index)
            {
              if (current.Index == patternInfo1.Index)
              {
                if (current.Length <= patternInfo1.Length)
                  goto label_22;
              }
              else
                goto label_22;
            }
            numberPatterns.Clear();
            numberPatterns.Add(current);
          }
          else
            numberPatterns.Add(current);
        }
      }
label_22:
      return numberPatterns;
    }

    private static string[] CreateSuggestions(IPatternInfo minPi) => NumberParsersSuggestions.CreateSuggestions(minPi);
  }
}

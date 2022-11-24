// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.InlineVerifierBase
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using SCICT.NLP;
using SCICT.NLP.Utility;
using System.Collections.Generic;

namespace SCICT.VirastyarInlineVerifiers
{
  public abstract class InlineVerifierBase
  {
    public IEnumerable<VerificationInstance> VerifyText(string text)
    {
      foreach (TokenInfo parInfo in StringUtil.ExtractParagraphs(text))
      {
        foreach (VerificationInstance patInfo in this.VerifyParagraphBase(parInfo.Value, parInfo.Index))
          yield return patInfo;
      }
    }

    protected abstract bool NeedRefinedStrings { get; }

    private List<VerificationInstance> VerifyParagraphBase(
      string par,
      int parStartIndex)
    {
      List<VerificationInstance> verificationInstanceList = new List<VerificationInstance>();
      string str = par;
      string par1 = str;
      if (this.NeedRefinedStrings)
        par1 = StringUtil.RefineAndFilterPersianWord(str);
      if (string.IsNullOrEmpty(par1))
        return verificationInstanceList;
      foreach (VerificationInstance verificationInstance in this.VerifyParagraph(par1))
      {
        if (verificationInstance != null)
        {
          if (verificationInstance.IsValid)
          {
            if (this.NeedRefinedStrings)
            {
              int num1 = StringUtil.IndexInNotFilterAndRefinedString(str, verificationInstance.Index);
              int num2 = StringUtil.IndexInNotFilterAndRefinedString(str, verificationInstance.EndIndex);
              verificationInstance.Index = num1;
              verificationInstance.Length = num2 - num1 + 1;
            }
            verificationInstance.Index += parStartIndex;
            verificationInstanceList.Add(verificationInstance);
          }
          else
            break;
        }
        else
          break;
      }
      return verificationInstanceList;
    }

    public abstract List<VerificationInstance> VerifyParagraph(string par);
  }
}

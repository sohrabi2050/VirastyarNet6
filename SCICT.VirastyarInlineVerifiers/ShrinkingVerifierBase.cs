// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.ShrinkingVerifierBase
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using System.Collections.Generic;

namespace SCICT.VirastyarInlineVerifiers
{
  public abstract class ShrinkingVerifierBase : InlineVerifierBase
  {
    /// <summary>
    /// Finds the first and the most prominent pattern in the string.
    /// </summary>
    /// <param name="content">The content to search the pattern in.</param>
    protected abstract VerificationInstance FindPattern(string content);

    public override List<VerificationInstance> VerifyParagraph(string par)
    {
      List<VerificationInstance> verificationInstanceList = new List<VerificationInstance>();
      int startIndex = 0;
      int num = 0;
      string content = par;
      while (!string.IsNullOrEmpty(content) && startIndex < content.Length)
      {
        if (startIndex > 0)
        {
          content = content.Substring(startIndex);
          if (string.IsNullOrEmpty(content))
            break;
        }
        VerificationInstance pattern = this.FindPattern(content);
        if (pattern != null && pattern.IsValid)
        {
          startIndex = pattern.EndIndex + 1;
          pattern.Index += num;
          num += startIndex;
          verificationInstanceList.Add(pattern);
        }
        else
          break;
      }
      return verificationInstanceList;
    }
  }
}

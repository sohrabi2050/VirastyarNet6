// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.StateMachineVerifierBase
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using System.Collections.Generic;

namespace SCICT.VirastyarInlineVerifiers
{
  public abstract class StateMachineVerifierBase : InlineVerifierBase
  {
    public override List<VerificationInstance> VerifyParagraph(string par)
    {
      List<VerificationInstance> verificationInstanceList = new List<VerificationInstance>();
      if (!this.InitParagraph(par))
        return verificationInstanceList;
      VerificationInstance nextPattern;
      while ((nextPattern = this.FindNextPattern()) != null && nextPattern.IsValid)
        verificationInstanceList.Add(nextPattern);
      return verificationInstanceList;
    }

    protected abstract bool InitParagraph(string par);

    protected abstract VerificationInstance FindNextPattern();
  }
}

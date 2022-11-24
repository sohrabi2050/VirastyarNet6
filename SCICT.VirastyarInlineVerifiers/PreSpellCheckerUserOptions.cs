// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.PreSpellCheckerUserOptions
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using System;

namespace SCICT.VirastyarInlineVerifiers
{
  [Flags]
  public enum PreSpellCheckerUserOptions
  {
    None = 0,
    RefineBe = 1,
    RefinePrefix = 2,
    RefineSuffix = 4,
  }
}

// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.EditedInterval
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

namespace SCICT.VirastyarInlineVerifiers
{
  public struct EditedInterval
  {
    public int Index { get; set; }

    public int Length { get; set; }

    public int EndIndex => this.Index + this.Length - 1;

    public bool IsValid => this.Index >= 0 && this.Length > 0;

    public bool IsInConflict(EditedInterval other) => other.Index <= this.EndIndex && this.Index <= other.EndIndex;
  }
}

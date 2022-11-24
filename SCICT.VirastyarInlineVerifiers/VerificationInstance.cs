// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.VerificationInstance
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using System.Text;

namespace SCICT.VirastyarInlineVerifiers
{
  public class VerificationInstance
  {
    public VerificationInstance(int index, int length, VerificationTypes verTypes, string[] sugs)
      : this(index, length, verTypes, sugs, (string) null, (string[]) null)
    {
    }

    public VerificationInstance(
      int index,
      int length,
      VerificationTypes verTypes,
      string[] sugs,
      string message)
      : this(index, length, verTypes, sugs, message, (string[]) null)
    {
    }

    public VerificationInstance(
      int index,
      int length,
      VerificationTypes verTypes,
      string[] sugs,
      string[] groups)
      : this(index, length, verTypes, sugs, (string) null, groups)
    {
    }

    public VerificationInstance(
      int index,
      int length,
      VerificationTypes verTypes,
      string[] sugs,
      string message,
      string[] groups)
    {
      this.Index = index;
      this.Length = length;
      this.VerificationType = verTypes;
      this.Suggestions = sugs;
      this.Message = message;
      if (groups == null || sugs == null || groups.Length != sugs.Length)
        return;
      this.IsGrouped = true;
      this.GroupTitles = groups;
    }

    public int Index { get; set; }

    public int Length { get; set; }

    public VerificationTypes VerificationType { get; private set; }

    public string Message { get; private set; }

    public string[] Suggestions { get; private set; }

    public bool IsGrouped { get; private set; }

    public string[] GroupTitles { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format("{0} - (i: {1} - l:{2})", (object) this.VerificationType, (object) this.Index, (object) this.Length));
      if (this.Suggestions != null && this.Suggestions.Length != 0)
      {
        if (this.IsGrouped)
        {
          for (int index = 0; index < this.Suggestions.Length; ++index)
            stringBuilder.AppendLine(string.Format("{0}   ({1})", (object) this.Suggestions[index], (object) this.GroupTitles[index]));
        }
        else
        {
          for (int index = 0; index < this.Suggestions.Length; ++index)
            stringBuilder.AppendLine(string.Format("{0}", (object) this.Suggestions[index]));
        }
      }
      stringBuilder.AppendLine("-----------------------");
      return stringBuilder.ToString();
    }

    public bool IsValid => this.Index >= 0 && this.Length > 0;

    public int EndIndex => this.Index + this.Length - 1;

    public bool HasMessage => !string.IsNullOrEmpty(this.Message);
  }
}

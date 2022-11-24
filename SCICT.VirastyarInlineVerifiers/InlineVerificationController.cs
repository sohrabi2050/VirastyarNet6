// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.InlineVerificationController
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using SCICT.NLP.Persian;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.NLP.Utility.Parsers;
using SCICT.Utility.SpellChecker;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCICT.VirastyarInlineVerifiers
{
  public class InlineVerificationController
  {
    private readonly VerificationEngines m_verificationEngines;

    public InlineVerificationController(VerificationEngines vengines) => this.m_verificationEngines = vengines;

    public IEnumerable<VerificationInstance> InlineSpellCheckAndLog(
      string text,
      SpellCheckerUserOptions options,
      string logFileName)
    {
      return new SpellCheckerInlineVerifier(false, this.m_verificationEngines.GetSpellCheckerEngine(), new SessionLogger(), logFileName).VerifyText(text);
    }

    public IEnumerable<VerificationInstance> InlineSpellCheck(
      string text)
    {
      return new SpellCheckerInlineVerifier(false, this.m_verificationEngines.GetSpellCheckerEngine()).VerifyText(text);
    }

    public IEnumerable<VerificationInstance> InlineSpellCheck(
      string text,
      SpellingRules rules)
    {
      PersianSpellChecker spellCheckerEngine = this.m_verificationEngines.GetSpellCheckerEngine();
      spellCheckerEngine.UnsetSpellingRules((SpellingRules) 255);
      spellCheckerEngine.SetSpellingRules(rules);
      return new SpellCheckerInlineVerifier(false, spellCheckerEngine).VerifyText(text);
    }

    public VerificationInstance[] RemoveConflicts(
      IEnumerable<VerificationInstance> verifs)
    {
      List<VerificationInstance> verificationInstanceList = new List<VerificationInstance>();
      EditedInterval editedInterval = new EditedInterval()
      {
        Index = -1,
        Length = 0
      };
      foreach (VerificationInstance verif in verifs)
      {
        EditedInterval other = new EditedInterval()
        {
          Index = verif.Index,
          Length = verif.Length
        };
        if (other.IsValid && (!editedInterval.IsValid || !editedInterval.IsInConflict(other)))
        {
          verificationInstanceList.Add(verif);
          editedInterval = other;
        }
      }
      return verificationInstanceList.ToArray();
    }

    public string ApplyAll(string text, IEnumerable<VerificationInstance> verifs) => this.ApplyAll(text, verifs, out int _);

    public string ApplyAll(
      string text,
      IEnumerable<VerificationInstance> verifs,
      out int numChanges)
    {
      VerificationInstance[] array = ((IEnumerable<VerificationInstance>) this.RemoveConflicts(verifs)).ToArray<VerificationInstance>();
      StringBuilder stringBuilder = new StringBuilder(text);
      numChanges = 0;
      for (int index = array.Length - 1; index >= 0; --index)
      {
        VerificationInstance verificationInstance = array[index];
        if (verificationInstance.Suggestions.Length > 0)
        {
          stringBuilder.Remove(verificationInstance.Index, verificationInstance.Length);
          stringBuilder.Insert(verificationInstance.Index, verificationInstance.Suggestions[0]);
          ++numChanges;
        }
      }
      return stringBuilder.ToString();
    }

    public BatchVerificationInstance BatchPreSpellCheck(
      string text,
      PreSpellCheckerUserOptions options)
    {
      SpellCheckerInlineVerifier checkerInlineVerifier = new SpellCheckerInlineVerifier(true, this.m_verificationEngines.GetPreSpellCheckerEngine(options));
      int numChanges;
      string str1 = this.ApplyAll(text, checkerInlineVerifier.VerifyText(text), out numChanges);
      string str2 = string.Format("تعداد اصلاحات انجام شده: {0}", (object) ParsingUtils.ConvertNumber2Persian(numChanges.ToString()));
      return new BatchVerificationInstance()
      {
        Message = str2,
        Result = str1
      };
    }

    public IEnumerable<VerificationInstance> InlinePunctuationCheck(
      string text)
    {
      return new PunctuationInlineVerifier(this.m_verificationEngines.GetPunctuationEngine()).VerifyText(text);
    }

    public BatchVerificationInstance BatchPunctuationCheck(string text)
    {
      int numChanges;
      string str1 = new PunctuationInlineVerifier(this.m_verificationEngines.GetPunctuationEngine()).BatchConvert(text, out numChanges);
      string str2 = string.Format("تعداد اصلاحات انجام شده: {0}", (object) ParsingUtils.ConvertNumber2Persian(numChanges.ToString()));
      return new BatchVerificationInstance()
      {
        Message = str2,
        Result = str1
      };
    }

    public IEnumerable<VerificationInstance> InlineNumberCheck(
      string text)
    {
      return new InlineNumberVerifier().VerifyText(text);
    }

    public IEnumerable<VerificationInstance> InlineDateCheck(
      string text)
    {
      return new InlineDateVerifier().VerifyText(text);
    }

    public IEnumerable<VerificationInstance> InlinePinglishCheck(
      string text)
    {
      return new InlinePinglishVerifier(this.m_verificationEngines.GetPinglishEngine()).VerifyText(text);
    }

    public BatchVerificationInstance BatchPinglishCheck(string text)
    {
      InlinePinglishVerifier pinglishVerifier = new InlinePinglishVerifier(this.m_verificationEngines.GetPinglishEngine());
      int numChanges;
      string str1 = this.ApplyAll(text, pinglishVerifier.VerifyText(text), out numChanges);
      string str2 = string.Format("تعداد اصلاحات انجام شده: {0}", (object) ParsingUtils.ConvertNumber2Persian(numChanges.ToString()));
      return new BatchVerificationInstance()
      {
        Message = str2,
        Result = str1
      };
    }

    public BatchVerificationInstance BatchCharacterRefinement(
      string text,
      IEnumerable<char> charsToIgnore,
      FilteringCharacterCategory notIgnoredCategories,
      bool refineHalfSpacePositioning,
      bool normalizeHeYe,
      bool shortHeYeToLong,
      bool longHeYeToShort)
    {
      return new SCICT.VirastyarInlineVerifiers.BatchCharacterRefinement().PerformBatchCharacterRefinement(text, charsToIgnore, notIgnoredCategories, refineHalfSpacePositioning, normalizeHeYe, shortHeYeToLong, longHeYeToShort);
    }
  }
}

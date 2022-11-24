// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.VerificationEngines
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using SCICT.NLP.TextProofing.Punctuation;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.NLP.Utility.PinglishConverter;
using SCICT.Utility.Extensions;
using System;

namespace SCICT.VirastyarInlineVerifiers
{
  public class VerificationEngines
  {
    private readonly string m_dicPath;
    private readonly string m_stemPath;
    private readonly int m_editDistance = 2;
    private readonly int m_numSuggestions = 7;
    private readonly string m_puncFile;
    private readonly string m_pinglishFile;
    private readonly string m_pinglishPreprocessFile;
    private PersianSpellChecker m_spellCheckerEngine;
    private readonly PersianSpellChecker[] m_preSpellCheckerEngines;
    private PunctuationCheckerEngine m_punctuationEngine;
    private SCICT.NLP.Utility.PinglishConverter.PinglishConverter m_pinglishConverter;
    private readonly object m_objLock = new object();

    public VerificationEngines(
      string dicPath,
      string stemPath,
      string puncPath,
      string pingPath,
      string pingPrepPath)
      : this(dicPath, stemPath, puncPath, pingPath, pingPrepPath, 2, 7)
    {
    }

    public VerificationEngines(
      string dicPath,
      string stemPath,
      string puncPath,
      string pingPath,
      string pingPrepPath,
      int editDistance,
      int numSuggs)
    {
      this.m_dicPath = dicPath;
      this.m_stemPath = stemPath;
      this.m_puncFile = puncPath;
      this.m_pinglishFile = pingPath;
      this.m_pinglishPreprocessFile = pingPrepPath;
      this.m_editDistance = editDistance;
      this.m_numSuggestions = numSuggs;
      this.m_preSpellCheckerEngines = new PersianSpellChecker[VerificationEngines.NumPreSpellCheckerEngines()];
    }

    public SCICT.NLP.Utility.PinglishConverter.PinglishConverter GetPinglishEngine()
    {
      if (this.m_pinglishConverter != null)
        return this.m_pinglishConverter;
      this.m_pinglishConverter = new SCICT.NLP.Utility.PinglishConverter.PinglishConverter();
      ((IPinglishLearner) this.m_pinglishConverter).Learn(PinglishConverterUtils.LoadPinglishStrings(this.m_pinglishFile));
      this.m_pinglishConverter.LoadPreprocessElements(this.m_pinglishPreprocessFile);
      this.m_pinglishConverter.SetSpellerEngine(new SpellCheckerEngine(new SpellCheckerConfig()
      {
        DicPath = this.m_dicPath,
        StemPath = this.m_stemPath
      }));
      return this.m_pinglishConverter;
    }

    private static int NumPreSpellCheckerEngines() => (int) Math.Pow(2.0, (double) (Enum.GetValues(typeof (PreSpellCheckerUserOptions)).Length - 1));

    private static OnePassCorrectionRules ToEnginePreSpellOptions(
      PreSpellCheckerUserOptions userOptions)
    {
      PreSpellCheckerUserOptions[] checkerUserOptionsArray = new PreSpellCheckerUserOptions[3]
      {
        PreSpellCheckerUserOptions.RefineBe,
        PreSpellCheckerUserOptions.RefinePrefix,
        PreSpellCheckerUserOptions.RefineSuffix
      };
      OnePassCorrectionRules[] passCorrectionRulesArray = new OnePassCorrectionRules[3]
      {
        OnePassCorrectionRules.CorrectBe,
        OnePassCorrectionRules.CorrectPrefix,
        OnePassCorrectionRules.CorrectSuffix
      };
      OnePassCorrectionRules enginePreSpellOptions = OnePassCorrectionRules.None;
      bool flag = false;
      for (int index = 0; index < checkerUserOptionsArray.Length; ++index)
      {
        PreSpellCheckerUserOptions checkerUserOptions = checkerUserOptionsArray[index];
        if (userOptions.Has<PreSpellCheckerUserOptions>(checkerUserOptions))
        {
          enginePreSpellOptions = flag ? enginePreSpellOptions | passCorrectionRulesArray[index] : passCorrectionRulesArray[index];
          flag = true;
        }
      }
      return enginePreSpellOptions;
    }

    public PersianSpellChecker GetSpellCheckerEngine()
    {
      lock (this.m_objLock)
      {
        if (this.m_spellCheckerEngine != null)
          return this.m_spellCheckerEngine;
        PersianSpellChecker spellCheckerEngine = new PersianSpellChecker(new SpellCheckerConfig(this.m_dicPath, this.m_editDistance, this.m_numSuggestions)
        {
          StemPath = this.m_stemPath
        });
        this.m_spellCheckerEngine = spellCheckerEngine;
        return spellCheckerEngine;
      }
    }

    public PersianSpellChecker GetPreSpellCheckerEngine(
      PreSpellCheckerUserOptions options)
    {
      lock (this.m_objLock)
      {
        int index = (int) options;
        if (index < 0 || index >= this.m_preSpellCheckerEngines.Length)
          throw new ArgumentOutOfRangeException(nameof (options));
        if (this.m_preSpellCheckerEngines[index] != null)
          return this.m_preSpellCheckerEngines[index];
        PersianSpellChecker spellCheckerEngine = new PersianSpellChecker(new SpellCheckerConfig(this.m_dicPath, this.m_editDistance, this.m_numSuggestions)
        {
          StemPath = this.m_stemPath
        });
        this.m_preSpellCheckerEngines[index] = spellCheckerEngine;
        OnePassCorrectionRules enginePreSpellOptions = VerificationEngines.ToEnginePreSpellOptions(options);
        foreach (OnePassCorrectionRules rules in Enum.GetValues(typeof (OnePassCorrectionRules)))
        {
          if (enginePreSpellOptions.Has<OnePassCorrectionRules>(rules))
            spellCheckerEngine.SetOnePassCorrectionRules(rules);
          else
            spellCheckerEngine.UnsetOnePassCorrectionRules(rules);
        }
        return spellCheckerEngine;
      }
    }

    public PunctuationCheckerEngine GetPunctuationEngine()
    {
      lock (this.m_objLock)
      {
        if (this.m_punctuationEngine == null)
          this.m_punctuationEngine = new PunctuationCheckerEngine(this.m_puncFile);
      }
      return this.m_punctuationEngine;
    }

    public void CreateAllSpellingEngines() => this.GetSpellCheckerEngine();

    public void CreateAllPreSpellingEngines()
    {
      for (int options = 0; options < this.m_preSpellCheckerEngines.Length; ++options)
        this.GetPreSpellCheckerEngine((PreSpellCheckerUserOptions) options);
    }

    public void CreateAllEngines()
    {
      this.CreateAllPreSpellingEngines();
      this.CreateAllSpellingEngines();
      this.GetPunctuationEngine();
    }
  }
}

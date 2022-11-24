// Decompiled with JetBrains decompiler
// Type: SCICT.VirastyarInlineVerifiers.InlineDateVerifier
// Assembly: SCICT.VirastyarInlineVerifiers, Version=1.3.1.23136, Culture=neutral, PublicKeyToken=null
// MVID: A2DBDBAC-2B04-4582-887A-A2282827433A
// Assembly location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.dll
// XML documentation location: C:\Projects\virastyar-master\virastyar-master\Examples\Virastyar_SpellCheck_Sample1\CorrectPersian\Lib\SCICT.VirastyarInlineVerifiers.xml

using SCICT.NLP.Utility.Calendar;
using SCICT.NLP.Utility.Parsers;
using System;
using System.Collections.Generic;

namespace SCICT.VirastyarInlineVerifiers
{
  public class InlineDateVerifier : ShrinkingVerifierBase
  {
    private readonly PersianDateParser m_perianDataParser = new PersianDateParser();
    private readonly EnglishDateParser m_englishDateParser = new EnglishDateParser();
    private readonly NumericDateParser m_numericDateParser = new NumericDateParser();
    private readonly List<IPatternInfo> m_lstPatternInfo = new List<IPatternInfo>();
    private IPatternInfo m_minPi;

    protected override bool NeedRefinedStrings => true;

    protected override VerificationInstance FindPattern(string content)
    {
      this.m_lstPatternInfo.Clear();
      foreach (PersianDatePatternInfo persianDatePatternInfo in this.m_perianDataParser.FindAndParse(content))
      {
        if (persianDatePatternInfo.YearNumber >= 0)
          this.m_lstPatternInfo.Add((IPatternInfo) persianDatePatternInfo);
      }
      foreach (EnglishDatePatternInfo englishDatePatternInfo in this.m_englishDateParser.FindAndParse(content))
      {
        if (englishDatePatternInfo.YearNumber >= 0)
          this.m_lstPatternInfo.Add((IPatternInfo) englishDatePatternInfo);
      }
      foreach (IPatternInfo patternInfo in this.m_numericDateParser.FindAndParse(content))
        this.m_lstPatternInfo.Add(patternInfo);
      if (this.m_lstPatternInfo.Count <= 0)
        return (VerificationInstance) null;
      this.m_minPi = this.PeakFirstVerification();
      string[] sugs;
      string[] groups;
      this.CreateSuggestions(this.m_minPi, out sugs, out groups);
      return new VerificationInstance(this.m_minPi.Index, this.m_minPi.Length, VerificationTypes.Information, sugs, groups);
    }

    private IPatternInfo PeakFirstVerification()
    {
      IPatternInfo patternInfo1 = (IPatternInfo) null;
      int num = int.MaxValue;
      foreach (IPatternInfo patternInfo2 in this.m_lstPatternInfo)
      {
        if (patternInfo2.Index < num)
        {
          num = patternInfo2.Index;
          patternInfo1 = patternInfo2;
        }
      }
      return patternInfo1;
    }

    private void CreateSuggestions(IPatternInfo minPi, out string[] sugs, out string[] groups)
    {
      switch (minPi.PatternInfoType)
      {
        case PatternInfoTypes.EnglishDate:
          this.CreateEnglishDateSuggestions(minPi as EnglishDatePatternInfo, out sugs, out groups);
          break;
        case PatternInfoTypes.NumericDate:
          this.CreateNumericDateSuggestions(minPi as NumericDatePatternInfo, out sugs, out groups);
          break;
        case PatternInfoTypes.PersianDate:
          this.CreatePersianDateSuggestions(minPi as PersianDatePatternInfo, out sugs, out groups);
          break;
        default:
          throw new ArgumentException("minPi.PatterinInfoType");
      }
    }

    private DateCalendarType GuessCalendarType(int yearNumber)
    {
      DateTime today = DateTime.Today;
      int year1 = today.Year;
      int year2 = new PersianCalendarEx(today).GetYear();
      int year3 = new HijriCalendarEx(today).GetYear();
      int num1 = Math.Abs(yearNumber - year1);
      DateCalendarType dateCalendarType = DateCalendarType.Gregorian;
      int num2 = Math.Abs(yearNumber - year2);
      if (num2 < num1)
      {
        num1 = num2;
        dateCalendarType = DateCalendarType.Jalali;
      }
      int num3 = Math.Abs(yearNumber - year3);
      if (num3 < num1)
      {
        num1 = num3;
        dateCalendarType = DateCalendarType.HijriGhamari;
      }
      if (yearNumber < 100)
      {
        int num4 = Math.Abs(1300 + yearNumber - year2);
        if (num4 < num1)
        {
          num1 = num4;
          dateCalendarType = DateCalendarType.Jalali;
        }
        if (Math.Abs(2000 + yearNumber - year1) < num1)
          dateCalendarType = DateCalendarType.Gregorian;
      }
      return dateCalendarType;
    }

    private int GetMostPossibleYearNumber(int yearNumber, DateCalendarType calendarType)
    {
      if (yearNumber <= 0 || yearNumber >= 100)
        return yearNumber;
      int possibleYearNumber = yearNumber;
      switch (calendarType)
      {
        case DateCalendarType.Gregorian:
          if (2000 - (1900 + yearNumber) < 2000 + yearNumber - 2000)
          {
            possibleYearNumber += 1900;
            break;
          }
          possibleYearNumber += 2000;
          break;
        case DateCalendarType.Jalali:
          possibleYearNumber += 1300;
          break;
        case DateCalendarType.HijriGhamari:
          possibleYearNumber += 1400;
          break;
      }
      return possibleYearNumber;
    }

    private void CreateNumericDateSuggestions(
      NumericDatePatternInfo pi,
      out string[] sugs,
      out string[] groups)
    {
      DateCalendarType calendarType = pi != null ? this.GuessCalendarType(pi.YearNumber) : throw new ArgumentNullException(nameof (pi));
      this.CreateGeneralDateSuggestions(new NumericDatePatternInfo(pi.Content, pi.Index, pi.Length, pi.DayNumber, pi.MonthNumber, this.GetMostPossibleYearNumber(pi.YearNumber, calendarType)), calendarType, out sugs, out groups);
    }

    private void CreateEnglishDateSuggestions(
      EnglishDatePatternInfo pi,
      out string[] sugs,
      out string[] groups)
    {
      if (pi == null)
        throw new ArgumentNullException(nameof (pi));
      this.CreateGeneralDateSuggestions(new NumericDatePatternInfo(pi.Content, pi.Index, pi.Length, pi.DayNumber, pi.MonthNumber, this.GetMostPossibleYearNumber(pi.YearNumber, pi.CalendarType)), pi.CalendarType, out sugs, out groups);
    }

    private void CreatePersianDateSuggestions(
      PersianDatePatternInfo pi,
      out string[] sugs,
      out string[] groups)
    {
      if (pi == null)
        throw new ArgumentNullException(nameof (pi));
      this.CreateGeneralDateSuggestions(new NumericDatePatternInfo(pi.Content, pi.Index, pi.Length, pi.DayNumber, pi.MonthNumber, this.GetMostPossibleYearNumber(pi.YearNumber, pi.CalendarType)), pi.CalendarType, out sugs, out groups);
    }

    private void CreateGeneralDateSuggestions(
      NumericDatePatternInfo pi,
      DateCalendarType calendarType,
      out string[] sugs,
      out string[] groups)
    {
      if (pi == null)
        throw new ArgumentNullException(nameof (pi));
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      if (pi.YearNumber >= 0 && pi.DayNumber > 0 && pi.MonthNumber > 0)
      {
        switch (calendarType)
        {
          case DateCalendarType.Gregorian:
            try
            {
              string str1 = "تبدیل به تاریخ میلادی";
              DateTime dt = new DateTime(pi.YearNumber, pi.MonthNumber, pi.DayNumber);
              stringList1.Add(dt.ToString("d"));
              stringList1.Add(ParsingUtils.ConvertNumber2Persian(dt.ToString("d")));
              stringList1.Add(CalendarStringUtils.GetPersianDateString(dt));
              stringList2.Add(str1);
              stringList2.Add(str1);
              stringList2.Add(str1);
              string str2 = "تبدیل به تاریخ خورشیدی";
              PersianCalendarEx persianCalendarEx = new PersianCalendarEx(dt);
              stringList1.Add(persianCalendarEx.ToString("D"));
              stringList1.Add(ParsingUtils.ConvertNumber2Persian(persianCalendarEx.ToString("d")));
              stringList1.Add(persianCalendarEx.ToString("d"));
              stringList2.Add(str2);
              stringList2.Add(str2);
              stringList2.Add(str2);
              string str3 = "تبدیل به تاریخ قمری";
              HijriCalendarEx hijriCalendarEx = new HijriCalendarEx(dt);
              stringList1.Add(hijriCalendarEx.ToString("D"));
              stringList1.Add(ParsingUtils.ConvertNumber2Persian(hijriCalendarEx.ToString("d")));
              stringList1.Add(hijriCalendarEx.ToString("d"));
              stringList2.Add(str3);
              stringList2.Add(str3);
              stringList2.Add(str3);
              break;
            }
            catch
            {
              break;
            }
          case DateCalendarType.Jalali:
            try
            {
              string str4 = "تبدیل به تاریخ خورشیدی";
              PersianCalendarEx persianCalendarEx = new PersianCalendarEx(pi.YearNumber, pi.MonthNumber, pi.DayNumber);
              stringList1.Add(ParsingUtils.ConvertNumber2Persian(persianCalendarEx.ToString()));
              stringList1.Add(persianCalendarEx.ToString());
              stringList1.Add(persianCalendarEx.ToString("D"));
              stringList2.Add(str4);
              stringList2.Add(str4);
              stringList2.Add(str4);
              string str5 = "تبدیل به تاریخ میلادی";
              DateTime dateTime = persianCalendarEx.DateTime;
              stringList1.Add(CalendarStringUtils.GetPersianDateString(dateTime));
              stringList1.Add(ParsingUtils.ConvertNumber2Persian(dateTime.ToString("d")));
              stringList1.Add(dateTime.ToString("d"));
              stringList2.Add(str5);
              stringList2.Add(str5);
              stringList2.Add(str5);
              string str6 = "تبدیل به تاریخ قمری";
              HijriCalendarEx hijriCalendarEx = new HijriCalendarEx(persianCalendarEx.DateTime);
              stringList1.Add(hijriCalendarEx.ToString("D"));
              stringList1.Add(ParsingUtils.ConvertNumber2Persian(hijriCalendarEx.ToString("d")));
              stringList1.Add(hijriCalendarEx.ToString("d"));
              stringList2.Add(str6);
              stringList2.Add(str6);
              stringList2.Add(str6);
              break;
            }
            catch
            {
              break;
            }
          case DateCalendarType.HijriGhamari:
            try
            {
              string str7 = "تبدیل به تاریخ قمری";
              HijriCalendarEx hijriCalendarEx = new HijriCalendarEx(pi.YearNumber, pi.MonthNumber, pi.DayNumber);
              stringList1.Add(ParsingUtils.ConvertNumber2Persian(hijriCalendarEx.ToString()));
              stringList1.Add(hijriCalendarEx.ToString());
              stringList1.Add(hijriCalendarEx.ToString("D"));
              stringList2.Add(str7);
              stringList2.Add(str7);
              stringList2.Add(str7);
              string str8 = "تبدیل به تاریخ میلادی";
              DateTime dateTime = hijriCalendarEx.DateTime;
              stringList1.Add(CalendarStringUtils.GetPersianDateString(dateTime));
              stringList1.Add(ParsingUtils.ConvertNumber2Persian(dateTime.ToString("d")));
              stringList1.Add(dateTime.ToString("d"));
              stringList2.Add(str8);
              stringList2.Add(str8);
              stringList2.Add(str8);
              string str9 = "تبدیل به تاریخ خورشیدی";
              PersianCalendarEx persianCalendarEx = new PersianCalendarEx(hijriCalendarEx.DateTime);
              stringList1.Add(persianCalendarEx.ToString("D"));
              stringList1.Add(ParsingUtils.ConvertNumber2Persian(persianCalendarEx.ToString("d")));
              stringList1.Add(persianCalendarEx.ToString("d"));
              stringList2.Add(str9);
              stringList2.Add(str9);
              stringList2.Add(str9);
              break;
            }
            catch (Exception ex)
            {
              break;
            }
        }
      }
      sugs = stringList1.ToArray();
      groups = stringList2.ToArray();
    }
  }
}

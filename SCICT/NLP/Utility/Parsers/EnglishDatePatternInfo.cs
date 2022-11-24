namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// Class to hold information about the parsed English date patterns.
    /// </summary>
    public class EnglishDatePatternInfo : IPatternInfo
    {
        readonly string m_content;

        /// <summary>
        /// Gets the content of the found pattern.
        /// </summary>
        /// <value>The m_content.</value>
        public string Content
        {
            get { return m_content; }
        }

        private int m_index;

        /// <summary>
        /// Gets the index of the original string at which the found pattern begins.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        private readonly int m_length;

        /// <summary>
        /// Gets the length of the found pattern.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return m_length; }
        }

        private readonly DateCalendarType m_calType;

        /// <summary>
        /// Gets the type of the calendar.
        /// </summary>
        /// <value>The type of the calendar.</value>
        public DateCalendarType CalendarType
        {
            get { return m_calType; }
        }

        private readonly Weekdays m_weekday;

        /// <summary>
        /// Gets the day of the week.
        /// </summary>
        /// <value>The day of the week.</value>
        public Weekdays Weekday
        {
            get { return m_weekday; }
        }

        private readonly int m_dayNumber;

        /// <summary>
        /// Gets the day number (in month).
        /// </summary>
        /// <value>The day number.</value>
        public int DayNumber
        {
            get { return m_dayNumber; }
        }

        private readonly int m_monthNumber;

        /// <summary>
        /// Gets the month number.
        /// </summary>
        /// <value>The month number.</value>
        public int MonthNumber
        {
            get { return m_monthNumber; }
        }

        private readonly int m_yearNumber;

        /// <summary>
        /// Gets the year number.
        /// </summary>
        /// <value>The year number.</value>
        public int YearNumber
        {
            get { return m_yearNumber; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnglishDatePatternInfo"/> class.
        /// </summary>
        /// <param name="content">The m_content.</param>
        /// <param name="index">The m_index.</param>
        /// <param name="length">The length of the pattern found.</param>
        /// <param name="t">The type of the calendar.</param>
        /// <param name="w">The day of the week.</param>
        /// <param name="dayNo">The day number (in month).</param>
        /// <param name="monthNo">The month number.</param>
        /// <param name="yearNo">The year number.</param>
        public EnglishDatePatternInfo(string content, int index, int length, DateCalendarType t, Weekdays w, int dayNo, int monthNo, int yearNo)
        {
            m_content = content;
            m_index = index;
            m_length = length;
            m_calType = t;
            m_weekday = w;
            m_dayNumber = dayNo;
            m_monthNumber = monthNo;
            m_yearNumber = yearNo;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Content:{1}{0}At:{2}{0}\tT:{7}{0}\tW:{3}{0}\tD:{4}{0}\tM:{5}{0}\tY:{6}{0}", Environment.NewLine,
                m_content, m_index, m_weekday, m_dayNumber, m_monthNumber, m_yearNumber, m_calType);
        }

        #region IPatternInfo Members

        /// <summary>
        /// Gets the type of the pattern info.
        /// </summary>
        /// <value>The type of the pattern info.</value>
        public PatternInfoTypes PatternInfoType
        {
            get { return PatternInfoTypes.EnglishDate; }
        }

        #endregion
    }

}

namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// Class to hold information about the parsed numeric date patterns.
    /// </summary>
    public class NumericDatePatternInfo : IPatternInfo
    {
        string content;

        /// <summary>
        /// Gets the content of the found pattern.
        /// </summary>
        /// <value>The content.</value>
        public string Content
        {
            get { return content; }
        }

        private int index;

        /// <summary>
        /// Gets the index of the original string at which the found pattern begins.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        private int length;

        /// <summary>
        /// Gets the length of the found pattern.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return length; }
        }

        private int dayNumber;

        /// <summary>
        /// Gets the day number (in month).
        /// </summary>
        /// <value>The day number.</value>
        public int DayNumber
        {
            get { return dayNumber; }
        }

        private int monthNumber;

        /// <summary>
        /// Gets the month number.
        /// </summary>
        /// <value>The month number.</value>
        public int MonthNumber
        {
            get { return monthNumber; }
        }

        private int yearNumber;

        /// <summary>
        /// Gets the year number.
        /// </summary>
        /// <value>The year number.</value>
        public int YearNumber
        {
            get { return yearNumber; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericDatePatternInfo"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length of the pattern found.</param>
        /// <param name="dayNo">The day number (in month).</param>
        /// <param name="monthNo">The month number.</param>
        /// <param name="yearNo">The year number.</param>
        public NumericDatePatternInfo(string content, int index, int length, int dayNo, int monthNo, int yearNo)
        {
            this.content = content;
            this.index = index;
            this.length = length;
            this.dayNumber = dayNo;
            this.monthNumber = monthNo;
            this.yearNumber = yearNo;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Content:{1}{0}At:{2}{0}\tD:{3}{0}\tM:{4}{0}\tY:{5}{0}", Environment.NewLine,
                content, index, dayNumber, monthNumber, yearNumber);
        }

        /// <summary>
        /// Gets the type of the pattern info.
        /// </summary>
        /// <value>The type of the pattern info.</value>
        public PatternInfoTypes PatternInfoType
        {
            get { return PatternInfoTypes.NumericDate; }
        }

    }

}

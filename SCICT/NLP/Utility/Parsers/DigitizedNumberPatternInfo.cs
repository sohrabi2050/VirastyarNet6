namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// Will contain information about the Digitized Numbers found in a string 
    /// as returned by <see cref="DigitizedNumberParser"/> class.
    /// </summary>
    public class DigitizedNumberPatternInfo : IPatternInfo
    {
        /// <summary>
        /// Gets the content of the found pattern.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; private set; }

        /// <summary>
        /// Gets the index of the original string at which the found pattern begins.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the length of the found pattern.
        /// </summary>
        /// <value>The length.</value>
        public int Length { get; private set; }

        /// <summary>
        /// Gets the number parsed from the pattern.
        /// </summary>
        /// <value>The number.</value>
        public double Number { get; private set; }

        /// <summary>
        /// Gets the type of the pattern info.
        /// </summary>
        /// <value>The type of the pattern info.</value>
        public PatternInfoTypes PatternInfoType
        {
            get { return PatternInfoTypes.DigitizedNumber; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitizedNumberPatternInfo"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="index">The index.</param>
        /// <param name="len">The length of the found pattern.</param>
        /// <param name="number">The parsed number.</param>
        public DigitizedNumberPatternInfo(string content, int index, int len, double number)
        {
            Content = content;
            Index = index;
            Length = len;
            Number = number;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="DigitizedNumberPatternInfo"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="DigitizedNumberPatternInfo"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Content:{1}{0}At:{2}{0}\tValue:{3}{0}", Environment.NewLine,
                Content, Index , Number);
        }

    }
}

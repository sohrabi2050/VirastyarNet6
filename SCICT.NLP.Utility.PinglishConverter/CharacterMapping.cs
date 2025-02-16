//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//



using System.Collections.Generic;

namespace SCICT.NLP.Utility.PinglishConverter
{
    /// <summary>
    /// Holds all possible mapping information for a letter.
    /// </summary>
    public class CharacterMapping
    {
        #region Private Fields

        private readonly List<CharacterMappingInfo> m_values;
        private readonly char m_letter;
        private readonly bool m_caseSensitive;
        //private readonly object m_label;
        
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMapping"/> class.
        /// </summary>
        /// <param name="letter">The letter which this instance will hold its mappings.</param>
        /// <param name="values">Mapping values for the given letter.</param>
        public CharacterMapping(char letter, CharacterMappingInfo[] values)
            : this(letter, false, values)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMapping"/> class.
        /// </summary>
        /// <param name="letter">The letter which this instance will hold its mappings.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <param name="values">Mapping values for the given letter.</param>
        private CharacterMapping(char letter, bool caseSensitive, IEnumerable<CharacterMappingInfo> values)
        {
            this.m_letter = letter;
            this.m_caseSensitive = caseSensitive;
            this.m_values = new List<CharacterMappingInfo>(values);
            this.m_values.Sort();
        }

        /// <summary>
        /// Gets the letter which this instance holds its mapping information.
        /// </summary>
        /// <value>The letter.</value>
        public char Letter
        {
            get
            {
                return this.m_letter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is case sensitive.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is case sensitive; otherwise, <c>false</c>.
        /// </value>
        public bool IsCaseSensitive
        {
            get { return this.m_caseSensitive; }
        }

        /// <summary>
        /// Gets the corresponding mapping information of the <see cref="Letter"/>
        /// </summary>
        /// <value>The values.</value>
        public CharacterMappingInfo[] Values
        {
            get
            {
                return (this.m_values != null) ? this.m_values.ToArray() : null;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.m_letter != '\0')
            {
                return this.m_letter.ToString();
            }
            return "";
            /*else
            {
                return this.m_label.ToString();
            }*/
        }
    }
}

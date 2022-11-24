// Author: Omid Kashefi, Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mehrdad Senobari at 2010-March-08
//

namespace SCICT.Utility.IO
{
    ///<summary>
    /// Load dictionary file
    ///</summary>
    public class DictionaryLoader
    {
        protected string m_filename;
        protected StreamReader m_streamReader;
        protected StreamWriter m_streamWriter;
        protected bool m_endOfStream = false;

        ///<summary>
        /// End of Stream
        ///</summary>
        public bool EndOfStream
        {
            get
            {
                return m_endOfStream;
            }
        }

        ///<summary>
        /// Load file
        ///</summary>
        ///<param name="fileName">File name</param>
        ///<returns>True if suucessfully loade, otherwise False</returns>
        public bool LoadFile(string fileName)
        {
            m_filename = fileName;
            if (m_filename.Length == 0)
            {
                return false;
            }

            if (!File.Exists(m_filename))
            {
                return false;
            }

            try
            {
                m_streamReader = new StreamReader(m_filename);
            }
            catch (Exception)
            {
                return false;
            }

            m_endOfStream = false;
            return true;
        }

        ///<summary>
        /// Get next line
        ///</summary>
        ///<param name="line">Line contents</param>
        ///<returns>True if not EOF, False on EOF</returns>
        public bool NextLine(out string line)
        {
            line = "";

            if (m_streamReader == null)
            {
                return false;
            }

            try
            {
                this.m_endOfStream = m_streamReader.EndOfStream;
                if (!m_streamReader.EndOfStream)
                {
                    line = m_streamReader.ReadLine();
                    return true;
                }
                else
                {
                    m_streamReader.Close();
                    m_streamReader = null;
                    return false;
                }
            }
            catch(Exception)
            {
                m_streamReader.Close();
                m_streamReader = null;

                return false;
            }
        }

        /// <summary>
        /// Close Stream Reader
        /// </summary>
        public void CloseFile()
        {
            if (m_streamReader != null)
            {
                m_streamReader.Close();
            }
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="line">word</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddLine(string line)
        {
            if (m_filename.Length == 0)
            {
                return false;
            }

            if (!File.Exists(m_filename))
            {
                return false;
            }

            try
            {
                m_streamWriter = new StreamWriter(m_filename, true);
                m_streamWriter.WriteLine(line);
                m_streamWriter.Close();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        ///<summary>
        /// Add a term to dictionary
        ///</summary>
        ///<param name="line">word</param>
        ///<param name="fileName">File name</param>
        ///<returns>True if word successfully added, otherwise False</returns>
        public bool AddLine(string line, string fileName)
        {
            if (fileName.Length == 0)
            {
                return false;
            }

            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                using (m_streamWriter = new StreamWriter(fileName, true))
                {
                    m_streamWriter.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }

}

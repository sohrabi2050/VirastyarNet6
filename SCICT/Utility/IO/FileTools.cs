// Author: Omid Kashefi, Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mehrdad Senobari at 2010-March-08
//


using System.Diagnostics;
using System.Text;

namespace SCICT.Utility.IO
{
    ///<summary>
    /// Generic tools for filing
    ///</summary>
    public class FileTools
    {
        /// <summary>
        /// Find the position (byte index) of the given word in the specified stream.
        /// </summary>
        /// <param name="fstream"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static long GetWordStartPositionInFile(FileStream fstream, string word)
        {
            #region Variables

            byte[] BOM = { 0xEF, 0xBB, 0xBF };
            const int BufferSize = 0x10000;

            byte[] byteBuff = new byte[BufferSize];
            byte[] zeroBuff = new byte[BufferSize];
            char[] charBuff = new char[BufferSize];

            fstream.Seek(0, SeekOrigin.Begin);

            long startPoisiton = 0;

            int readedCount;

            bool wholeStrFound = false;

            Encoding UTF8 = Encoding.UTF8;

            int buffWriteIndex = 0;
            bool eof = false;

            #endregion

            #region Check for BOM - If file is created manually
            if (fstream.Read(byteBuff, 0, 3) == 3)
            {
                bool isEqual = true;
                for (int i = 0; i < BOM.Length; i++)
                {
                    if (BOM[i] != byteBuff[i])
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual)
                    buffWriteIndex = 0;
                else
                    buffWriteIndex = 3;
            }
            #endregion

            do
            {
                readedCount = fstream.Read(byteBuff, buffWriteIndex, BufferSize - buffWriteIndex);
                eof = (readedCount != BufferSize - buffWriteIndex);

                int lastOperationalIndex = Array.LastIndexOf<byte>(byteBuff, 0x0D);
                if (lastOperationalIndex + 1 != BufferSize && byteBuff[lastOperationalIndex + 1] == 0x0A)
                    ++lastOperationalIndex;

                int charCount = UTF8.GetChars(byteBuff, 0, lastOperationalIndex + 1, charBuff, 0);

                for (int i = 0; i < charCount; i++)
                {
                    #region Check first character
                    if (charBuff[i] == word[0])
                    {
                        if ((i - 1) > 0 && charBuff[i - 1] != 0x0A && charBuff[i - 1] != 0x0D)
                            continue;

                        #region Searsch to find the match
                        wholeStrFound = true;
                        for (var j = 1; j < word.Length; j++)
                        {
                            Debug.Assert(i + j < charBuff.Length && charBuff[i + j] != 0);

                            if (word[j] == charBuff[i + j])
                            {
                                continue;
                            }
                            else
                            {
                                wholeStrFound = false;
                                break;
                            }
                            //else // TODO: Remove this after completion
                            //{
                            //    wholeStrFound = true;
                            //}
                        }

                        #endregion

                        if (wholeStrFound && char.IsWhiteSpace(charBuff[i + word.Length]))
                        {
                            // Calculate Position
                            startPoisiton = (fstream.Position - (readedCount + buffWriteIndex)) + UTF8.GetByteCount(charBuff, 0, i);
                            return startPoisiton;
                        }
                    }
                    #endregion
                }

                buffWriteIndex = BufferSize - (lastOperationalIndex + 1);

                #region Copy Buffers
                Buffer.BlockCopy(byteBuff, lastOperationalIndex + 1, byteBuff, 0, buffWriteIndex);
                Buffer.BlockCopy(zeroBuff, 0, byteBuff, buffWriteIndex, BufferSize - buffWriteIndex);
                Buffer.BlockCopy(zeroBuff, 0, charBuff, 0, BufferSize);
                #endregion

            } while (!eof);

            return -1;
        }

        ///<summary>
        /// Remove a line from file
        ///</summary>
        ///<param name="fstream">Opened file stream</param>
        ///<param name="position">position of line</param>
        public static void RemoveLineFromPosition(FileStream fstream, long position)
        {
            int BufferSize = 0x400;

            byte[] byteBuff = new byte[BufferSize];
            char[] charBuff = new char[BufferSize];

            // 1. Go to next line
            fstream.Seek(position, SeekOrigin.Begin);
            fstream.Read(byteBuff, 0, BufferSize);

            int firstEndOfLine = Array.IndexOf<byte>(byteBuff, 0x0D, 0);

            byte[] wholeOfFile = new byte[fstream.Length - (position + firstEndOfLine + 2)];
            fstream.Seek(position + firstEndOfLine + 2, SeekOrigin.Begin);
            int readedCount = fstream.Read(wholeOfFile, 0, wholeOfFile.Length);
            fstream.Seek(position, SeekOrigin.Begin);
            fstream.Write(wholeOfFile, 0, wholeOfFile.Length);

            fstream.SetLength(position + wholeOfFile.Length);
        }
    }

}

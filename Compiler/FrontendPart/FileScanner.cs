using System;
using System.IO;

namespace Compiler.FrontendPart
{
    public class FileScanner
    {
        public StreamReader ReadStream { get; }
        private string currentLine;
        
        public FileScanner(string fileName)
        {
            try
            {
                ReadStream = new StreamReader(fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        ~FileScanner()
        {
            ReadStream.Close();
        }
        

        public string ReadLine()
        {
            try
            {
                if (!(ReadStream.EndOfStream))
                {
                    currentLine = ReadStream.ReadLine();
                    return currentLine;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public string PeekLine()
        {
            return currentLine;
        }
    }
}
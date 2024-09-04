using System;

namespace ZipLogTool
{
    public class CmdOutput
    {
        private int currentLevel;

        // Constructor to set the initial level
        public CmdOutput(int level)
        {
            currentLevel = level;
        }

        // Method to output a message based on the level
        public void WriteLine(int level, string message)
        {
            if (level >= currentLevel)
            {
                Console.WriteLine(message);
            }
        }

        // Optionally, a method to change the level at runtime
        public void SetLevel(int level)
        {
            currentLevel = level;
        }
    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        // Set the CmdOutput level to 2, for example
    //        var cmdOutput = new CmdOutput(2);

    //        // Example usage
    //        cmdOutput.WriteLine(1, "This is a level 1 message."); // Will not be printed
    //        cmdOutput.WriteLine(2, "This is a level 2 message."); // Will be printed
    //        cmdOutput.WriteLine(3, "This is a level 3 message."); // Will be printed

    //        // You can change the level later if needed
    //        cmdOutput.SetLevel(3);
    //        cmdOutput.WriteLine(2, "This is another level 2 message."); // Will not be printed
    //        cmdOutput.WriteLine(3, "This is another level 3 message."); // Will be printed
    //    }
    //}
}

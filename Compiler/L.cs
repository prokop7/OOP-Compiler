using System;
using System.Linq;

namespace Compiler
{
	public static class L
	{
		public static int LogLevel { get; set; } = 0;
		
		
		public static void Log(string message, int level)
		{
			var spaces = string.Concat(Enumerable.Repeat("  ", level));
			if (level <= L.LogLevel)
				Console.WriteLine($"{DateTime.Now}\t{spaces}{message}");
		}
	}
}
using System;
using System.IO;

namespace Brainfuck_Interpreter
{
	class Program
	{
		static void Main(string[] args)
		{
			if(args.Length == 0)
			{
				throw new ArgumentException("No file supplied");
			}

			string filename = args[0];

			if(!File.Exists(filename))
			{
				throw new FileNotFoundException("File does not exist");
			}

			Interpreter brainfuckInterpreter = new();

			var code = File.ReadAllText(filename);

			brainfuckInterpreter.Instructions = code;
			brainfuckInterpreter.InterpretCode();
			brainfuckInterpreter.Reset();

			Console.WriteLine("Press any key to continue...");
			//New iteration on keypress
			Console.ReadKey();
		}
	}
}
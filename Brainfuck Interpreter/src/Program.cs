using System;
using System.IO;

namespace Brainfuck_Interpreter
{
	class Program
	{
		static void Main(string[] args)
		{
			Interpreter brainfuckInterpreter = new Interpreter();

			while (true)
			{
				string filename = string.Empty;
				string code = string.Empty;

				if (args.Length > 0 && File.Exists(args[0]))
				{
					filename = args[0];
					code = File.ReadAllText(filename);

					//Emptying args so the file wont be run on next iteration
					args = new string[0];
				}
				else
					code = GetInput();

				brainfuckInterpreter.Instructions = code;
				brainfuckInterpreter.InterpretCode();
				brainfuckInterpreter.Reset();

				Console.WriteLine("Press any key to continue...");
				//New iteration on keypress
				Console.ReadKey();
			}
		}

		private static string GetInput()
		{
			string code = string.Empty;

			Console.Clear();
			Console.Write("1. By file\n2. By Brainfuck code\nChoice: ");
			var choice = Console.ReadKey().KeyChar;
			switch (choice)
			{
				case '1':
					string inputFilename = string.Empty;
					do
					{
						Console.Clear();
						Console.Write("Enter filename: ");
						inputFilename = Console.ReadLine();
					} while (!File.Exists(inputFilename));

					code = File.ReadAllText(inputFilename);

					Console.Write("\n");
					break;
				default:
					Console.Clear();
					Console.WriteLine("Enter your brainfuck code:");
					string inputString = string.Empty;
					do
					{
						inputString = Console.ReadLine();
						code += inputString;
					} while (!(string.IsNullOrEmpty(inputString)));
					break;
			}

			return code;
		}
	}
}
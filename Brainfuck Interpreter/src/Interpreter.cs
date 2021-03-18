using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Brainfuck_Interpreter
{
	class Interpreter
	{
		//All valid brainfuck instructions
		private static readonly char[] brainfuckInstructions = { '>', '<', '+', '-', '.', ',', '[', ']'};

		//All the instructions to interpret
		public string Instructions { get; set; }

		//Represents the tape
		private byte[] memory;

		//Pointer to current memory address
		private int memoryPointer;

		//Pointer to current instruction
		private int instructionPointer;

		//Used to build the output while interpreting
		private StringBuilder outputBuffer;

		//Keeps track of all indices of loop begins for nested loops
		private Stack<int> loopBeginPointers;

		private char[] tokens;

		public Interpreter()
		{
			Reset();
		}

		public void InterpretCode()
		{
			tokens = Instructions.Where(x => brainfuckInstructions.Contains(x)).ToArray();

			while(instructionPointer < tokens.Count())
			{
				NextStep();
			}

			Console.WriteLine($"Output:\n{outputBuffer.ToString()}\n");
		}

		private void NextStep()
		{
			switch (tokens[instructionPointer])
			{
				case '>':
					memoryPointer++;
					break;
				case '<':
					memoryPointer--;
					break;
				case '+':
					memory[memoryPointer]++;
					break;
				case '-':
					memory[memoryPointer]--;
					break;
				case '.':
					//Brainfuck standard to use 10 for a newline
					if (memory[memoryPointer] == 10)
						outputBuffer.Append('\n');
					else
						outputBuffer.Append((char)memory[memoryPointer]);
					break;
				case ',':
					Console.Write("Enter a character: ");
					char c = Console.ReadKey().KeyChar;
					memory[memoryPointer] = (byte)c;

					//Entering a newline after receiving character
					Console.Write("\n\n");
					break;
				case '[':
					//If current cell is 0 then jump behind the corresponding closing bracket
					if (memory[memoryPointer] == 0)
					{
						JumpToMatchingClosingBracket();
					}
					else
					{
						//Add loop begin index to stack
						loopBeginPointers.Push(instructionPointer);
					}
					break;
				case ']':
					if (memory[memoryPointer] == 0)
					{
						//Remove innermost loop begin pointer
						loopBeginPointers.Pop();
					}
					else
					{
						//Get innermost loop begin pointer and jump there
						instructionPointer = loopBeginPointers.Peek();
					}
					break;
			}

			instructionPointer++;
		}

		private void JumpToMatchingClosingBracket()
		{
			//0 based loop depth
			int depth = 0;
			instructionPointer++;
			while (instructionPointer < tokens.Count())
			{
				//New loop is one depth deeper
				if (tokens[instructionPointer] == '[')
					depth++;
				//Corresponding bracket found
				else if (tokens[instructionPointer] == ']' && depth == 0)
					break;
				//Inner closing bracket found
				else
					depth--;
			}
		}

		public void Reset()
		{
			memory = new byte[32768];

			memoryPointer = 0;

			instructionPointer = 0;

			Instructions = string.Empty;

			outputBuffer = new StringBuilder();

			loopBeginPointers = new Stack<int>();
		}
	}
}


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

		private readonly Dictionary<char, Action> tokenToAction = new Dictionary<char, Action>();

		public Interpreter()
		{
			Reset();

			tokenToAction = new Dictionary<char, Action>()
			{
				{ '>', () => IncrementMemoryPointer() },
				{ '<', () => DecrementMemoryPointer() },
				{ '+', () => IncrementCell() },
				{ '-', () => DecrementCell() },
				{ '.', () => AddToOutput() },
				{ ',', () => GetInput() },
				{ '[', () => LoopStart() },
				{ ']', () => LoopEnd() }
			};
		}

		public void InterpretCode()
		{
			tokens = Instructions.Where(x => brainfuckInstructions.Contains(x)).ToArray();

			while(instructionPointer < tokens.Count())
			{
				NextStep();
			}

			Console.WriteLine($">>>>>Output Start<<<<<:\n{outputBuffer.ToString()}\n>>>>>Output End<<<<<\n");
		}

		private void NextStep()
		{
			var currentToken = tokens[instructionPointer];

			tokenToAction[currentToken].Invoke();

			instructionPointer++;
		}

		private void IncrementMemoryPointer()
		{
			memoryPointer++;
		}

		private void DecrementMemoryPointer()
		{
			memoryPointer--;
		}

		private void IncrementCell()
		{
			memory[memoryPointer]++;
		}

		private void DecrementCell()
		{
			memory[memoryPointer]--;
		}

		/// <summary>
		/// Takes the value out of the current cell and adds the ASCII equivalent to the output buffer
		/// </summary>
		private void AddToOutput()
		{
			//Brainfuck standard to use 10 for a newline
			if (memory[memoryPointer] == 10)
				outputBuffer.Append('\n');
			else
				outputBuffer.Append((char)memory[memoryPointer]);
		}

		/// <summary>
		/// Logic to handle an open bracket (a loop start)
		/// </summary>
		private void LoopStart()
		{
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
		}

		/// <summary>
		/// Jumps to the corresponding closing bracket of an open bracket
		/// </summary>
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
				else if (tokens[instructionPointer] == ']')
				{
					//Corresponding Bracket found
					if (depth == 0)  break; 
					//Inner closing Bracket
					else depth--;
				}
				instructionPointer++;
			}
		}

		/// <summary>
		/// Logic to handle a closing bracket (a loop end)
		/// </summary>
		private void LoopEnd()
		{
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
		}

		/// <summary>
		/// Takes one byte of input and stores it in the current cell
		/// </summary>
		private void GetInput()
		{
			Console.Write("Enter a character: ");
			char c = Console.ReadKey().KeyChar;
			memory[memoryPointer] = (byte)c;

			//Entering a newline after receiving character
			Console.Write("\n\n");
		}

		/// <summary>
		/// Resets the state of the interpreter
		/// </summary>
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
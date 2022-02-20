using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Brainfuck_Interpreter
{
	class Interpreter
	{
		//All valid brainfuck instructions
		private static readonly char[] BrainfuckInstructions = { '>', '<', '+', '-', '.', ',', '[', ']'};

		//All the instructions to interpret
		public string Instructions { get; set; }

		//Represents the tape
		private byte[] _memory;

		//Pointer to current memory address
		private int _memoryPointer;

		//Pointer to current instruction
		private int _instructionPointer;

		//Used to build the output while interpreting
		private StringBuilder _outputBuffer;

		//Keeps track of all indices of loop begins for nested loops
		private Stack<int> _loopBeginPointers;

		private char[] _tokens;

		private readonly Dictionary<char, Action> _tokenToAction = new();

		public Interpreter()
		{
			Reset();

			_tokenToAction = new Dictionary<char, Action>()
			{
				{ '>', IncrementMemoryPointer },
				{ '<', DecrementMemoryPointer },
				{ '+', IncrementCell },
				{ '-', DecrementCell },
				{ '.', AddToOutput },
				{ ',', GetInput },
				{ '[', LoopStart },
				{ ']', LoopEnd }
			};
		}

		public void InterpretCode()
		{
			_tokens = Instructions.Where(x => BrainfuckInstructions.Contains(x)).ToArray();

			while(_instructionPointer < _tokens.Length)
			{
				NextStep();
			}

			Console.WriteLine($">>>>>Output Start<<<<<:\n{_outputBuffer}\n>>>>>Output End<<<<<\n");
		}

		private void NextStep()
		{
			var currentToken = _tokens[_instructionPointer];

			_tokenToAction[currentToken].Invoke();

			_instructionPointer++;
		}

		private void IncrementMemoryPointer()
		{
			_memoryPointer++;
		}

		private void DecrementMemoryPointer()
		{
			_memoryPointer--;
		}

		private void IncrementCell()
		{
			_memory[_memoryPointer]++;
		}

		private void DecrementCell()
		{
			_memory[_memoryPointer]--;
		}

		/// <summary>
		/// Takes the value out of the current cell and adds the ASCII equivalent to the output buffer
		/// </summary>
		private void AddToOutput()
		{
			//Brainfuck standard to use 10 for a newline
			if (_memory[_memoryPointer] == 10)
				_outputBuffer.Append('\n');
			else
				_outputBuffer.Append((char)_memory[_memoryPointer]);
		}

		/// <summary>
		/// Logic to handle an open bracket (a loop start)
		/// </summary>
		private void LoopStart()
		{
			//If current cell is 0 then jump behind the corresponding closing bracket
			if (_memory[_memoryPointer] == 0)
			{
				JumpToMatchingClosingBracket();
			}
			else
			{
				//Add loop begin index to stack
				_loopBeginPointers.Push(_instructionPointer);
			}
		}

		/// <summary>
		/// Jumps to the corresponding closing bracket of an open bracket
		/// </summary>
		private void JumpToMatchingClosingBracket()
		{
			//0 based loop depth
			int depth = 0;
			_instructionPointer++;
			while (_instructionPointer < _tokens.Length)
			{
				//New loop is one depth deeper
				if (_tokens[_instructionPointer] == '[')
					depth++;
				//Corresponding bracket found
				else if (_tokens[_instructionPointer] == ']')
				{
					//Corresponding Bracket found
					if (depth == 0)  break; 
					//Inner closing Bracket
					else depth--;
				}
				_instructionPointer++;
			}
		}

		/// <summary>
		/// Logic to handle a closing bracket (a loop end)
		/// </summary>
		private void LoopEnd()
		{
			if (_memory[_memoryPointer] == 0)
			{
				//Remove innermost loop begin pointer
				_loopBeginPointers.Pop();
			}
			else
			{
				//Get innermost loop begin pointer and jump there
				_instructionPointer = _loopBeginPointers.Peek();
			}
		}

		/// <summary>
		/// Takes one byte of input and stores it in the current cell
		/// </summary>
		private void GetInput()
		{
			Console.Write("Enter a character: ");
			char c = Console.ReadKey().KeyChar;
			_memory[_memoryPointer] = (byte)c;

			//Entering a newline after receiving character
			Console.Write("\n\n");
		}

		/// <summary>
		/// Resets the state of the interpreter
		/// </summary>
		public void Reset()
		{
			_memory = new byte[32768];

			_memoryPointer = 0;

			_instructionPointer = 0;

			Instructions = string.Empty;

			_outputBuffer = new StringBuilder();

			_loopBeginPointers = new Stack<int>();
		}
	}
}
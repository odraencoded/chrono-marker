using System;
using System.Text.RegularExpressions;

namespace Chrono
{
	public class TimeParser : IValidator
	{
		public TimeParser() 
		: this(TimeSpan.MinValue, TimeSpan.MaxValue)
		{ }

		public TimeParser(TimeSpan lowerCap, TimeSpan upperCap)
		{
			LowerCap = lowerCap;
			UpperCap = upperCap;
		}

		public TimeSpan LowerCap { get; set; }
		public TimeSpan UpperCap { get; set; }

		public TimeSpan LastValidated { get; private set; }

		public bool Validate(string text)
		{
			TimeSpan time;
			if( TryParseTime( text, out time ) ) {
				if(time >= LowerCap && time <= UpperCap) {
					LastValidated = time;
					return true;
				} else return false;
			}
			else return false;
		}

		/// <summary>
		/// Tries to parse a string into a TimeSpan.
		/// <returns>True if the string was succesfully parsed.</returns>
		/// </param>
		public static bool TryParseTime(string input, out TimeSpan result)
		{
			const int maxUnitCount = 4;
			
			input = input.Trim( );

			result = new TimeSpan();

			if( string.IsNullOrEmpty( input ) ) // This makes easier to undo
				return false;
			
			bool hasLeftSymbol; // This is a "+" or '-" symbol
			bool isNegative; // ...or is it?
			bool hasHours, hasMinutes, hasSeconds, hasMilli;
			hasLeftSymbol = isNegative =
				hasHours = hasMinutes = hasSeconds = hasMilli = false;

			int hoursIndex, minutesIndex, secondsIndex, milliIndex;
			hoursIndex = minutesIndex = secondsIndex = milliIndex = -1;

			int hoursVal, minutesVal, secondsVal, milliVal;
			hoursVal = minutesVal = secondsVal = milliVal = 0;

			bool knowSeparatorType, usesAbbreviations;
			knowSeparatorType = usesAbbreviations = false;

			int unitCount = 0;
			string[] manyUnitStrings = new string[maxUnitCount];
			
			for(int i = 0; i < input.Length;)
			{
				if(char.IsDigit(input[i]))
				{
					if(unitCount == maxUnitCount)
						return false;
					
					string timeUnit = "";

					do // Bet ya haven't seen one of these in a _while_
					{
						timeUnit += input[i];
						i++;
					} while(i < input.Length && char.IsDigit(input[i]));

					manyUnitStrings[unitCount] = timeUnit;
					unitCount++;
				}
				else
				{
					string nonDigitPart = "";
					
					do
					{
						nonDigitPart += input[i];
						i++;
					} while(i < input.Length && !char.IsDigit(input[i]));
					
					nonDigitPart = nonDigitPart.Trim().ToLowerInvariant();

					if(unitCount == 0)
					{
						if(!hasLeftSymbol)
						{
							isNegative = nonDigitPart == "-";

							if(isNegative) hasLeftSymbol = true;
							else if(nonDigitPart == "+") hasLeftSymbol = true;
							else return false;
						}
						else return false;

						continue;
					}

					if(!knowSeparatorType)
					{
						// Clearly, this will fail to check non valid separators.
						// Those are handled next.
						usesAbbreviations = !(nonDigitPart == ":" || nonDigitPart == ".");
						knowSeparatorType = true;
					}
					
					if(usesAbbreviations)
					{
						// While using abbreviations, order doesn't matter
						if(!hasHours && nonDigitPart == "h")
						{
							hasHours = true;
							hoursIndex = unitCount - 1;
						}
						else if(!hasMinutes && nonDigitPart == "m")
						{
							hasMinutes = true;
							minutesIndex = unitCount - 1;
						}
						else if(!hasSeconds && nonDigitPart == "s")
						{
							hasSeconds = true;
							secondsIndex = unitCount - 1;
						}
						else if(!hasMilli && nonDigitPart == "ms")
						{
							hasMilli = true;
							milliIndex = unitCount - 1;
						}
						else return false;
					}
					else
					{
						// There are no separators at the end of the string.
						// What would they separate if there were?
						if(i == input.Length)
							return false;

						if(nonDigitPart == ":" && !(hasHours && hasMinutes))
						{
							// The format is invalid if the "." separators comes
							// before the ":" separator.
							if(hasMilli)
								return false;

							if(!hasMinutes) hasMinutes = true;
							else hasHours = true;
						}
						else if(nonDigitPart == "." && !hasMilli)
						{
							hasMilli = true;
						}
						else return false;
					}
				}
			}

			if(unitCount == 0) 
				return false;

			// If there are neither separators or abbreviations
			// The program implies they are all seconds.
			if(!knowSeparatorType) // Implicit unitCount == 1
			{
				hasSeconds = true;
				usesAbbreviations = false;
			}

			if(!usesAbbreviations)
			{
				hasSeconds = true; // This is always true in this case.

				// Infer unit positioning from separator order here.
				if(hasHours) hoursIndex = 0;
				if(hasMinutes) minutesIndex = hasHours ? hoursIndex + 1 : 0;
				if(hasSeconds) secondsIndex = hasMinutes ? minutesIndex + 1 : 0;
				if(hasMilli) milliIndex = secondsIndex + 1;
			}

			for(int i = 0; i < unitCount; i++)
			{
				int unitVal;
				string unitString = manyUnitStrings[i];

				if(!usesAbbreviations)
				{
					if(i == milliIndex)
					{
						if(unitString.Length > 3)
							return false;
						
						unitString = unitString.PadRight(3, '0');
					}

					if(i != 0 && (i == secondsIndex || i == minutesIndex))
					{
						if(unitString.Length != 2)
							return false;
					}
				}

				// Even though the string is all digits, that won't prevent overflow errors
				if(!int.TryParse(unitString, out unitVal))
					return false;

				if(i == milliIndex)
				{
					milliVal = unitVal; 

					if((hasSeconds || hasMinutes || hasHours) && milliVal >= 1000)
						return false;
				}
				else if (i == secondsIndex)
				{
					secondsVal = unitVal;

					if((hasMinutes || hasHours) && secondsVal >= 60)
						return false;
				}
				else if(i == minutesIndex)
				{
					minutesVal = unitVal;

					if(hasHours && minutesVal >= 60)
						return false;
				}
				else if(i == hoursIndex)
				{
					hoursVal = unitVal;
				}
				else return false; // This supposedly never happens though.
			}

			result = new TimeSpan(0, hoursVal, minutesVal, secondsVal, milliVal);

			if(isNegative) result = result.Negate();

			return true;
		}
	}
}


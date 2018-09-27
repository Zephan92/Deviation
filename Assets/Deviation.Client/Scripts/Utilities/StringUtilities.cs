using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Deviation.Client.Scripts.Utilities
{
	public class StringUtilities
	{
		public const int THOUSAND	= 1000;
		public const int MILLION	= 1000000;
		public const int BILLION	= 1000000000;

		public static int ConvertShortIntStringToInt(string numberText)
		{
			if (numberText.Length == 0)
			{
				return 0;
			}

			string lastCharacter = numberText.Substring(numberText.Length - 1, 1);
			string remaining = numberText.Substring(0, numberText.Length - 1);

			double amount;
			bool success = Double.TryParse(remaining, out amount);

			if (success)
			{
				switch (lastCharacter)
				{
					case "k":
						return (int) (amount * THOUSAND);
					case "m":
						return (int) (amount * MILLION);
					case "b":
						return (int) (amount * BILLION);
				}
			}

			success = Double.TryParse(numberText, out amount);

			if (success)
			{
				return (int) amount;
			}

			return 0;
		}

		public static int ConvertLongIntStringToInt(string number)
		{
			return int.Parse(number, NumberStyles.AllowThousands);
		}

		public static string ConvertIntToShortIntString(int number)
		{
			if (number >= THOUSAND * 100 && number < MILLION * 10)
			{
				return $"{number / THOUSAND}k";
			}
			else if (number >= MILLION * 10 && number <= Int32.MaxValue)
			{
				return $"{number / MILLION}m";
			}
			else
			{
				return $"{number}";
			}
		}

		public static string ConvertStringToShortIntString(string numberText)
		{
			return ConvertIntToShortIntString(Int32.Parse(numberText));
		}

		public static string ConvertIntToLongIntString(int number)
		{
			return number.ToString("#,##0");
		}
	}
}

namespace Tgs.GameInterface.TPhere.Core.Extensions;

public static class StringExtensions
{
	public static string NoLongerThan(this string str, int chars)
		=> string.IsNullOrEmpty(str) || str.Length <= chars ? str : str[..chars];

	public static string StringLength(this string str, int minimumLength = 0, int maximumLength = 30)
	{
		return str.Length < minimumLength || str.Length > maximumLength
			? throw new ArgumentOutOfRangeException(str, $"The length of this string is minimum{minimumLength}maximum{maximumLength}.({str})")
			: str;
	}
}

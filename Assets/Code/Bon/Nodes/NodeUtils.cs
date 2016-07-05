using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class NodeUtils {


	/** Draws a textfield that accepts float inputs only.
		Returns true if the value has changed.
	**/
	public static bool FloatTextField(Rect area, ref string number)
	{
		var textFieldValue = GUI.TextField(area, number);
		var newTextFieldValue = GetValidNumberString(textFieldValue, number);
		var numberChanged = !IsEqualFloatValue(newTextFieldValue, number);
		number = newTextFieldValue;
		return numberChanged;
	}


	public static string GetValidNumberString(string text, string defaultNumber)
	{
		if (text == "") text = "0";
		if (Regex.Match(text, @"^-?[0-9]*(?:\.[0-9]*)?$").Success) return text;
		return defaultNumber;
	}

	public static bool IsEqualFloatValue(string number01, string number02)
	{
		return !(System.Math.Abs(float.Parse(number01) - float.Parse(number02)) > 0);
	}

	public static Color GetMapValueColor(float value)
	{
		if (value > 1f) return Color.red;
		if (value < 0f) return Color.blue;
		return Color.white * value;
	}

}

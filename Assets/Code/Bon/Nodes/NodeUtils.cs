using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Code.Bon.Interface;

public class NodeUtils {

	public const string NotConnectedMessage = "not connected";
	private static Texture2D _staticRectTexture;
	private static GUIStyle _staticRectStyle;

	/** Draws a textfield that accepts float inputs only.
		Returns true if the value has changed.
	**/
	public static bool FloatTextField(Rect area, ref string number)
	{
		if (number == null) return false;
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
		if (float.IsNaN(value)) return Color.magenta;
		if (value > 1f) return Color.red;
		if (value < -1f) return Color.blue;
		Color c = Color.white * (value + 1f) / 2f;
		c.a = 1;
		return c;
	}


	public static Color[] ToColorMap(float[,] values, IColorSampler colorSampler = null)
	{
		int width = values.GetLength(0);
		int height = values.GetLength(1);
		Color[] colorMap = new Color[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				Color c;
				if (colorSampler == null) c = GetMapValueColor(values[x, y]);
				else c = colorSampler.GetColor(null, (values[x, y] + 1f) / 2f);
				colorMap[y * width + x] = c;
			}
		}
		return colorMap;
	}


	public static void GUIDrawRect(Rect position, Color color )
	{
		if(_staticRectTexture == null) _staticRectTexture = new Texture2D( 1, 1 );
		if(_staticRectStyle == null) _staticRectStyle = new GUIStyle();
		_staticRectTexture.SetPixel(0, 0, color);
		_staticRectTexture.Apply();
		_staticRectStyle.normal.background = _staticRectTexture;
		GUI.Box( position, GUIContent.none, _staticRectStyle );
	}

	public static float ModifySeed(float baseSeed, float modifierSeed)
	{
		if (modifierSeed == 0) return baseSeed;
		return baseSeed * modifierSeed % float.MaxValue;
	}

}

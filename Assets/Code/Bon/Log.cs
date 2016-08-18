using System;
using Assets.Code.Bon;
using UnityEngine;

public static class Log  {


	public static void Info(String info)
	{
		if (BonConfig.LogLevel > 0)
		{
			Debug.Log(info);
		}
	}

}

using UnityEngine;
using System.Collections;
using Assets.Code.Bon.Graph;

public interface IMathNode
{
	float GetNumber(Socket outSocket);
}

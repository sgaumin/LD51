using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CardLevelData", order = 1)]
public class CardLevelData : ScriptableObject
{
	public int spawnMax = 3;
	public List<GameObject> items;
	[Range(0f, 1f)] public float probability;
}
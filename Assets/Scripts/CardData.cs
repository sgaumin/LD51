using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CardData", order = 1)]
public class CardData : ScriptableObject
{
	public string cardName;
	public Sprite cardSprite;
	public Sprite slotSprite;
	public int weigth;
	public List<CardLevelData> levels = new List<CardLevelData>();
}
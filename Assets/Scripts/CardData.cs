using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CardData", order = 1)]
public class CardData : ScriptableObject
{
	public string cardName;
	public Sprite cardSprite;
	public Sprite slotSprite;
	public int weigth;
}
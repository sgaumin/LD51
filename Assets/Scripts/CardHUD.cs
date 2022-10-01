using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Utils;
using Utils.Dependency;
using UnityEngine;
using UnityEngine.UI;
using static Facade;

public class CardHUD : Singleton<CardHUD>
{
	[SerializeField] private SpriteRenderer goldIcon;
	[SerializeField] private TextMeshProUGUI goldCountText;

	public Vector2 GoldIconPosition => goldIcon.transform.position;

	public void SetGoldValue(int value)
	{
		goldCountText.text = value.ToString();
	}
}
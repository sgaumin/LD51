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

public class ShopButton : MonoBehaviour
{
	private enum ShopButtonType
	{
		Life,
		Attack,
		Skip
	}

	[SerializeField] private ShopButtonType type;
	[SerializeField] private int startCost = 5;
	[SerializeField] private int maxValue;

	[Header("References")]
	[SerializeField] private TextMeshProUGUI costText;
	[SerializeField] private Button button;
	[SerializeField] private GameObject indications;
	[SerializeField] private TextMeshProUGUI buttonDescription;

	private bool isMaxed;

	private void Awake()
	{
		Player.OnReceivingGold += UpdateStatus;
	}

	private void Start()
	{
		UpdateCost();
	}

	private void UpdateCost()
	{
		costText.text = startCost.ToString();
		UpdateStatus(Player.Gold);
	}

	private void CheckMaxed()
	{
		switch (type)
		{
			case ShopButtonType.Life:
				isMaxed = Player.MaxLife >= maxValue;
				break;

			case ShopButtonType.Attack:
				isMaxed = Player.Attack >= maxValue;
				break;
		}
	}

	public void UpdateStatus(int value)
	{
		CheckMaxed();

		if (isMaxed)
		{
			button.interactable = false;
			buttonDescription.text = "MAX";
			indications.SetActive(false);
		}
		else
		{
			button.interactable = value >= startCost;
			costText.color = value >= startCost ? Color.white : Color.red;
		}
	}

	public void Buy()
	{
		if (Player.Gold < startCost) return;

		Player.Gold -= startCost;
		switch (type)
		{
			case ShopButtonType.Life:
				Player.IncreaseMaxLife();

				break;

			case ShopButtonType.Attack:
				Player.Attack++;
				isMaxed = Player.Attack >= maxValue;
				break;

			case ShopButtonType.Skip:
				Card.SkipCard();
				break;

			default:
				break;
		}
	}
}
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
using System.Drawing.Text;

public class ShopButton : MonoBehaviour
{
	private enum ShopButtonType
	{
		Life,
		Attack,
		Skip
	}

	[SerializeField] private ShopButtonType type;
	[SerializeField] private List<int> prices = new List<int>();

	[Header("Sounds")]
	[SerializeField] private AudioExpress clickSound;

	[Header("References")]
	[SerializeField] private TextMeshProUGUI costText;
	[SerializeField] private Button button;
	[SerializeField] private GameObject indications;
	[SerializeField] private TextMeshProUGUI buttonDescription;

	private bool isMaxed;
	private int currentPriceIndex = 0;

	private int ExtraLoopCost => Mathf.FloorToInt(Player.LoopIndex / 3);

	private void Awake()
	{
		Player.OnEndLoop += UpdateCost;
		Player.OnReceivingGold += UpdateStatus;
	}

	private void Start()
	{
		UpdateCost();
	}

	private void UpdateCost()
	{
		CheckMaxed();
		if (isMaxed) return;

		if (type == ShopButtonType.Skip)
		{
			costText.text = (prices[currentPriceIndex] + ExtraLoopCost).ToString();
		}
		else
		{
			costText.text = prices[currentPriceIndex].ToString();
		}
		UpdateStatus(Player.Gold);
	}

	private void CheckMaxed()
	{
		switch (type)
		{
			case ShopButtonType.Life:
				isMaxed = Player.MaxLife >= Player.StartMaxLife + prices.Count;
				break;

			case ShopButtonType.Attack:
				isMaxed = Player.Attack >= Player.StartAttack + prices.Count;
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
			if (type == ShopButtonType.Skip)
			{
				button.interactable = value >= prices[currentPriceIndex] + ExtraLoopCost;
				costText.color = value >= prices[currentPriceIndex] + ExtraLoopCost ? Color.white : Color.red;
			}
			else
			{
				button.interactable = value >= prices[currentPriceIndex];
				costText.color = value >= prices[currentPriceIndex] ? Color.white : Color.red;
			}
		}
	}

	public void Buy()
	{
		if (type == ShopButtonType.Skip)
		{
			if (Player.Gold < prices[0] + ExtraLoopCost) return;
		}
		else
		{
			if (Player.Gold < prices[currentPriceIndex]) return;
		}

		switch (type)
		{
			case ShopButtonType.Life:
				Player.IncreaseMaxLife();
				Player.Gold -= prices[currentPriceIndex++];
				break;

			case ShopButtonType.Attack:
				Player.Attack++;
				Player.Gold -= prices[currentPriceIndex++];
				break;

			case ShopButtonType.Skip:
				Card.SkipCard();
				Player.Gold -= prices[0] + ExtraLoopCost;
				break;

			default:
				break;
		}

		clickSound.Play();
		UpdateCost();
	}
}
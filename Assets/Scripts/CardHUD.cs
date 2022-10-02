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
	[Header("Gold")]
	[SerializeField] private SpriteRenderer goldIcon;
	[SerializeField] private TextMeshProUGUI goldCountText;

	[Header("Life")]
	[SerializeField] private Sprite fullHeart;
	[SerializeField] private Sprite emptyHeart;
	[SerializeField] private Transform holderHeart;
	[SerializeField] private Image statsIndicatorPrefab;

	[Header("Attack")]
	[SerializeField] private Sprite attackSprite;
	[SerializeField] private Transform holderAttack;

	[Header("Cards")]
	[SerializeField] private List<CardData> cards;
	[SerializeField] private Sprite defaultCardSprite;
	[SerializeField] private TextMeshProUGUI cardDescription;
	[SerializeField] private SpriteRenderer cardSpriteRenderer;

	[Header("HUD")]
	[SerializeField] private TextMeshProUGUI loopIndicator;
	[SerializeField] private CanvasGroup shopButtons;

	private CardData current;
	private List<Image> lifeIndicators = new List<Image>();
	private List<Image> attackIndicators = new List<Image>();

	public CardData Current
	{
		get => current;
		private set
		{
			current = value;
			cardSpriteRenderer.sprite = current.cardSprite;
			cardDescription.text = current.cardName;
		}
	}
	public Vector2 GoldIconPosition => goldIcon.transform.position;

	protected override void Awake()
	{
		base.Awake();
		Level.OnBuildingPhase += GetCard;
		Level.OnBuildingPhase += Show;
		Level.OnLoopingPhase += ResetCard;
		Level.OnLoopingPhase += Hide;
		Player.OnReceivingGold += SetGoldValue;
	}

	public void SetGoldValue(int value)
	{
		goldCountText.text = value.ToString();
	}

	private void ResetCard()
	{
		cardSpriteRenderer.sprite = defaultCardSprite;
		cardDescription.text = "";
	}

	public void UpdateAttack()
	{
		int missing = Player.Attack - attackIndicators.Count;
		if (missing > 0)
		{
			for (int i = 0; i < missing; i++)
			{
				Image indicator = Instantiate(statsIndicatorPrefab, holderAttack);
				indicator.sprite = attackSprite;
				attackIndicators.Add(indicator);
			}
		}
	}

	public void UpdateLife()
	{
		int missing = Player.MaxLife - lifeIndicators.Count;
		if (missing > 0)
		{
			for (int i = 0; i < missing; i++)
			{
				Image indicator = Instantiate(statsIndicatorPrefab, holderHeart);
				lifeIndicators.Add(indicator);
			}
		}

		for (int i = 0; i < lifeIndicators.Count; i++)
		{
			lifeIndicators[i].sprite = i <= (Player.Life - 1) ? fullHeart : emptyHeart;
		}
	}

	public void Show()
	{
		loopIndicator.DOKill();
		loopIndicator.DOFade(1f, 0.2f);

		shopButtons.DOFade(1f, 0.2f);
		shopButtons.interactable = true;
	}

	public void Hide()
	{
		loopIndicator.DOKill();
		loopIndicator.DOFade(0f, 0.2f);

		shopButtons.DOFade(0f, 0.2f);
		shopButtons.interactable = false;
	}

	public void SetLoopIndicator(int value)
	{
		loopIndicator.text = value.ToString();
	}

	public void SkipCard()
	{
		List<CardData> weigthed = new List<CardData>();
		foreach (CardData card in cards)
		{
			if (card.cardName == Current.cardName) continue;

			for (int i = 0; i < card.weigth; i++)
			{
				weigthed.Add(card);
			}
		}
		Current = weigthed.Random();
	}

	private void GetCard()
	{
		List<CardData> weigthed = new List<CardData>();
		foreach (CardData card in cards)
		{
			for (int i = 0; i < card.weigth; i++)
			{
				weigthed.Add(card);
			}
		}
		Current = weigthed.Random();
	}
}
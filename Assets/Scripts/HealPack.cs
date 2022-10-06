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

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class HealPack : MonoBehaviour, ISwordTarget
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private AudioExpress collectSound;

	public void Interact()
	{
		collectSound.Play();
		spriteRenderer.gameObject.layer = LayerMask.NameToLayer("CardHUD");
		spriteRenderer.sortingLayerName = "CardHUD";
		spriteRenderer.sortingOrder = 10;

		transform.DOMove(Card.GetCurrentHeartIconPosition(), 0.6f).SetEase(Ease.OutSine).OnComplete(() =>
		{
			Player.Life++;
			Destroy(gameObject);
		});
	}
}
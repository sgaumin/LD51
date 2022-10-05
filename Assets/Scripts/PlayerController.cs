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
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : Singleton<PlayerController>
{
	public Action OnStartLoop;
	public Action OnEndLoop;
	public Action OnTick;
	public Action<int> OnReceivingGold;

	[SerializeField] private int startLifePoint = 3;
	[SerializeField] private int startGold = 0;
	[SerializeField] private int startAttack = 1;

	[Header("Animations")]
	[SerializeField] private float fullRotationDuration = 2f;
	[SerializeField] private Ease easeFullRotation;
	[SerializeField] private Ease easeRotation;

	[Header("Audio")]
	[SerializeField] private AudioExpress movementSound;
	[SerializeField] private AudioExpress damageSound;

	[Header("Effects")]
	[SerializeField] private ParticleSystem dirtEffect;

	[Header("References")]
	[SerializeField] private SwordController sword;

	private Coroutine swordRotation;
	private Coroutine swordFullRotation;
	private int goldCount;
	private int attackCount;
	private int maxLifeCount;
	private int lifeCount;
	private int loopIndex;

	public int Gold
	{
		get => goldCount; set
		{
			goldCount = value;
			OnReceivingGold?.Invoke(goldCount);
		}
	}

	public int Attack
	{
		get => attackCount;
		set
		{
			attackCount = value;
			Card.UpdateAttack();
		}
	}

	public int StartMaxLife => startLifePoint;
	public int StartAttack => startAttack;

	public int MaxLife
	{
		get => maxLifeCount;
		private set
		{
			maxLifeCount = Mathf.Max(0, value);
			Card.UpdateLife();
		}
	}

	public int Life
	{
		get => lifeCount;
		set
		{
			lifeCount = Mathf.Clamp(value, 0, maxLifeCount);
			Card.UpdateLife();

			if (lifeCount <= 0)
			{
				Level.ReloadScene();
			}
		}
	}

	public int LoopIndex
	{
		get => loopIndex;
		set
		{
			loopIndex = value;
			OnEndLoop?.Invoke();
			Card.SetLoopIndicator(loopIndex);
			Level.State = SceneState.BuildingPhase;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Level.OnLoopingPhase += StartSwordRotation;
	}

	private void Start()
	{
		LoopIndex = 0;
		Attack = startAttack;
		MaxLife = startLifePoint;
		Life = startLifePoint;
		Gold = startGold;

		FullRotation(3);
	}

	private void HealFull()
	{
		Life = MaxLife;
	}

	private void FullRotation(int turnCount = 1)
	{
		this.TryStartCoroutine(FullRotationCore(turnCount), ref swordFullRotation);
	}

	private IEnumerator FullRotationCore(int turnCount = 1)
	{
		for (int i = 0; i < turnCount; i++)
		{
			sword.transform?.DOKill();
			sword.transform.rotation = Quaternion.identity;
			Tweener rotater = sword.transform.DORotate(new Vector3(0f, 0f, 360f), fullRotationDuration, RotateMode.FastBeyond360).SetEase(easeFullRotation);
			yield return rotater.WaitForCompletion();
		}
	}

	public void StartSwordRotation()
	{
		this.TryStartCoroutine(RotationCore(), ref swordRotation);
	}

	public void StopSwordRotation()
	{
		this.TryStopCoroutine(ref swordRotation);
	}

	private IEnumerator RotationCore()
	{
		OnStartLoop?.Invoke();

		float step = 360f / 10f;
		int tickCount = 0;
		while (tickCount < 10)
		{
			movementSound.Play();
			dirtEffect.Play();
			sword.transform?.DOKill();
			Tweener rotater = sword.transform.DORotate(new Vector3(0f, 0f, -step), 1f, RotateMode.LocalAxisAdd).SetEase(easeRotation);
			yield return rotater.WaitForCompletion();

			tickCount++;
			OnTick?.Invoke();
		}

		LoopIndex++;
	}

	public void IncreaseMaxLife()
	{
		MaxLife++;
		HealFull();
	}

	public void TakeDamage()
	{
		Life--;
		damageSound.Play();
		Level.GenerateImpulse();
		Level.FreezeTime();
	}

	private void OnDestroy()
	{
		OnStartLoop = null;
		OnEndLoop = null;
		OnTick = null;
	}
}
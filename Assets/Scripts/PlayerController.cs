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

public class PlayerController : Singleton<PlayerController>
{
	public Action OnStartLoop;
	public Action OnEndLoop;
	public Action OnTick;

	[Header("Animations")]
	[SerializeField] private Ease easeRotation;

	[Header("References")]
	[SerializeField] private SwordController sword;

	private Coroutine swordRotation;
	private int goldCount;

	public int Gold
	{
		get => goldCount; set
		{
			goldCount = value;
			CardHUD.Instance.SetGoldValue(goldCount);
		}
	}
	public int Life { get; private set; }
	public int LoopIndex { get; set; }

	protected override void Awake()
	{
		base.Awake();
		Level.OnLoopingPhase += StartSwordRotation;
	}

	private void Start()
	{
		Gold = 0;
	}

	[ContextMenu("StartSwordRotation")]
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
			sword.transform?.DOKill();
			Tweener rotater = sword.transform.DORotate(new Vector3(0f, 0f, -step), 1f, RotateMode.LocalAxisAdd).SetEase(easeRotation);
			yield return rotater.WaitForCompletion();

			tickCount++;
			OnTick?.Invoke();
		}
		OnEndLoop?.Invoke();
		Level.State = SceneState.BuildingPhase;
	}

	public void TakeDamage()
	{
		Life--;
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
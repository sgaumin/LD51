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
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour, ISwordTarget
{
	[SerializeField] private int startLifePoint = 1;
	[SerializeField, Range(0f, 1f)] private float probabilityGold = 0.5f;
	[SerializeField, Range(0f, 1f)] private float probabilityShield = 0.3f;

	[Header("Movements")]
	[SerializeField] private float movementSpeed = 0.5f;
	[SerializeField] private float moveAroundMaxRange = 1f;
	[SerializeField] private float spawnMaxOffset = 1f;
	[SerializeField, FloatRangeSlider(0f, 3f)] private FloatRange movementBreakDuration = new FloatRange(0.8f, 2f);

	[Header("PushbackAnimation")]
	[SerializeField] private float pushbackFactor = 1f;
	[SerializeField, FloatRangeSlider(-90, 90f)] private FloatRange pushbackAngleVariation = new FloatRange(-30, 30f);
	[SerializeField] private float pushbackDuration = 0.2f;
	[SerializeField, FloatRangeSlider(0f, 2f)] private FloatRange pushbackBreakDuration = new FloatRange(0.8f, 1.2f);
	[SerializeField] private Ease pushbackEase;

	[Header("Attack")]
	[SerializeField] private EnemyBullet bulletPrefab;

	[Header("Life Bar")]
	[SerializeField] private GameObject lifeBarHolder;
	[SerializeField] private Image foregroundBar;

	[Header("Audio")]
	[SerializeField] private AudioExpress damageSound;

	[Header("References")]
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite shiledSprite;

	private int realStartLifePoints;
	private int lifePoints;
	private bool hasShield;
	private Vector2 spawnPoint;
	private bool hasBeenAlreadyTouched;
	private Coroutine pushbackRoutine;
	private Coroutine moveToSpawnRoutine;
	private Coroutine moveAroundRoutine;

	private void Awake()
	{
		Player.OnStartLoop += ResetStatus;
	}

	private void Start()
	{
		hasShield = Random.value <= probabilityShield;
		if (hasShield)
		{
			spriteRenderer.sprite = shiledSprite;
		}
		realStartLifePoints = startLifePoint + (hasShield ? 1 : 0);
		lifePoints = realStartLifePoints;
	}

	private void Update()
	{
		spriteRenderer.sortingOrder = -Mathf.FloorToInt(transform.position.y * 1000);
	}

	public void Init(Vector2 spawnPoint)
	{
		Vector2 spawnPointOffset = new Vector2((Random.value > 0.5f ? -1f : 1f) * spawnMaxOffset, (Random.value > 0.5f ? -1f : 1f) * spawnMaxOffset);
		this.spawnPoint = spawnPoint + spawnPointOffset;
	}

	private void ResetStatus()
	{
		hasBeenAlreadyTouched = false;
	}

	public void Interact()
	{
		if (hasBeenAlreadyTouched) return;

		damageSound.Play();
		Level.FreezeTime();
		hasBeenAlreadyTouched = true;
		lifePoints -= Player.Attack;

		if (lifePoints <= 0)
		{
			Kill();
			return;
		}

		UpdateLifeBar();
		PushBack();
	}

	private void PushBack()
	{
		transform?.DOKill();
		StopAllCoroutines();
		this.TryStartCoroutine(PushBackCore(), ref pushbackRoutine);
	}

	private IEnumerator PushBackCore()
	{
		Vector2 direction = (transform.position - Player.transform.position).normalized;
		direction = direction.Rotate(pushbackAngleVariation.RandomValue);
		Vector2 pushBackDestination = (Vector2)transform.position + direction * pushbackFactor;
		Tween mover = transform.DOMove(pushBackDestination, pushbackDuration).SetEase(pushbackEase);

		yield return mover.WaitForCompletion();
		yield return new WaitForSeconds(pushbackBreakDuration.RandomValue);

		Attack();
		MoveBackToSpawn();
	}

	public void MoveBackToSpawn()
	{
		this.TryStartCoroutine(MoveBackToSpawnCore(), ref moveToSpawnRoutine);
	}

	private IEnumerator MoveBackToSpawnCore()
	{
		Tween mover = transform.DOMove(spawnPoint, movementSpeed);
		yield return mover.WaitForCompletion();
		yield return new WaitForSeconds(movementBreakDuration.RandomValue);

		MoveAround();
	}

	private void MoveAround()
	{
		this.TryStartCoroutine(MoveAroundCore(), ref moveAroundRoutine);
	}

	private IEnumerator MoveAroundCore()
	{
		while (true)
		{
			Vector2 targetPosition = spawnPoint + new Vector2(Random.Range(-moveAroundMaxRange, moveAroundMaxRange), Random.Range(-moveAroundMaxRange, moveAroundMaxRange));
			Tween mover = transform.DOMove(targetPosition, movementSpeed);
			yield return mover.WaitForCompletion();
			yield return new WaitForSeconds(movementBreakDuration.RandomValue);
		}
	}

	private void UpdateLifeBar()
	{
		bool check = lifePoints == realStartLifePoints;
		lifeBarHolder.SetActive(!check);
		if (check) return;

		foregroundBar.fillAmount = (float)lifePoints / realStartLifePoints;
	}

	private void Attack()
	{
		damageSound.Play();
		EnemyBullet bullet = Instantiate(bulletPrefab);
		bullet.transform.position = transform.position;
		bullet.Shoot();
	}

	private void Kill()
	{
		if (Random.value <= probabilityGold)
		{
			Gold gold = Instantiate(Prefabs.goldPrefab);
			gold.Spawn(transform.position);
		}

		Destroy(gameObject);
	}
}
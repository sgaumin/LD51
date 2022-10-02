using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using static Facade;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class BuildingController : MonoBehaviour
{
	public Action OnKill;

	[SerializeField, IntRangeSlider(0, 10)] private IntRange goldAtStart = new IntRange(0, 3);
	[SerializeField, FloatRangeSlider(0f, 3f)] private FloatRange offset = new FloatRange(1f, 1.5f);
	[SerializeField] private SpriteRenderer spriteRenderer;

	[Header("Animation")]
	[SerializeField] private float loopDuration = 10f;
	[SerializeField] private List<Transform> animationPath = new List<Transform>();

	private CardData data;
	private Vector2 itemSpawn;
	private int spawnedCount;
	private int currentSpawnMax;
	private Queue<GameObject> items = new Queue<GameObject>();

	public void Init(Vector2 itemSpawn)
	{
		Player.OnStartLoop += Spawn;
		Player.OnEndLoop += CheckStatus;

		data = Card.Current;
		spriteRenderer.sprite = data.slotSprite;

		this.itemSpawn = itemSpawn;

		currentSpawnMax = Random.Range(1, data.spawnMax + 1);
		for (int i = 0; i < currentSpawnMax; i++)
		{
			GameObject item = Instantiate(data.item);
			Vector2 positionOffset = new Vector2((Random.value > 0.5f ? -1f : 1f) * offset.RandomValue, (Random.value > 0.5f ? -1f : 1f) * offset.RandomValue);
			item.transform.position = (Vector2)transform.position + positionOffset;
			items.Enqueue(item);

			List<Vector3> path = new List<Vector3>();
			animationPath.ForEach(x => path.Add(x.position));
			var finalPath = path.Shuffle();
			item.transform.DOPath(finalPath.ToArray(), loopDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
		}

		SpawnGold(goldAtStart.RandomValue);
	}

	private void Spawn()
	{
		if (Random.value > data.probability) return;

		spawnedCount++;

		GameObject nextItem = items.Dequeue();
		nextItem.transform?.DOKill();

		if (nextItem.TryGetComponent(out EnemyController enemy))
		{
			enemy.Init(itemSpawn);
			enemy.MoveBackToSpawn();
		}
		else
		{
			Vector2 spawnPointOffset = new Vector2((Random.value > 0.5f ? -1f : 1f) * 0.2f, (Random.value > 0.5f ? -1f : 1f) * 0.2f);
			nextItem.transform.DOMove(itemSpawn + spawnPointOffset, 1f);
		}
	}

	private void SpawnGold(int count)
	{
		for (int i = 0; i < count; i++)
		{
			Gold gold = Instantiate(Prefabs.goldPrefab);
			gold.transform.position = transform.position;
			Vector2 spawnPointOffset = new Vector2((Random.value > 0.5f ? -1f : 1f) * 0.2f, (Random.value > 0.5f ? -1f : 1f) * 0.2f);
			gold.transform.DOMove(itemSpawn + spawnPointOffset, 1f);
		}
	}

	private void CheckStatus()
	{
		if (spawnedCount >= currentSpawnMax)
		{
			Kill();
		}
	}

	private void Kill()
	{
		Player.OnStartLoop -= Spawn;
		OnKill?.Invoke();
		gameObject.SetActive(false);
	}
}
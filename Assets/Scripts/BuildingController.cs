using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utils;
using static Facade;
using Random = UnityEngine.Random;

public class BuildingController : MonoBehaviour
{
	public Action OnKill;

	[SerializeField] private int spawnMax = 3;
	[SerializeField] private GameObject itemToSpawn;
	[SerializeField, FloatRangeSlider(0f, 3f)] private FloatRange offset = new FloatRange(1f, 1.5f);

	private Vector2 itemSpawn;
	private int spawnedCount;
	private Queue<GameObject> items = new Queue<GameObject>();

	public void Init(Vector2 itemSpawn)
	{
		Player.OnStartLoop += Spawn;

		this.itemSpawn = itemSpawn;

		for (int i = 0; i < spawnMax; i++)
		{
			GameObject item = Instantiate(itemToSpawn);
			Vector2 positionOffset = new Vector2((Random.value > 0.5f ? -1f : 1f) * offset.RandomValue, (Random.value > 0.5f ? -1f : 1f) * offset.RandomValue);
			item.transform.position = (Vector2)transform.position + positionOffset;
			items.Enqueue(item);
		}
		// TODO: Gain gold on setup
	}

	private void Spawn()
	{
		spawnedCount++;

		if (items.Dequeue().TryGetComponent(out EnemyController enemy))
		{
			enemy.Init(itemSpawn);
			enemy.MoveBackToSpawn();
		}

		if (spawnedCount >= spawnMax)
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
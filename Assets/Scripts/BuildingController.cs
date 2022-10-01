using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utils;
using static Facade;

public class BuildingController : MonoBehaviour
{
	public Action OnKill;

	[SerializeField] private int spawnMax = 3;
	[SerializeField] private GameObject itemToSpawn;

	private Vector2 itemSpawn;
	private int spawnedCount;

	public void Init(Vector2 itemSpawn)
	{
		Player.OnStartLoop += Spawn;

		this.itemSpawn = itemSpawn;

		// TODO: Gain gold on setup
	}

	private void Spawn()
	{
		spawnedCount++;
		GameObject item = Instantiate(itemToSpawn);
		item.transform.position = transform.position;

		if (item.TryGetComponent(out EnemyController enemy))
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
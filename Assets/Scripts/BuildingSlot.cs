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

[RequireComponent(typeof(SpriteRenderer))]
public class BuildingSlot : MonoBehaviour
{
	[Header("Animations")]
	[SerializeField] private Color onEntered;
	[SerializeField] private Color onPressed;
	[SerializeField] private Color defaultColor;

	[Header("References")]
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private TextMeshProUGUI levelIndicator;
	[SerializeField] private BuildingController buildingPrefab;
	[SerializeField] private Transform itemSpawn;

	private BuildingController currentBuilding;
	private int currentLevel;

	public int CurrentLevel
	{
		get => currentLevel;
		private set
		{
			currentLevel = value;
			UpdateLevelIndicator(currentLevel);
		}
	}

	public void SetItemSpawn(Vector2 position)
	{
		itemSpawn.transform.position = position;
	}

	private void OnMouseEnter()
	{
		if (Level.State == SceneState.BuildingPhase && currentBuilding is null)
		{
			spriteRenderer.color = onEntered;
		}
	}

	private void OnMouseExit()
	{
		if (Level.State == SceneState.BuildingPhase && currentBuilding is null)
		{
			spriteRenderer.color = defaultColor;
		}
	}

	private void OnMouseDown()
	{
		if (Level.State == SceneState.BuildingPhase && currentBuilding is null)
		{
			spriteRenderer.color = onPressed;
		}
	}

	private void OnMouseUp()
	{
		if (Level.State == SceneState.BuildingPhase && currentBuilding is null)
		{
			spriteRenderer.color = onEntered;

			currentBuilding = Instantiate(buildingPrefab, transform);
			currentBuilding.transform.position = transform.position;
			currentBuilding.OnKill += OnBuildingKill;
			currentBuilding.Init(itemSpawn.position);

			Level.State = SceneState.LoopingPhase;
		}
	}

	private void OnBuildingKill()
	{
		CurrentLevel++;
		currentBuilding = null;
		spriteRenderer.color = defaultColor;
	}

	private void UpdateLevelIndicator(int value)
	{
		levelIndicator.text = value.ToString();
	}
}
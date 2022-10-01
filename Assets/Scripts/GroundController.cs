using Utils;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GroundController : MonoBehaviour
{
	[SerializeField] private int buildingSlotCount = 10;
	[SerializeField] private float buildingSlotDistanceFromOrigin = 2f;
	[SerializeField] private float canvasDistanceFromOrigin = 4f;
	[SerializeField, FloatRangeSlider(0f, 10f)] private FloatRange slotPositionOffset = new FloatRange(1f, 3f);
	[SerializeField, FloatRangeSlider(0f, 10f)] private FloatRange itemSpawnDistanceFromOrigin = new FloatRange(1f, 2f);
	[SerializeField] private float rotationOffset = 10f;
	[SerializeField] private Transform buildingHolder;
	[SerializeField] private BuildingSlot buildingSlopPrefab;

#if UNITY_EDITOR

	[Button]
	public void SpawnBuildingSlots()
	{
		FindObjectsOfType<BuildingSlot>().ForEach(x => DestroyImmediate(x.gameObject));

		float step = 360f / buildingSlotCount;
		for (int i = 0; i < buildingSlotCount; i++)
		{
			BuildingSlot slot = PrefabUtility.InstantiatePrefab(buildingSlopPrefab, buildingHolder) as BuildingSlot;
			Vector2 position = Vector2.up.Rotate(-step * i + rotationOffset);
			Vector2 positionOffset = new Vector2((Random.value > 0.5f ? -1f : 1f) * slotPositionOffset.RandomValue, (Random.value > 0.5f ? -1f : 1f) * slotPositionOffset.RandomValue);
			slot.transform.position = position * buildingSlotDistanceFromOrigin + (Vector2)transform.position + positionOffset;
			slot.transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1f / transform.localScale.z);
			slot.SetItemSpawn(position * itemSpawnDistanceFromOrigin.RandomValue + (Vector2)transform.position);
			slot.SetCanvas(position * canvasDistanceFromOrigin + (Vector2)transform.position + positionOffset);
			slot.name = $"BuildingSlot_{i}";
		}
	}

#endif
}
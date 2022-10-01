using DG.Tweening;
using UnityEngine;
using Utils;
using static Facade;

public class EnemyBullet : MonoBehaviour
{
	[SerializeField, FloatRangeSlider(0f, 2f)] private FloatRange speed = new FloatRange(1.2f, 1.5f);

	public void Shoot()
	{
		float duration = Vector2.Distance(Player.transform.position, transform.position) / speed.RandomValue;
		transform.DOMove(Player.transform.position, duration).OnComplete(() => { DealingDamage(); });
	}

	private void DealingDamage()
	{
		Player.TakeDamage();
		Destroy(gameObject);
	}
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SwordController : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		foreach (var component in collision.GetComponents<MonoBehaviour>())
		{
			if (component is ISwordTarget)
			{
				((ISwordTarget)component).Interact();
			}
		}
	}
}
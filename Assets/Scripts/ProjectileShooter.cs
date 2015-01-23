using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProjectileShooter : MonoBehaviour {

	public Image projectilePrefab;
	private Canvas canvas;

	void Awake() {
		this.canvas = GetComponentInParent<Canvas> ();
	}

	public void ShootMotherfuckingProjectile() {
		Image projectile = Object.Instantiate (projectilePrefab, new Vector2 (), Quaternion.identity) as Image;
		projectile.transform.SetParent (this.canvas.transform, false);

	}

}

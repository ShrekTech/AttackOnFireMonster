using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Image))]
public class HealthBar : MonoBehaviour {

	public Enemy enemy;
	private Image healthBarImage;
	private float initialFillAmount;

	void Start() {
		this.healthBarImage = GetComponent<Image> ();
		this.initialFillAmount = this.healthBarImage.fillAmount;
	}

	void Update () {
		float healthPercentage = (float)enemy.currentHitPoints / (float)enemy.maxHitPoints;
		this.healthBarImage.fillAmount = healthPercentage * initialFillAmount;
	}
}

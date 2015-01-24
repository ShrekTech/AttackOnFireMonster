using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour {

	public Player player;
	private Image healthBarImage;
	private float initialFillAmount;
	
	void Start() {
		this.healthBarImage = GetComponent<Image> ();
		this.initialFillAmount = this.healthBarImage.fillAmount;
	}
	
	void Update () {
		float healthPercentage = (float)player.currentHitPoints / (float)player.maxHitPoints;
		this.healthBarImage.fillAmount = healthPercentage * initialFillAmount;
	}
}

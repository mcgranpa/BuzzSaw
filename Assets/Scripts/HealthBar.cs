using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
	public Player Player;
	public Transform ForegoundSprite;
	public SpriteRenderer ForegroundRenderer;
	public Color MaxHealthColor = new Color (255 /255f, 63 / 266f , 63/255f);
	public Color MinHealthColor = new Color (64 /255f, 137 / 266f , 255/255f);
			
	// Update is called once per frame
	public void Update ()
	{
		var healthPerenct = Player.Health / (float)Player.MaxHealth;
	
		ForegoundSprite.localScale = new Vector3 (healthPerenct, 1, 1);
		ForegroundRenderer.color = Color.Lerp (MaxHealthColor, MinHealthColor, healthPerenct);
	}
}


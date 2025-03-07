using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{   
    public Text healthText;
    public float health = 100;
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Здоровье игрока: " + health);
        healthText.text = "Здоровье: " + health;
    }
}
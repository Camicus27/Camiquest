using UnityEngine;
using UnityEngine.UI;

public class GameOverStatistics : MonoBehaviour
{
    public Text health;
    public Text mana;
    public Text strength;
    public Text toughness;
    public Text gold;
    public Text XP;
    public Text dmgGiven;
    public Text dmgReceived;

    public void UpdateAllStats()
    {
        health.text = "Maximum HP: " + GameManager.instance.maxHealth;
        mana.text = "Maximum Mana: " + GameManager.instance.maxMana;
        strength.text = "Total Strength: " + GameManager.instance.player.strength;
        toughness.text = "Total Toughness: " + GameManager.instance.player.toughness;
        gold.text = "Total Gold: " + GameManager.instance.totalGold;
        XP.text = "Total XP: " + GameManager.instance.totalXP;
        dmgGiven.text = "Total Damage Given:\n" + GameManager.instance.totalDmgGiven;
        dmgReceived.text = "Total Damage Received:\n" + GameManager.instance.totalDmgReceived;
    }
}

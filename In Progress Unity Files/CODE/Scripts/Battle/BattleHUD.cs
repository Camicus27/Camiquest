using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    private GameObject fighter;
    public bool isPlayer;
    public Text nameText;
    public Text levelText;
    public Text hpText;
    public Text mpText;
    public RectTransform hpBar;
    private float barAnimateSpeed = 0.01f;

    public void Setup(GameObject enemyObj)
    {
        if (isPlayer)
        {
            // Find current player
            fighter = GameObject.Find("Player");
            UpdateHUD();
        }
        else
        {
            fighter = enemyObj;
            UpdateHUD();
        }
    }

    public void UpdateHUD()
    {
        if (isPlayer)
        {
            // Set text values
            nameText.text = GameManager.instance.playerName;
            levelText.text = "Level: " + GameManager.instance.level;
            hpText.text = GameManager.instance.health + "/" + GameManager.instance.maxHealth;
            mpText.text = GameManager.instance.MP + "/" + GameManager.instance.maxMana;
            // Set hpBar scale
            hpBar.localScale = new Vector3((float)GameManager.instance.health / (float)GameManager.instance.maxHealth, 1, 1);
        }
        else
        {
            // Set text values
            nameText.text = fighter.GetComponent<Enemy>().enemyName;
            levelText.text = "Level: " + fighter.GetComponent<Enemy>().level;
            hpText.text = fighter.GetComponent<Enemy>().health + "/" + fighter.GetComponent<Enemy>().maxHealth;
            // Set hpBar scale
            hpBar.localScale = new Vector3((float)fighter.GetComponent<Enemy>().health / (float)fighter.GetComponent<Enemy>().maxHealth, 1, 1);
        }

        // Update text colors
        GetHPColor();
        GetMPColor();
    }

    public IEnumerator UpdateHP(float currentHP)
    {
        float newHP;
        if (isPlayer)
        {
            newHP = (float)GameManager.instance.health;
            float changeAmount = currentHP - newHP;

            // If removing hp
            if (changeAmount > 0)
            {
                while (currentHP - newHP > Mathf.Epsilon)
                {
                    currentHP -= changeAmount * barAnimateSpeed;
                    // Set text value
                    hpText.text = Mathf.FloorToInt(currentHP) + "/" + GameManager.instance.maxHealth;
                    // Set hpBar scale
                    hpBar.localScale = new Vector3(currentHP / (float)GameManager.instance.maxHealth, 1, 1);
                    GetHPColor();
                    yield return null;
                }
            }
            // If restoring hp
            else
            {
                while (currentHP - newHP + 0.075f <= Mathf.Epsilon)
                {
                    currentHP += -(changeAmount) * barAnimateSpeed;
                    // Set text value
                    hpText.text = Mathf.FloorToInt(currentHP) + "/" + GameManager.instance.maxHealth;
                    // Set hpBar scale
                    hpBar.localScale = new Vector3(currentHP / (float)GameManager.instance.maxHealth, 1, 1);
                    GetHPColor();
                    yield return null;
                }
            }

            // Set text value
            hpText.text = GameManager.instance.health + "/" + GameManager.instance.maxHealth;
            // Set hpBar scale
            hpBar.localScale = new Vector3(newHP / (float)GameManager.instance.maxHealth, 1, 1);
        }
        else
        {
            float maxHP = (float)fighter.GetComponent<Enemy>().maxHealth;
            newHP = (float)fighter.GetComponent<Enemy>().health;
            float changeAmount = currentHP - newHP;

            // If removing hp
            if (changeAmount > 0)
            {
                while (currentHP - newHP > Mathf.Epsilon)
                {
                    currentHP -= changeAmount * barAnimateSpeed;
                    // Set text value
                    hpText.text = Mathf.FloorToInt(currentHP) + "/" + maxHP;
                    // Set hpBar scale
                    hpBar.localScale = new Vector3(currentHP / maxHP, 1, 1);
                    GetHPColor();
                    yield return null;
                }
            }
            // If restoring hp
            else
            {
                while ((currentHP - newHP) + 0.075f <= Mathf.Epsilon)
                {
                    currentHP += -(changeAmount) * barAnimateSpeed;
                    // Set text value
                    hpText.text = Mathf.FloorToInt(currentHP) + "/" + maxHP;
                    // Set hpBar scale
                    hpBar.localScale = new Vector3(currentHP / maxHP, 1, 1);
                    GetHPColor();
                    yield return null;
                }
            }

            // Set text value
            hpText.text = fighter.GetComponent<Enemy>().health + "/" + maxHP;
            // Set hpBar scale
            hpBar.localScale = new Vector3(newHP / maxHP, 1, 1);
        }

        GetHPColor();
    }

    private void GetHPColor()
    {
        // Set hp text color based on value
        if (hpBar.localScale.x > 0.25f)
            hpText.color = new Color(0.0373567f, 0.2264151f, 0f);
        else
            hpText.color = new Color(0.6627451f, 0f, 0.03921569f);
    }

    public IEnumerator UpdateMP(float currentMP)
    {
        float newMP = (float)GameManager.instance.MP;
        float changeAmount = currentMP - newMP;

        // If removing mana
        if (changeAmount > 0)
        {
            while (currentMP - newMP > Mathf.Epsilon)
            {
                currentMP -= changeAmount * barAnimateSpeed;
                // Set text value
                mpText.text = Mathf.FloorToInt(currentMP) + "/" + GameManager.instance.maxMana;
                GetMPColor();
                yield return null;
            }
        }
        // If restoring mana
        else
        {
            while (currentMP - newMP + 0.075f <= Mathf.Epsilon)
            {
                currentMP += -(changeAmount) * barAnimateSpeed;
                // Set text value
                mpText.text = Mathf.FloorToInt(currentMP) + "/" + GameManager.instance.maxMana;
                GetMPColor();
                yield return null;
            }
        }

        // Set final text value
        mpText.text = GameManager.instance.MP + "/" + GameManager.instance.maxMana;
    }

    private void GetMPColor()
    {
        double currentMP = (double)(GameManager.instance.MP) / GameManager.instance.maxMana;
        // Set mp text color based on value
        if (currentMP > 0.5)
            mpText.color = new Color(0f, 0.2914259f, 0.5283019f);
        else if (currentMP <= 0.5f && currentMP > 0.2f)
            mpText.color = new Color(0.099f, 0.103487f, 0.4528302f);
        else if (currentMP <= 0.2f)
            mpText.color = new Color(0.554f, 0.09803922f, 0.3422876f);
    }
}

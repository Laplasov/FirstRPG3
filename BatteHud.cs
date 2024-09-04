using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BatteHud : MonoBehaviour
{
    [SerializeField] private GameObject Image;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text defenceText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text skillPointsText;
    [SerializeField] private GameObject Esc;


    private void Start()
    {
        Image = transform.GetChild(0).gameObject;
        Image.SetActive(false);
        Esc.SetActive(false);

    }
    public void SetHUD(Unit unit)
    {
        BatteHud hudInstance = FindObjectOfType<BatteHud>();
        if (hudInstance != null && hudInstance.Image != null) { 
        
        hudInstance.nameText.text = unit.unitName;
        hudInstance.levelText.text = "Lvl " + unit.unitLevel;
        hudInstance.hpText.text = "HP " + unit.currentHealth + "/" + unit.maxHealth;
        hudInstance.hpSlider.maxValue = unit.maxHealth;
        hudInstance.hpSlider.value = unit.currentHealth;
        hudInstance.defenceText.text = "DEF " + unit.unitDefence;
        hudInstance.attackText.text = "ATK " + unit.unitAttack;
        hudInstance.skillPointsText.text = "SP " + unit.unitSkillPoints;
        }
    }

    public static void HudVisible()
    {
        BatteHud hudInstance = FindObjectOfType<BatteHud>();
        if (hudInstance != null && hudInstance.Image != null)
        {
            hudInstance.Image.SetActive(true);
        }
    }

    public static void HudInVisible()
    {
        BatteHud hudInstance = FindObjectOfType<BatteHud>();
        if (hudInstance != null && hudInstance.Image != null)
        {
            hudInstance.Image.SetActive(false);
        }
    }
    public void EscOn()
    {
        Esc.SetActive(true);
    }
    public void EscOff()
    {
        Esc.SetActive(false);
    }
}

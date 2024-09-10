using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class BatteHud : MonoBehaviour
{
    [SerializeField] private GameObject Image;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private UnityEngine.UI.Slider hpSlider;
    [SerializeField] private TMP_Text defenceText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text skillPointsText;
    [SerializeField] private GameObject Esc;
    [SerializeField] private GameObject hudObject;

    public RectTransform m_parant;
    public RectTransform m_image;
    public Camera m_camera;
    public Canvas m_canvas;
    private Rect canvasBounds;



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

    public void UpdatePosition()
    {
        Vector3 anchoredPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            m_parant, 
            Input.mousePosition, 
            m_canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : m_camera, 
            out anchoredPos);

        m_image.anchoredPosition = anchoredPos * 100f;
        m_image.anchoredPosition = new Vector3(
            m_image.anchoredPosition.x < 0 ? m_image.anchoredPosition.x + 250f : m_image.anchoredPosition.x - 250f,
            m_image.anchoredPosition.y < 0 ? m_image.anchoredPosition.y + 150f : m_image.anchoredPosition.y - 150f,
            0
            );
    }

}

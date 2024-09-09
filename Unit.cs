using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int unitAttack;
    public int unitDefence;
    public int unitModDefence;
    public int unitSkillPoints;
    public int maxHealth;
    public int currentHealth;
    public string loyalty;
    private new Renderer renderer;
    private GameObject playerHud;
    private BatteHud hud;
    private bool unitChoosen;
    private BattleLogic battleLogic;
    private bool unitDisable;
    private bool _unitSelfChosen;


    void Start()
    {
        unitLevel = Random.Range(1, 5);
        unitAttack = Random.Range(1 + unitLevel, 10 + unitLevel);
        maxHealth = Random.Range(30, 45);
        currentHealth = Random.Range(25, maxHealth);
        unitDefence = Random.Range(2, 8);
        unitSkillPoints = Random.Range(1, 5) + unitLevel;
        unitChoosen = false;
        unitDisable = false;
        unitModDefence = 0;

        renderer = GetComponent<Renderer>();
        hud = FindObjectOfType<BatteHud>();
        battleLogic = FindObjectOfType<BattleLogic>();
        renderer.material.color = Color.white;

    }

    private void OnMouseEnter()
    {
        if (battleLogic.status == BattleStates.START) { return; }

        if (battleLogic.status == BattleStates.PLAYERTURN && loyalty == "Ally" && !unitDisable)
        {
            if (!unitChoosen)
            {
                renderer.material.color = Color.red;
            }
        }
        if (battleLogic.status == BattleStates.CHOOSE && loyalty == "Foe")
        {
            renderer.material.color = Color.red;
        }
        BatteHud.HudVisible();
        hud.SetHUD(this);

    }

    private void OnMouseExit()
    {
        if (battleLogic.status == BattleStates.START) { return; }

        if (!unitChoosen && !unitDisable) 
        { 
            renderer.material.color = Color.white;
            BatteHud.HudInVisible();
        }
        if (!unitChoosen && battleLogic.status == BattleStates.CHOOSE && loyalty == "Foe")
        {
            renderer.material.color = Color.white;
            BatteHud.HudInVisible();
        }

    }
    private void OnMouseDown()
    {
        if (battleLogic.status == BattleStates.START) { return; }

        if (battleLogic.status == BattleStates.PLAYERTURN && loyalty == "Ally" && !unitDisable)
        {
            hud.EscOn();
            unitChoosen = true;
            battleLogic.status = BattleStates.CHOOSE;
            battleLogic.choosenAlly = this;
            battleLogic.dropAndActionContainer.SetActive(true);
            StartCoroutine(WaitForEscape());
        }

        if (battleLogic.status == BattleStates.CHOOSE && loyalty == "Foe" && !DropdownValueBuff())
        {
            battleLogic.choosenFoe = this;
            unitChoosen = true;
            battleLogic.status = BattleStates.WAITACTION;
            StartCoroutine(WaitForEscape());
        }

    }

    private IEnumerator WaitForEscape()
    {
        IsUnitSelfChosen();
        while (unitChoosen)
        {
            if (renderer.material.color != Color.green && IsUnitSelfChosen())
            {
                renderer.material.color = Color.green;
            } 
            else if (renderer.material.color != Color.red && !DropdownValueBuff())
            {
                renderer.material.color = Color.red;
            }

            yield return null;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                unitChoosen = false;
                battleLogic.status = BattleStates.PLAYERTURN;

                battleLogic.dropAndActionContainer.SetActive(false);

                renderer.material.color = Color.white;
                break;
            }
        }
        hud.EscOff();
    }

    public void OnChooseAll()
    {
        unitChoosen = false;
    }

    public void OnUnitDisable()
    {
        if (loyalty == "Ally")
        {
            unitDisable = true;
            renderer.material.color = Color.gray;
        }
        if (loyalty == "Foe")
        {
            unitDisable = true;
            renderer.material.color = Color.white;
        }

    }
    public void OnEndTurn()
    {
        unitChoosen = false;
        unitDisable = false;
        renderer.material.color = Color.white;
    }
    public void InTargeted()
    {
        renderer.material.color = Color.red;
    }
    public void OutTargeted()
    {
        renderer.material.color = Color.white;
    }
    public bool IsUnitSelfChosen()
    {
        if (loyalty == "Ally" &&
        (battleLogic.actionTypeDropdown.value == 1 ||
        battleLogic.actionTypeDropdown.value == 2))
        {
             return true;
        } 
        else
        {
            return false;
        }
    }
    public bool DropdownValueBuff()
    {
        if (battleLogic.actionTypeDropdown.value == 1 ||
            battleLogic.actionTypeDropdown.value == 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

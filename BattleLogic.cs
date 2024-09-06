using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.VersionControl.Asset;

public enum BattleStates { START, PLAYERTURN, CHOOSE, WAITACTION, ENEMYTURN, WON, LOST }

public class BattleLogic : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public BattleStates status;

    public Unit[] allyUnits;
    public Unit[] enemyUnits;

    public TMP_Text dialogueText;

    public Unit choosenAlly;
    public Unit choosenFoe;
    public TMP_Dropdown actionTypeDropdown;
    public GameObject dropAndActionContainer;

    private void Start()
    {
        allyUnits = new Unit[6];
        enemyUnits = new Unit[6];

        var allyStations = GameObject.FindGameObjectsWithTag("AllyStation");
        var foeStations = GameObject.FindGameObjectsWithTag("FoeStation");

        SpawnEnemies(enemyUnits, foeStations);
        SpawnAllies(allyUnits, allyStations);

        status = BattleStates.START;
        actionTypeDropdown.value = 0;

        StartCoroutine(SetupBattle());

    }
    private void SpawnEnemies(Unit[] enemyUnits, GameObject[] stations)
    {
        int i = 0;
        foreach (var station in stations)
        {
            GameObject enemyGo = Instantiate(enemyPrefab, station.transform);
            enemyUnits[i] = enemyGo.GetComponent<Unit>();
            i++;
        }
    }
    private void SpawnAllies(Unit[] allyUnits, GameObject[] stations)
    {
        int i = 0;
        foreach (var station in stations)
        {
            GameObject allyGo = Instantiate(playerPrefab, station.transform);
            allyUnits[i] = allyGo.GetComponent<Unit>();
            allyGo.GetComponent<Unit>();
            i++;
        }
    }

    IEnumerator SetupBattle()
    {
        dialogueText.text = "Battle begins, there is " + enemyUnits.Length + " enemy units named " + enemyUnits[0].unitName;

        yield return new WaitForSeconds(2f);

        status = BattleStates.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        dialogueText.text = "Your turn";

    }
    public void OnActionButton()
    {

        if (choosenAlly != null && choosenFoe != null && 
            ((status == BattleStates.CHOOSE) || (status == BattleStates.WAITACTION)))
        {

            switch (actionTypeDropdown.value)
            {
                case 0:
                    if (status != BattleStates.WAITACTION) { return; }
                        StartCoroutine(PlayerAttack());
                    break;
                case 1:
                    StartCoroutine(PlayerDefence());
                    break;
                case 2:
                    StartCoroutine(PlayerHeal());
                    break;
                default:
                    return;
            }

            dropAndActionContainer.SetActive(false);
        } 
        else 
        { 
            return;
        }

        status = BattleStates.PLAYERTURN;
    }

    IEnumerator PlayerAttack()
    {
        int damage;
        if (choosenAlly.unitAttack <= choosenFoe.unitDefence)
        {
            damage = 0;
        }
        else 
        {
            damage = choosenAlly.unitAttack - choosenFoe.unitDefence;
        }

        choosenFoe.currentHealth = choosenFoe.currentHealth - damage;

        CleanUpDisable();

        dialogueText.text = "Attack is successful!";
        yield return new WaitForSeconds(2f);
    }

    IEnumerator PlayerDefence()
    {
        choosenAlly.unitDefence = choosenAlly.unitDefence + 5;

        CleanUpDisable();

        dialogueText.text = "You are blocking!";
        yield return new WaitForSeconds(2f);
    }
    IEnumerator PlayerHeal()
    {
        choosenAlly.currentHealth = Mathf.Min(choosenAlly.currentHealth + 10, choosenAlly.maxHealth);

        CleanUpDisable();

        dialogueText.text = "Health restored!";
        yield return new WaitForSeconds(2f);
    }


    public void OnEndTurnButton()
    {
        if (status != BattleStates.PLAYERTURN && status != BattleStates.CHOOSE && status != BattleStates.WAITACTION)
        {
            return;
        }

        EndTurnCleanUp();

        status = BattleStates.ENEMYTURN;

        StartCoroutine(EnemyTurn());
        status = BattleStates.PLAYERTURN;

    }
    IEnumerator EnemyTurn()
    {

        foreach (Unit enemy in enemyUnits)
        {
            Unit allyTargeted = allyUnits[Random.Range(0, allyUnits.Length - 1 )];

            int damage = Mathf.Max(enemy.unitAttack - allyTargeted.unitDefence, 0);
            allyTargeted.currentHealth -= damage;


        }

        yield return new WaitForSeconds(2f);
    }

    public void CleanUpDisable()
    {
        choosenAlly.OnChooseAll();
        choosenAlly.OnUnitDisable();

        choosenFoe.OnChooseAll();
        choosenFoe.OnUnitDisable();

        choosenAlly = null;
        choosenFoe = null;
    }

    public void EndTurnCleanUp()
    {
        foreach(var unit in allyUnits)
        {
            unit.OnEndTurn();
        }
        foreach (var unit in enemyUnits)
        {
            unit.OnEndTurn();
        }

    }

}
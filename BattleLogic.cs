using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    private void Start()
    {
        allyUnits = new Unit[6];
        enemyUnits = new Unit[6];

        var allyStations = GameObject.FindGameObjectsWithTag("AllyStation");
        var foeStations = GameObject.FindGameObjectsWithTag("FoeStation");

        SpawnEnemies(enemyUnits, foeStations);
        SpawnAllies(allyUnits, allyStations);

        status = BattleStates.START;

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

        if (choosenAlly != null && choosenFoe != null && status == BattleStates.WAITACTION)
        {
            StartCoroutine(PlayerAttack());
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

    public void OnEndTurnButton()
    {
        if (status != BattleStates.PLAYERTURN || status != BattleStates.CHOOSE)
        {
            return;
        }

        status = BattleStates.ENEMYTURN;
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

}
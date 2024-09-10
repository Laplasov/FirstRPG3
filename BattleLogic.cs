using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleStates { START, PLAYERTURN, CHOOSE, WAITACTION, ENEMYTURN, WON, LOST }

public class BattleLogic : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public BattleStates status;

    public List<Unit> allyUnits;
    public List<Unit> enemyUnits;

    public TMP_Text dialogueText;

    public Unit choosenAlly;
    public Unit choosenFoe;
    public TMP_Dropdown actionTypeDropdown;
    public GameObject dropAndActionContainer;

    private void Start()
    {
        allyUnits = new List<Unit>();
        enemyUnits = new List<Unit>();

        var allyStations = GameObject.FindGameObjectsWithTag("AllyStation");
        var foeStations = GameObject.FindGameObjectsWithTag("FoeStation");

        SpawnEnemies(enemyUnits, foeStations);
        SpawnAllies(allyUnits, allyStations);

        status = BattleStates.START;
        actionTypeDropdown.value = 0;

        StartCoroutine(SetupBattle());

    }
    private void SpawnEnemies(List<Unit> enemyUnits, GameObject[] stations)
    {
        foreach (var station in stations)
        {
            GameObject enemyGo = Instantiate(enemyPrefab, station.transform);
            enemyUnits.Add(enemyGo.GetComponent<Unit>());
        }
    }
    private void SpawnAllies(List<Unit> allyUnits, GameObject[] stations)
    {
        foreach (var station in stations)
        {
            GameObject allyGo = Instantiate(playerPrefab, station.transform);
            allyUnits.Add(allyGo.GetComponent<Unit>());
        }
    }

    IEnumerator SetupBattle()
    {
        dialogueText.text = "Battle begins, there is " + enemyUnits.Count + " enemy units named " + enemyUnits[0].unitName;

        yield return new WaitForSeconds(2f);

        PlayerTurn();
    }

    void PlayerTurn()
    {
        dialogueText.text = "Your turn";
        status = BattleStates.PLAYERTURN;
    }
    public void OnActionButton()
    {
        if (choosenAlly != null)
        {
            if (status == BattleStates.WAITACTION)
            {
                if (actionTypeDropdown.value == 0)
                {
                    StartCoroutine(PlayerAttack());
                }
            }
            if (status == BattleStates.CHOOSE && actionTypeDropdown.value == 0)
            {
                return;
            }

            if (status == BattleStates.CHOOSE || status == BattleStates.WAITACTION)
            {
                if (actionTypeDropdown.value == 1)
                {
                    StartCoroutine(PlayerDefence());
                }
                else if (actionTypeDropdown.value == 2)
                {
                    StartCoroutine(PlayerHeal());
                }
            }
        }
        else
        {
            return;
        }

        dropAndActionContainer.SetActive(false);

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

        yield return StartCoroutine(WaitForPlayerMovement());

        if (choosenFoe.currentHealth <= 0)
        {
            enemyUnits.Remove(choosenFoe);
            Destroy(choosenFoe.gameObject);
            dialogueText.text = "Unit destroed!";
        }
        else 
        {
            dialogueText.text = "Attack is successful!";
        }
        if (enemyUnits.Count == 0)
        {
            status = BattleStates.WON;
            dialogueText.text = "You have WON!";
            yield break;
        }

        CleanUpDisable();
        yield return new WaitForSeconds(2f);
    }

    IEnumerator WaitForPlayerMovement()
    {
        Vector3 startPosition = choosenAlly.transform.position;
        Vector3 endPosition = choosenFoe.transform.position;
        float time = 0f;
        float LerpSpeed = 0.01f;

        while (time < 1f)
        {
            choosenAlly.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            time += Time.deltaTime;
            yield return new WaitForSeconds(LerpSpeed);
        }

        time = 0f;
        while (time < 1f)
        {
            choosenAlly.transform.position = Vector3.Lerp(endPosition, startPosition, time);
            time += Time.deltaTime;
            yield return new WaitForSeconds(LerpSpeed);
        }

        yield return new WaitForSeconds(0.5f);
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
    }
    IEnumerator EnemyTurn()
    {
        foreach (Unit enemy in enemyUnits)
        {
            Unit allyTargeted = allyUnits[Random.Range(0, allyUnits.Count)];

            int damage = Mathf.Max(enemy.unitAttack - allyTargeted.unitDefence, 0);
            allyTargeted.currentHealth -= damage;
            
            enemy.InTargeted();
            allyTargeted.InTargeted();

            yield return StartCoroutine(WaitForEnemyMovement(enemy, allyTargeted));

            if (allyTargeted.currentHealth <= 0)
            {
                allyUnits.Remove(allyTargeted);
                Destroy(allyTargeted.gameObject);
            }

            yield return new WaitForSeconds(1f);

            enemy.OutTargeted();
            if (allyTargeted != null)
            {
                allyTargeted.OutTargeted();
            }

            if (allyUnits.Count == 0)
            {
                status = BattleStates.LOST;
                dialogueText.text = "You have LOST!";
                yield break;
            }
        }
        status = BattleStates.PLAYERTURN;

    }
    IEnumerator WaitForEnemyMovement(Unit enemy, Unit allyTargeted)
    {
        Vector3 startPosition = enemy.transform.position;
        Vector3 endPosition = allyTargeted.transform.position;
        float time = 0f;
        float LerpSpeed = 0.01f;

        while (time < 1f)
        {
            enemy.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            time += Time.deltaTime;
            yield return new WaitForSeconds(LerpSpeed);
        }

        time = 0f;
        while (time < 1f)
        {
            enemy.transform.position = Vector3.Lerp(endPosition, startPosition, time);
            time += Time.deltaTime;
            yield return new WaitForSeconds(LerpSpeed);
        }

        yield return new WaitForSeconds(0.5f);
    }



    public void CleanUpDisable()
    {
        if (choosenAlly != null)
        {
            choosenAlly.OnChooseAll();
            choosenAlly.OnUnitDisable();
        }

        if (choosenFoe != null)
        {
            choosenFoe.OnChooseAll();
            choosenFoe.OnUnitDisable();
        }

        choosenAlly = null;
        choosenFoe = null;
    }

    public void EndTurnCleanUp()
    {
        foreach (var unit in allyUnits)
        {
            unit.OnEndTurn();
        }
        foreach (var unit in enemyUnits)
        {
            unit.OnEndTurn();
        }
    }
}

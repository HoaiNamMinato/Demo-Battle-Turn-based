using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : MonoBehaviour
{
    private BattleManager BSM;

    public BaseMonster monster;
    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }
    public TurnState currentState;
    //progress bar
    private float current_cooldown = 0f;
    private float max_cooldown = 5f;
    private Vector3 startPosition;
    private bool actionStarted = false;
    public GameObject playerToAttack;
    private float animSpeed = 5f;
    // Use this for initialization
    void Start()
    {
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("Battle Manager").GetComponent<BattleManager>();
        startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TurnState.PROCESSING:
                UpdateProgressBar();
                break;
            case TurnState.CHOOSEACTION:
                ChooseAction();
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                break;
            case TurnState.ACTION:
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                break;
        }
    }

    void UpdateProgressBar()
    {
        current_cooldown += Time.deltaTime;
        if (current_cooldown >= max_cooldown)
        {
            currentState = TurnState.CHOOSEACTION;
        }
    }

    void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.attacker = monster.name;
        myAttack.type = "Monster";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.PlayerInBattle[Random.Range(0, BSM.PlayerInBattle.Count)];
        BSM.CollectAction(myAttack);
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;

        //animate the enemy near the player to attack
        Vector3 playerPosition = new Vector3(playerToAttack.transform.position.x + 1.5f, playerToAttack.transform.position.y, playerToAttack.transform.position.z);
        while (MoveTowardsEnemy(playerPosition)) { yield return null; }
        //wait abit
        yield return new WaitForSeconds(0.5f);
        //do damage

        //anime back to startposition
        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(startPosition)) { yield return null; }
        //remove this perform form the list in BSM
        BSM.PerformList.RemoveAt(0);
        BSM.battleState = BattleManager.PerformAction.WAIT;
        //reset BSM -> wait

        actionStarted = false;
        //reset this enemy state
        current_cooldown = 0f;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

}

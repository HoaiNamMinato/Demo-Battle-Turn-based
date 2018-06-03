using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    private BattleManager BSM;
    public BasePlayer player;
    public GameObject selector;
    public GameObject enemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPosition;
    public float animSpeed = 10f;

    //trang thai turn
    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }
    //turn hien tai
    public TurnState currentState;
    public Image ProgressBar;
    //progress bar
    private float current_cooldown = 0f;
    private float max_cooldown = 5f;


    void Start()
    {
        startPosition = this.transform.position;
        current_cooldown = Random.Range(0, 2.5f);
        selector.SetActive(false);
        BSM = GameObject.Find("Battle Manager").GetComponent<BattleManager>();
        //set turn state la trang thai processing
        currentState = TurnState.PROCESSING;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            //neu o trang thai processing
            case TurnState.PROCESSING:
                UpdateProgressBar();
                break;
            case TurnState.ADDTOLIST:
                BSM.PlayerToManager.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                //idle
                break;
                //trang thai thuc hien hanh dong
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
        float calc_cooldown = current_cooldown / max_cooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        if (current_cooldown >= max_cooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    private IEnumerator TimeForAction()
    {
        //neu action start true
        if (actionStarted)
        {
            yield break;
        }
        //neu dang false, chuyen thanh true
        actionStarted = true;

        //animate the enemy near the player to attack
        Vector3 enemyPosition = new Vector3(enemyToAttack.transform.position.x - 1.5f, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z);
        while (MoveTowardsEnemy(enemyPosition)) { yield return null; }
        //wait abit
        yield return new WaitForSeconds(0.5f);
        //do damage

        //anime back to startposition
        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(startPosition)) { yield return null; }
        //remove this perform form the list in BSM
        BSM.PerformList.RemoveAt(0);
        //reset BSM -> wait
        BSM.battleState = BattleManager.PerformAction.WAIT;
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

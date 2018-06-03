using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    //cac trang thai thuc hien 
    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION
    }

    public enum PlayerGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }


    public PlayerGUI playerInput;
    public PerformAction battleState;

    //tao 1 list chua cac player thuc hien hanh dong
    public List<GameObject> PlayerToManager = new List<GameObject>();
    private HandleTurn playerChoice;
    //1 list chua cac hanh dong can thuc hien
    public List<HandleTurn> PerformList = new List<HandleTurn>();
    //list player trong battle
    public List<GameObject> PlayerInBattle = new List<GameObject>();
    //list monster trong battle
    public List<GameObject> MonsterInBattle = new List<GameObject>();

    public GameObject enemyButton;
    public Transform spacer;

    public GameObject AttackPanel;
    public GameObject MonsterSelectPanel;

    // Use this for initialization
    void Start()
    {
        //set trang thai dau tien la Wait
        battleState = PerformAction.WAIT;
        //add toan bo cac gameobject co tag la enemy
        MonsterInBattle.AddRange(GameObject.FindGameObjectsWithTag("enemy"));
        //tag toan bo cac game object co tag la monster
        PlayerInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        //GUI player dc kich hoat
        playerInput = PlayerGUI.ACTIVATE;
        AttackPanel.SetActive(false);
        MonsterSelectPanel.SetActive(false);
        EnemyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleState)
        {
            //truong hop battle dang trong trang thai wait
            case PerformAction.WAIT:
                //neu list hanh dong can thuc hien > 0
                if (PerformList.Count > 0)
                {
                    //chuyen trang thai ve tackeaction
                    battleState = PerformAction.TAKEACTION;
                }
                break;
            //truong hop battle dang trong trang thai takeaction
            case PerformAction.TAKEACTION:
                //tim gameobject co ten la ten phan tu dau tien trong list thuc hien
                GameObject performer = GameObject.Find(PerformList[0].attacker);
                //neu ng thuc hien thuoc type monster
                if (PerformList[0].type == "Monster")
                {
                    //get component MSM cua no
                    MonsterStateMachine MSM = performer.GetComponent<MonsterStateMachine>();
                    
                    MSM.playerToAttack = PerformList[0].AttackersTarget;
                    //chuyen trang thai cua monster sang action
                    MSM.currentState = MonsterStateMachine.TurnState.ACTION;
                }
                else if (PerformList[0].type == "Player")
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.enemyToAttack = PerformList[0].AttackersTarget;
                    HSM.currentState = HeroStateMachine.TurnState.ACTION;
                }
                //chuyen trang thai cua battle sang perform action
                battleState = PerformAction.PERFORMACTION;
                break;
            case PerformAction.PERFORMACTION:
                break;
        }

        switch (playerInput)
        {
            case PlayerGUI.ACTIVATE:
                //neu player quan li > 0
                if (PlayerToManager.Count > 0)
                {
                    //tim gameobject selector va set active = true
                    PlayerToManager[0].transform.Find("selector").gameObject.SetActive(true);
                    //tao moi 1 handler turn
                    playerChoice = new HandleTurn();
                    //hien thi attack panel
                    AttackPanel.SetActive(true);
                    //chuyen trang thai playerGUI ve waiting
                    playerInput = PlayerGUI.WAITING;
                }
                break;
            case PlayerGUI.WAITING:
                break;
            case PlayerGUI.DONE:
                //thuc hien ham PlayerInputDone()
                PlayerInputDone();
                break;
        }
    }

    public void CollectAction(HandleTurn input)
    {
        //thu thap cac hanh dong can thuc hien va cho vao list
        PerformList.Add(input);
    }

    //button hien thi ten enemy de thuc hien chon lua
    void EnemyButtons()
    {
        //duyet list mosnter co trong battle
        foreach (GameObject monster in MonsterInBattle)
        {
            //tao 1 button
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            //get component enemyselectbutton cua newButton
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            //get MSM cua phan tu trong list 
            MonsterStateMachine current_monster = monster.GetComponent<MonsterStateMachine>();
            //get text cua new button
            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            //set text la ten cua phan tu monster hien tai
            buttonText.text = current_monster.monster.name;

            button.enemyPrefabs = monster;

            newButton.transform.SetParent(spacer, false);
        }
    }

    public void Input1() //attack button
    {
        //khi an nut attack, set thuoc tinh cua playerChoice
        playerChoice.attacker = PlayerToManager[0].name;
        playerChoice.AttackersGameObject = PlayerToManager[0];
        playerChoice.type = "Player";

        AttackPanel.SetActive(false);
        MonsterSelectPanel.SetActive(true);
    }

    public void Input2(GameObject choosenEnemy) //enemy selection
    {
        //lua chon enemy can tan cong 
        playerChoice.AttackersTarget = choosenEnemy;
        Debug.Log("perform atatck with " + choosenEnemy);
        //chuyen trang thai PlayerGUI thanh done
        playerInput = PlayerGUI.DONE;
    }

    //sau khi player lua chon
    void PlayerInputDone()
    {
        //add lua chon vao list thuc hien
        PerformList.Add(playerChoice);
        //an panel lua chon monster
        MonsterSelectPanel.SetActive(false);
        //an selector cua player
        PlayerToManager[0].transform.Find("selector").gameObject.SetActive(false);
        //remove no ra khoi list player can set hanh dong
        PlayerToManager.RemoveAt(0);
        //chuyen trang thai PlayerGUI ve activate
        playerInput = PlayerGUI.ACTIVATE;
    }
}

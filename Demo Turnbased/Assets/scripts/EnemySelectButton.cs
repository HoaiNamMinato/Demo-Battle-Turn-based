using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject enemyPrefabs;
    void Start()
    {

    }

    public void SelectEnemy() 
    {
        GameObject.Find("Battle Manager").GetComponent<BattleManager>().Input2(enemyPrefabs);

    }
}

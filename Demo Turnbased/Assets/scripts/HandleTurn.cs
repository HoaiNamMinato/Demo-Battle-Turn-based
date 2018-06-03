using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string attacker; //name attacker
    public string type;
    public GameObject AttackersGameObject; //who's attack
    public GameObject AttackersTarget; //who's going to be attacked
}

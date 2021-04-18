using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PerkBase : MonoBehaviour
{
    public virtual void OnNextTurn()
    { }
}
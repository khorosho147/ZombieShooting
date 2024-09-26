using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife_Collider : MonoBehaviour
{
    private Knife knife;
    private bool canAttack = false;
    private List<GameObject> targetList = new List<GameObject>();

    public void Init(Knife knife)
    {
        this.knife = knife;
    }

    public void StartHit()
    {
        canAttack = true;
    }

    public void StopHit()
    {
        canAttack = false;
        targetList.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if(!canAttack) return;
        //没打过这个敌人
        if(!targetList.Contains(other.gameObject))
        {
            targetList.Add(other.gameObject);
            //ClosestPoint两个碰撞体的交界点，括号里是武器自身
            knife.HitTarget(other.gameObject, other.ClosestPoint(transform.position));
        }
    }
}

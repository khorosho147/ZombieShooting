using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif
using UnityEngine;

public class Knife : WeaponBase
{
    [SerializeField] Knife_Collider knife_Collider;
    private bool isLeftAttack;
    public override void Init(Player_Controller player)
    {
        base.Init(player);
        knife_Collider.Init(this);
    }

    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Shoot:
                if(isLeftAttack)
                {
                    OnLeftAttack();
                }
                else
                {
                    OnRightAttack();
                }
                break;
            case PlayerState.Reload:
                PlayAudio(2);
                animator.SetBool("Reload", true);
                break;
        }
    }

    public override void OnUpdatePlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Move:
                //左键攻击
                if(canShoot && Input.GetMouseButton(0))
                {
                    isLeftAttack = true;
                    player.ChangePlayerState(PlayerState.Shoot);
                    return;
                }

                //右键攻击
                if (canShoot && Input.GetMouseButton(1))
                {
                    isLeftAttack = false;
                    player.ChangePlayerState(PlayerState.Shoot);
                    return;
                }

                break;

        }
    }

    public void HitTarget(GameObject hitObj,Vector3 efPos)
    {
        PlayAudio(2);
        //判定是不是攻击到了僵尸
        if (hitObj.CompareTag("Zombie"))
        {
            //命中效果
            GameObject go = Instantiate(prefab_BulletEF[1], efPos, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
            //僵尸的逻辑
            ZombieController zombie = hitObj.GetComponent<ZombieController>();
            //如果打到没有脚本的身体部位，从父物体身上找
            if (zombie == null) zombie = hitObj.GetComponentInParent<ZombieController>();
            zombie.Hurt(attackValue);
        }
        else if (hitObj != player.gameObject)
        {
            GameObject go = Instantiate(prefab_BulletEF[0], efPos, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
        }

    }

    protected override void OnLeftAttack()
    {
        PlayAudio(1);
        animator.SetTrigger("Shoot");
        animator.SetBool("IsLeft",true);
        knife_Collider.StartHit();
    }

    private void OnRightAttack()
    {
        PlayAudio(1);
        animator.SetTrigger("Shoot");
        animator.SetBool("IsLeft", false);
        knife_Collider.StartHit();

    }

    protected override void ShootOver()
    {
        base.ShootOver();
        knife_Collider.StopHit();
    }
}
 
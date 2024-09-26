using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手枪控制脚本
/// </summary>
public class Pistol : WeaponBase
{
    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Shoot:
                OnLeftAttack();
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
                //有可能要切换子弹
                //第一种情况：子弹打完了,但是要有备用子弹
                if (curr_BulletNum == 0 && standby_BulletNum > 0)
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }

                //第二种情况，子弹没有打完，但是玩家按了R键
                if(standby_BulletNum > 0 && Input.GetKeyDown(KeyCode.R))
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }


                //有可能要射击
                //当前没有在换子弹中
                //当前弹匣里面有子弹
                //按鼠标左键
                if (canShoot && curr_BulletNum > 0 &&Input.GetMouseButton(0))
                {
                    player.ChangePlayerState(PlayerState.Shoot);
                }

                break;

        }
    }
}

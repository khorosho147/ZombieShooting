using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ǹ���ƽű�
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
                //�п���Ҫ�л��ӵ�
                //��һ��������ӵ�������,����Ҫ�б����ӵ�
                if (curr_BulletNum == 0 && standby_BulletNum > 0)
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }

                //�ڶ���������ӵ�û�д��꣬������Ұ���R��
                if(standby_BulletNum > 0 && Input.GetKeyDown(KeyCode.R))
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }


                //�п���Ҫ���
                //��ǰû���ڻ��ӵ���
                //��ǰ��ϻ�������ӵ�
                //��������
                if (canShoot && curr_BulletNum > 0 &&Input.GetMouseButton(0))
                {
                    player.ChangePlayerState(PlayerState.Shoot);
                }

                break;

        }
    }
}

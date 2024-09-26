using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Sniper : WeaponBase
{
    [SerializeField] GameObject sightCanvas;
    [SerializeField] GameObject[] renders;

    //瞄准状态
    private bool isAim = false;


    public override void Enter()
    {

        if (isAim) StopAim();
        isAim = false ;
        base.Enter();

    }
    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Shoot:
                if (isAim) StopAim();
                isAim = false;

                OnLeftAttack();
                break;
            case PlayerState.Reload:
                if (isAim) StopAim();
                isAim = false;
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
                if (standby_BulletNum > 0 && Input.GetKeyDown(KeyCode.R))
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }


                //有可能要射击
                //当前没有在换子弹中
                //当前弹匣里面有子弹
                //按鼠标左键
                if (canShoot && curr_BulletNum > 0 && Input.GetMouseButton(0))
                {
                    player.ChangePlayerState(PlayerState.Shoot);
                }

                //开镜
                //开镜/关镜
                if(canShoot && Input.GetMouseButtonDown(1))
                {
                    isAim = !isAim;
                    if (isAim) StartAim();
                    else StopAim();
                }

                break;

        }
    }


    private void StartAim()
    {
        //播放动画
        animator.SetBool("Aim", true);
        //关闭火花效果
        wantShootEF = false;
    }


    private void StopAim()
    {
        //停止播放动画
        animator.SetBool("Aim", false);
        //开启火花效果
        wantShootEF = true;
        //打开所有的渲染器
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].SetActive(true);
        }

        //关闭狙击镜
        sightCanvas.SetActive(false);
        //还原镜头缩放
        player.SetCameraView(60);
    }

    #region 动画事件
    private void StartLoad()
    {
        PlayAudio(3);
    }

    private void AimOver()
    {
        StartCoroutine(DoAim());
    }

    IEnumerator DoAim()
    {
        //隐藏所有的渲染器
        for(int i = 0; i < renders.Length; i++)
        {
            renders[i].SetActive(false);
        }
        //停留一点时间
        yield return new WaitForSeconds(0.1f);
        //显示狙击镜
        sightCanvas.SetActive(true);
        //设置镜头缩放
        player.SetCameraView(30);
    }

    #endregion
}

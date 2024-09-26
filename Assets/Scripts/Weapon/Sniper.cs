using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Sniper : WeaponBase
{
    [SerializeField] GameObject sightCanvas;
    [SerializeField] GameObject[] renders;

    //��׼״̬
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
                //�п���Ҫ�л��ӵ�
                //��һ��������ӵ�������,����Ҫ�б����ӵ�
                if (curr_BulletNum == 0 && standby_BulletNum > 0)
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }

                //�ڶ���������ӵ�û�д��꣬������Ұ���R��
                if (standby_BulletNum > 0 && Input.GetKeyDown(KeyCode.R))
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }


                //�п���Ҫ���
                //��ǰû���ڻ��ӵ���
                //��ǰ��ϻ�������ӵ�
                //��������
                if (canShoot && curr_BulletNum > 0 && Input.GetMouseButton(0))
                {
                    player.ChangePlayerState(PlayerState.Shoot);
                }

                //����
                //����/�ؾ�
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
        //���Ŷ���
        animator.SetBool("Aim", true);
        //�رջ�Ч��
        wantShootEF = false;
    }


    private void StopAim()
    {
        //ֹͣ���Ŷ���
        animator.SetBool("Aim", false);
        //������Ч��
        wantShootEF = true;
        //�����е���Ⱦ��
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].SetActive(true);
        }

        //�رվѻ���
        sightCanvas.SetActive(false);
        //��ԭ��ͷ����
        player.SetCameraView(60);
    }

    #region �����¼�
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
        //�������е���Ⱦ��
        for(int i = 0; i < renders.Length; i++)
        {
            renders[i].SetActive(false);
        }
        //ͣ��һ��ʱ��
        yield return new WaitForSeconds(0.1f);
        //��ʾ�ѻ���
        sightCanvas.SetActive(true);
        //���þ�ͷ����
        player.SetCameraView(30);
    }

    #endregion
}

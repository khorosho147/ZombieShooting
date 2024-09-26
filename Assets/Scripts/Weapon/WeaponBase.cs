using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;



/// <summary>
/// ���е���ҵ���������
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource audioSource;
    protected Player_Controller player;


    #region ��ϻ�������
    protected int curr_BulletNum;      //��ǰ�ӵ�����
    public int curr_MaxBulletNum;   //��ǰ�ӵ�����
    protected int standby_BulletNum;   //�����ӵ�����
    public int standby_MaxBulletNum;//�����ӵ�����
    #endregion


    #region �������
    public int attackValue;         //������
    public bool wantCrosshair;      //�Ƿ���׼��
    public bool wantBullet;         //�Ƿ����ӵ�
    public bool wantShootEF;        //�Ƿ��������Ч
    public bool wantRecoil;         //�Ƿ���Ҫ������
    public float recoilStrength;    //������ǿ��
    public bool canThroughWall;     //�����ǽ


    protected bool canShoot = false;
    private bool wantReloadOnEnter = false;
    #endregion

    #region Ч��
    [SerializeField] AudioClip[] audioClips;        //�õ���������Ч
    [SerializeField] protected GameObject[] prefab_BulletEF;  //�ӵ�����Ч��
    [SerializeField] protected GameObject shootEF;            //�����

    #endregion

    public virtual void Init(Player_Controller player)
    {
        this.player = player;
        //��ʼ���ӵ�
        curr_BulletNum = curr_MaxBulletNum;
        standby_BulletNum = standby_MaxBulletNum;

    }

    public abstract void OnEnterPlayerState(PlayerState playerState);

    public abstract void OnUpdatePlayerState(PlayerState playerState);

    /// <summary>
    /// ����л�����ǰ����
    /// </summary>
    public virtual void Enter()
    {
        canShoot = false;

        //��ʼ���������Ƿ���Ҫ�ӵ���UI׼�ǵ�
        player.InitForEnterWeapon(wantCrosshair, wantBullet);
        //�����ӵ�����
        if(wantBullet)
        {
            player.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
            if (curr_BulletNum > 0)
            {
                PlayAudio(0);
            }
            else 
            {
                //�����ʱ���Ҫ�����ӵ�
                wantReloadOnEnter = true;
            }
        }

        //����һЩ״��
        if(shootEF!=null) shootEF.SetActive(false);


        gameObject.SetActive(true);
    }

    private Action onExitOver;
    /// <summary>
    /// �˳��������
    /// </summary>
    public virtual void Exit(Action onExitOver)
    {
        animator.SetTrigger("Exit");
        this.onExitOver = onExitOver;
        player.ChangePlayerState(PlayerState.Move);
    }

    protected virtual void OnLeftAttack()
    {
        //�����ӵ�
        if(wantBullet)
        {
            curr_BulletNum--;
            player.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        }
        canShoot = false;
        //�����������
        animator.SetTrigger("Shoot");
        //��
        if (wantShootEF) shootEF.SetActive(true);
        //������
        if(wantRecoil) player.StartShootRecoil(recoilStrength);

        //��Ч
        PlayAudio(1);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        //��ǽ
        if (canThroughWall)
        {
            RaycastHit[] raycastHits = Physics.RaycastAll(ray,1500f);
            for (int i = 0; i < raycastHits.Length; i++)
            {
                    HitGameObject(raycastHits[i]);
            }
        }
        else
        {
            //�˺��ж�
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1500f))
            {
                HitGameObject(hitInfo);
            }
        }

        


    }

    private void HitGameObject(RaycastHit hitInfo)
    {
        //�ж��ǲ��ǹ������˽�ʬ
        if (hitInfo.collider.gameObject.CompareTag("Zombie"))
        {
            //����Ч��
            GameObject go = Instantiate(prefab_BulletEF[1], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
            //��ʬ���߼�
            ZombieController zombie = hitInfo.collider.gameObject.GetComponent<ZombieController>();
            //�����û�нű������岿λ���Ӹ�����������
            if(zombie == null) zombie = hitInfo.collider.gameObject.GetComponentInParent<ZombieController>();
            zombie.Hurt(attackValue);
        }
        else if(hitInfo.collider.gameObject !=player.gameObject)
        {
            GameObject go = Instantiate(prefab_BulletEF[0], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
        }
    }


    protected void PlayAudio(int index)
    {
        gameObject.SetActive(true);
        audioSource.PlayOneShot(audioClips[index]);

    }


    #region �����¼�
    private void EnterOver()
    {
        canShoot = true;
        if (wantReloadOnEnter)
        {
            player.ChangePlayerState(PlayerState.Reload);
        }
    }
    private void ExitOver()
    {
        gameObject.SetActive(false);
        onExitOver?.Invoke();
        #region
        //��ͬ������
        //if(onExitOver != null)
        //{
        //    onExitOver.Invoke();
        //}
        #endregion
    }

    protected virtual void ShootOver()
    {
        canShoot = true;
        if (wantShootEF) shootEF.SetActive(false);
        if(player.PlayerState == PlayerState.Shoot)
        {
            player.ChangePlayerState(PlayerState.Move);
        }
    }

    private void ReloadOver()
    {
        //����ӵ�
        int want = curr_MaxBulletNum - curr_BulletNum;
        if (standby_BulletNum - want < 0) 
        {
            want = standby_BulletNum;
        }
        standby_BulletNum -= want;
        curr_BulletNum += want;
        //����UI
        player.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        animator.SetBool("Reload", false);
        player.ChangePlayerState(PlayerState.Move);
    }

    #endregion
}

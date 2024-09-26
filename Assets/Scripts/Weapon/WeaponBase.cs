using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;



/// <summary>
/// 所有的玩家的武器基类
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource audioSource;
    protected Player_Controller player;


    #region 弹匣相关数据
    protected int curr_BulletNum;      //当前子弹数量
    public int curr_MaxBulletNum;   //当前子弹上限
    protected int standby_BulletNum;   //备用子弹数量
    public int standby_MaxBulletNum;//备用子弹上限
    #endregion


    #region 射击参数
    public int attackValue;         //攻击力
    public bool wantCrosshair;      //是否有准星
    public bool wantBullet;         //是否有子弹
    public bool wantShootEF;        //是否有射击特效
    public bool wantRecoil;         //是否需要后坐力
    public float recoilStrength;    //后坐力强度
    public bool canThroughWall;     //射击穿墙


    protected bool canShoot = false;
    private bool wantReloadOnEnter = false;
    #endregion

    #region 效果
    [SerializeField] AudioClip[] audioClips;        //用到的所有音效
    [SerializeField] protected GameObject[] prefab_BulletEF;  //子弹命中效果
    [SerializeField] protected GameObject shootEF;            //射击火花

    #endregion

    public virtual void Init(Player_Controller player)
    {
        this.player = player;
        //初始化子弹
        curr_BulletNum = curr_MaxBulletNum;
        standby_BulletNum = standby_MaxBulletNum;

    }

    public abstract void OnEnterPlayerState(PlayerState playerState);

    public abstract void OnUpdatePlayerState(PlayerState playerState);

    /// <summary>
    /// 玩家切换到当前武器
    /// </summary>
    public virtual void Enter()
    {
        canShoot = false;

        //初始化武器，是否需要子弹，UI准星等
        player.InitForEnterWeapon(wantCrosshair, wantBullet);
        //更新子弹数量
        if(wantBullet)
        {
            player.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
            if (curr_BulletNum > 0)
            {
                PlayAudio(0);
            }
            else 
            {
                //进入的时候就要更换子弹
                wantReloadOnEnter = true;
            }
        }

        //重置一些状况
        if(shootEF!=null) shootEF.SetActive(false);


        gameObject.SetActive(true);
    }

    private Action onExitOver;
    /// <summary>
    /// 退出这把武器
    /// </summary>
    public virtual void Exit(Action onExitOver)
    {
        animator.SetTrigger("Exit");
        this.onExitOver = onExitOver;
        player.ChangePlayerState(PlayerState.Move);
    }

    protected virtual void OnLeftAttack()
    {
        //更新子弹
        if(wantBullet)
        {
            curr_BulletNum--;
            player.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        }
        canShoot = false;
        //播放射击动画
        animator.SetTrigger("Shoot");
        //火花
        if (wantShootEF) shootEF.SetActive(true);
        //后坐力
        if(wantRecoil) player.StartShootRecoil(recoilStrength);

        //音效
        PlayAudio(1);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        //穿墙
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
            //伤害判定
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1500f))
            {
                HitGameObject(hitInfo);
            }
        }

        


    }

    private void HitGameObject(RaycastHit hitInfo)
    {
        //判定是不是攻击到了僵尸
        if (hitInfo.collider.gameObject.CompareTag("Zombie"))
        {
            //命中效果
            GameObject go = Instantiate(prefab_BulletEF[1], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
            //僵尸的逻辑
            ZombieController zombie = hitInfo.collider.gameObject.GetComponent<ZombieController>();
            //如果打到没有脚本的身体部位，从父物体身上找
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


    #region 动画事件
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
        //等同于下面
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
        //填充子弹
        int want = curr_MaxBulletNum - curr_BulletNum;
        if (standby_BulletNum - want < 0) 
        {
            want = standby_BulletNum;
        }
        standby_BulletNum -= want;
        curr_BulletNum += want;
        //更新UI
        player.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        animator.SetBool("Reload", false);
        player.ChangePlayerState(PlayerState.Move);
    }

    #endregion
}

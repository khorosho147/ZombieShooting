using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;



/// <summary>
/// Base class for all player weapons
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource audioSource;
    protected Player_Controller player;


    #region Magazine data
    protected int curr_BulletNum;      // Current bullet count
    public int curr_MaxBulletNum;      // Current max bullet count
    protected int standby_BulletNum;   // Standby bullet count
    public int standby_MaxBulletNum;   // Standby max bullet count
    #endregion


    #region Shooting parameters
    public int attackValue;         // Attack power
    public bool wantCrosshair;      // Has crosshair
    public bool wantBullet;         // Has bullets
    public bool wantShootEF;        // Has shooting effect
    public bool wantRecoil;         // Needs recoil
    public float recoilStrength;    // Recoil strength
    public bool canThroughWall;     // Can shoot through walls


    protected bool canShoot = false;
    private bool wantReloadOnEnter = false;
    #endregion

    #region Effect
    [SerializeField] AudioClip[] audioClips;        // All used audio clips
    [SerializeField] protected GameObject[] prefab_BulletEF;  // Bullet impact effects
    [SerializeField] protected GameObject shootEF;            // Muzzle flash effect

    #endregion

    public virtual void Init(Player_Controller player)
    {
        this.player = player;
        // Initialize bullets
        curr_BulletNum = curr_MaxBulletNum;
        standby_BulletNum = standby_MaxBulletNum;

    }

    public abstract void OnEnterPlayerState(PlayerState playerState);

    public abstract void OnUpdatePlayerState(PlayerState playerState);

    /// <summary>
    /// Player switches to the current weapon
    /// </summary>
    public virtual void Enter()
    {
        canShoot = false;

        //Initialize weapon, check if bullets, UI crosshair, etc. are needed
        player.InitForEnterWeapon(wantCrosshair, wantBullet);
        //Update bullet count
        if (wantBullet)
        {
            player.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
            if (curr_BulletNum > 0)
            {
                PlayAudio(0);
            }
            else 
            {
                //Reload bullets upon entering
                wantReloadOnEnter = true;
            }
        }

        //Reset some conditions
        if (shootEF!=null) shootEF.SetActive(false);


        gameObject.SetActive(true);
    }

    private Action onExitOver;
    /// <summary>
    /// Exit the current weapon
    /// </summary>
    public virtual void Exit(Action onExitOver)
    {
        animator.SetTrigger("Exit");
        this.onExitOver = onExitOver;
        player.ChangePlayerState(PlayerState.Move);
    }

    protected virtual void OnLeftAttack()
    {
        //Update bullets
        if (wantBullet)
        {
            curr_BulletNum--;
            player.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        }
        canShoot = false;
        //Play shooting animation
        animator.SetTrigger("Shoot");
        //Muzzle flash
        if (wantShootEF) shootEF.SetActive(true);
        //Recoil
        if (wantRecoil) player.StartShootRecoil(recoilStrength);

        //Sound effects
        PlayAudio(1);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        //Wall penetration
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
            //Damage detection
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1500f))
            {
                HitGameObject(hitInfo);
            }
        }

        


    }

    private void HitGameObject(RaycastHit hitInfo)
    {
        //Check if the zombie was hit
        if (hitInfo.collider.gameObject.CompareTag("Zombie"))
        {
            //Hit effect
            GameObject go = Instantiate(prefab_BulletEF[1], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
            //Zombie logic
            ZombieController zombie = hitInfo.collider.gameObject.GetComponent<ZombieController>();
            //If a non-scripted body part is hit, find the parent object
            if (zombie == null) zombie = hitInfo.collider.gameObject.GetComponentInParent<ZombieController>();
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


    #region Animation event
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
        //Equivalent to the following
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
        //Refill bullets
        int want = curr_MaxBulletNum - curr_BulletNum;
        if (standby_BulletNum - want < 0) 
        {
            want = standby_BulletNum;
        }
        standby_BulletNum -= want;
        curr_BulletNum += want;
        //Update UI
        player.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        animator.SetBool("Reload", false);
        player.ChangePlayerState(PlayerState.Move);
    }

    #endregion
}

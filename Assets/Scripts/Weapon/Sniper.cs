using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Sniper : WeaponBase
{
    [SerializeField] GameObject sightCanvas;
    [SerializeField] GameObject[] renders;

    //Aiming state
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
                //Might need to switch bullets
                //First case: Bullets are empty, but standby bullets are available
                if (curr_BulletNum == 0 && standby_BulletNum > 0)
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }

                // Second case: Bullets are not empty, but the player pressed the R key
                if (standby_BulletNum > 0 && Input.GetKeyDown(KeyCode.R))
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }


                // Might need to shoot
                // Currently not in reloading
                // There are bullets in the magazine
                // Left mouse button pressed
                if (canShoot && curr_BulletNum > 0 && Input.GetMouseButton(0))
                {
                    player.ChangePlayerState(PlayerState.Shoot);
                }

                //Zoom in / Zoom out
                if (canShoot && Input.GetMouseButtonDown(1))
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
        //Play animation
        animator.SetBool("Aim", true);
        //Disable muzzle flash effect
        wantShootEF = false;
    }


    private void StopAim()
    {
        //Stop playing animation
        animator.SetBool("Aim", false);
        //Enable muzzle flash effect
        wantShootEF = true;
        // Enable all renderers
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].SetActive(true);
        }

        //Disable scope
        sightCanvas.SetActive(false);
        //Restore camera view
        player.SetCameraView(60);
    }

    #region Animation event
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
        //Hide all renderers
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].SetActive(false);
        }
        //Stay for a moment
        yield return new WaitForSeconds(0.1f);
        //Show scope
        sightCanvas.SetActive(true);
        //Set camera view
        player.SetCameraView(30);
    }

    #endregion
}

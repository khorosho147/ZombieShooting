using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pistol control script
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
                if (canShoot && curr_BulletNum > 0 &&Input.GetMouseButton(0))
                {
                    player.ChangePlayerState(PlayerState.Shoot);
                }

                break;

        }
    }
}

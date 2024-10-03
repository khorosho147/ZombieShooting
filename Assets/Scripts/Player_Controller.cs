using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Player status
/// </summary>
public enum PlayerState
{
    Move,
    Shoot,
    Reload
}

public class Player_Controller : MonoBehaviour
{
    public static Player_Controller Instance;
    [SerializeField] FirstPersonController firstPersonController;


    // Crosshair
    [SerializeField] Image crossImage;

    [SerializeField] Camera[] cameras;

    #region Weapon related
    [SerializeField] WeaponBase[] weapons;
    private int currentWeaponIndex = -1;        // Current weapon
    private int previousWeaponIndex = -1;       // Previous weapon
    private bool canChangeWeapon = true;        // Can change weapon
    #endregion

    private int hp = 100;

    public PlayerState PlayerState;

    //Modify player status
    public void ChangePlayerState(PlayerState newState)
    {
        PlayerState = newState;
        // Perhaps some actions need to be taken when the weapon enters a certain state
        weapons[currentWeaponIndex].OnEnterPlayerState(newState);
    }

    private void Start()
    {
        Instance = this;
        // Initialize all weapons
        for (int i = 0; i< weapons.Length; i++)
        {
            weapons[i].Init(this);

        }
        PlayerState = PlayerState.Move;

        // Default to selecting the first weapon
        ChangeWeapon(0);
    }

    private void Update()
    {
        // Drive weapon layer
        weapons[currentWeaponIndex].OnUpdatePlayerState(PlayerState);

        // Detect key input to switch weapons
        if (canChangeWeapon == false) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(2);
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (previousWeaponIndex >= 0) ChangeWeapon(previousWeaponIndex);
        }
    }


    #region Recoil

    /// <summary>
    /// Activate shooting recoil
    /// </summary>
    /// <param name="recoil"></param>
    public void StartShootRecoil(float recoil = 1)
    {
        //Adjust sight magnification
        StartCoroutine(Shootrecoil_Cross(recoil));
        if(shootRecoil_CameraCoroutine != null) StopCoroutine(shootRecoil_CameraCoroutine);
        //Camera movement
        shootRecoil_CameraCoroutine = StartCoroutine(ShootRecoil_Camera(recoil));
    }

    //Recoil - Scope
    IEnumerator Shootrecoil_Cross(float recoil )
    {
        Vector2 scale = crossImage.transform.localScale;
        //Zoom in
        while (scale.x < 1.3f)
        {
            //Freeze frame
            yield return null;
            scale.x += Time.deltaTime * 3 * recoil;
            scale.y += Time.deltaTime * 3 * recoil;
            crossImage.transform.localScale = scale;
        }
        //Zoom out
        while (scale.x > 1)
        {
            //Freeze frame
            yield return null;
            scale.x -= Time.deltaTime * 3 * recoil;
            scale.y -= Time.deltaTime * 3 * recoil;
            crossImage.transform.localScale = scale;
        }
        crossImage.transform.localScale = Vector2.one;

    }


    private Coroutine shootRecoil_CameraCoroutine;
    IEnumerator ShootRecoil_Camera(float recoil)
    {
        float xOffset = UnityEngine.Random.Range(0.3f,0.6f) * recoil;
        float yOffset = UnityEngine.Random.Range(-0.15f,0.15f) * recoil;
        firstPersonController.xRotOffset = xOffset;
        firstPersonController.yRotOffset = yOffset;

        //Apply the offset over 6 frames
        for (int i = 0; i < 6; i++)
        {
            yield return null;
        }
        firstPersonController.xRotOffset = 0;
        firstPersonController.yRotOffset = 0;
    }

    #endregion


    public void Hurt(int damage)
    {
        hp -= damage;
        if (hp <= 0) 
        {
            hp = 0;
        }
        UI_MainPanel.Instance.UpdateHP_Text(hp);


    }


    /// <summary>
    /// Switch weapon
    /// </summary>
    private void ChangeWeapon(int newIndex)
    {
        // Check if the same key is being pressed repeatedly
        if (currentWeaponIndex == newIndex) return;
        // Previous weapon index = current weapon
        previousWeaponIndex = currentWeaponIndex;
        // Record the new weapon index
        currentWeaponIndex = newIndex;

        // If it's the first time using a weapon
        if (previousWeaponIndex < 0)
        {
            // Directly enter the current weapon
            weapons[currentWeaponIndex].Enter();
        }
        // If there is already a weapon in hand, we need to exit the current one first before playing the animation for the new weapon
        else
        {
            //Exit the current weapon
            //Wait for the previous weapon to finish exiting before playing the animation for the new weapon
            weapons[previousWeaponIndex].Exit(OnWeaponExitOver);
            canChangeWeapon = false;
        }
    }

    private void OnWeaponExitOver()
    {
        canChangeWeapon = true;
        weapons[currentWeaponIndex].Enter();
    }


    /// <summary>
    /// Perform external initialization for the weapon
    /// </summary>
    public void InitForEnterWeapon(bool wantCrosshair,bool wantBullet)
    {
        crossImage.gameObject.SetActive(wantCrosshair);
        UI_MainPanel.Instance.InitForEnterWeapon(wantBullet);
    }

    public void UpdateBulletUI(int curr_BulletNum,int curr_MaxBulletNum,int standby_BulletNum)
    {
        UI_MainPanel.Instance.UpdateCurrBullet_Text(curr_BulletNum, curr_MaxBulletNum);
        UI_MainPanel.Instance.UpdateStandByBullet_Text(standby_BulletNum);
    }


    public void SetCameraView(int value)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].fieldOfView = value;
        }
    }
}

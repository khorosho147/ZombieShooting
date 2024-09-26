using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// 玩家状态
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


    //十字准星
    [SerializeField] Image crossImage;

    [SerializeField] Camera[] cameras;

    #region 武器相关
    [SerializeField] WeaponBase[] weapons;
    private int currentWeaponIndex = -1;        //当前武器
    private int previousWeaponIndex = -1;       //上一个武器
    private bool canChangeWeapon = true;        //能否切换武器
    #endregion

    private int hp = 100;

    public PlayerState PlayerState;

    //修改玩家状态
    public void ChangePlayerState(PlayerState newState)
    {
        PlayerState = newState;
        //也许武器再进入某个状态时候，也需要做一些事情
        weapons[currentWeaponIndex].OnEnterPlayerState(newState);
    }

    private void Start()
    {
        Instance = this;
        //初始化所有的武器
        for(int i = 0; i< weapons.Length; i++)
        {
            weapons[i].Init(this);

        }
        PlayerState = PlayerState.Move;

        //默认选择第一把武器
        ChangeWeapon(0);
    }

    private void Update()
    {
        //驱动武器层
        weapons[currentWeaponIndex].OnUpdatePlayerState(PlayerState);

        //按键检测切换武器
        if (canChangeWeapon == false) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(2);
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (previousWeaponIndex >= 0) ChangeWeapon(previousWeaponIndex);
        }
    }


    #region 后坐力

    /// <summary>
    /// 开启射击后坐力
    /// </summary>
    /// <param name="recoil"></param>
    public void StartShootRecoil(float recoil = 1)
    {
        //瞄准器放大缩小
        StartCoroutine(Shootrecoil_Cross(recoil));
        if(shootRecoil_CameraCoroutine != null) StopCoroutine(shootRecoil_CameraCoroutine);
        //视角移动
        shootRecoil_CameraCoroutine = StartCoroutine(ShootRecoil_Camera(recoil));
    }

    //后坐力-瞄准器
    IEnumerator Shootrecoil_Cross(float recoil )
    {
        Vector2 scale = crossImage.transform.localScale;
        //放大
        while(scale.x < 1.3f)
        {
            //停帧
            yield return null;
            scale.x += Time.deltaTime * 3 * recoil;
            scale.y += Time.deltaTime * 3 * recoil;
            crossImage.transform.localScale = scale;
        }
        //缩小
        while (scale.x > 1)
        {
            //停帧
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

        //让偏移发生6帧
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
    /// 切换武器
    /// </summary>
    private void ChangeWeapon(int newIndex)
    {
        //是不是重复在按同一个键
        if (currentWeaponIndex == newIndex) return;
        //上一个武器的索引 = 当前武器
        previousWeaponIndex = currentWeaponIndex;
        //记录新的武器索引
        currentWeaponIndex = newIndex;

        //如果是第一次使用
        if (previousWeaponIndex < 0)
        {
            //直接进入当前武器
            weapons[currentWeaponIndex].Enter();
        }
        //现在手里有一个武器，所以要先退出这把武器的，才能播放新武器的动画
        else
        {
            //退出这把武器
            //要等待上一把武器的退出完成后，才能播放新武器的动画
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
    /// 为武器做外部的初始化
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

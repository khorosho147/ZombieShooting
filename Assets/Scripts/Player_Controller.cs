using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// ���״̬
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


    //ʮ��׼��
    [SerializeField] Image crossImage;

    [SerializeField] Camera[] cameras;

    #region �������
    [SerializeField] WeaponBase[] weapons;
    private int currentWeaponIndex = -1;        //��ǰ����
    private int previousWeaponIndex = -1;       //��һ������
    private bool canChangeWeapon = true;        //�ܷ��л�����
    #endregion

    private int hp = 100;

    public PlayerState PlayerState;

    //�޸����״̬
    public void ChangePlayerState(PlayerState newState)
    {
        PlayerState = newState;
        //Ҳ�������ٽ���ĳ��״̬ʱ��Ҳ��Ҫ��һЩ����
        weapons[currentWeaponIndex].OnEnterPlayerState(newState);
    }

    private void Start()
    {
        Instance = this;
        //��ʼ�����е�����
        for(int i = 0; i< weapons.Length; i++)
        {
            weapons[i].Init(this);

        }
        PlayerState = PlayerState.Move;

        //Ĭ��ѡ���һ������
        ChangeWeapon(0);
    }

    private void Update()
    {
        //����������
        weapons[currentWeaponIndex].OnUpdatePlayerState(PlayerState);

        //��������л�����
        if (canChangeWeapon == false) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(2);
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (previousWeaponIndex >= 0) ChangeWeapon(previousWeaponIndex);
        }
    }


    #region ������

    /// <summary>
    /// �������������
    /// </summary>
    /// <param name="recoil"></param>
    public void StartShootRecoil(float recoil = 1)
    {
        //��׼���Ŵ���С
        StartCoroutine(Shootrecoil_Cross(recoil));
        if(shootRecoil_CameraCoroutine != null) StopCoroutine(shootRecoil_CameraCoroutine);
        //�ӽ��ƶ�
        shootRecoil_CameraCoroutine = StartCoroutine(ShootRecoil_Camera(recoil));
    }

    //������-��׼��
    IEnumerator Shootrecoil_Cross(float recoil )
    {
        Vector2 scale = crossImage.transform.localScale;
        //�Ŵ�
        while(scale.x < 1.3f)
        {
            //ͣ֡
            yield return null;
            scale.x += Time.deltaTime * 3 * recoil;
            scale.y += Time.deltaTime * 3 * recoil;
            crossImage.transform.localScale = scale;
        }
        //��С
        while (scale.x > 1)
        {
            //ͣ֡
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

        //��ƫ�Ʒ���6֡
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
    /// �л�����
    /// </summary>
    private void ChangeWeapon(int newIndex)
    {
        //�ǲ����ظ��ڰ�ͬһ����
        if (currentWeaponIndex == newIndex) return;
        //��һ������������ = ��ǰ����
        previousWeaponIndex = currentWeaponIndex;
        //��¼�µ���������
        currentWeaponIndex = newIndex;

        //����ǵ�һ��ʹ��
        if (previousWeaponIndex < 0)
        {
            //ֱ�ӽ��뵱ǰ����
            weapons[currentWeaponIndex].Enter();
        }
        //����������һ������������Ҫ���˳���������ģ����ܲ����������Ķ���
        else
        {
            //�˳��������
            //Ҫ�ȴ���һ���������˳���ɺ󣬲��ܲ����������Ķ���
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
    /// Ϊ�������ⲿ�ĳ�ʼ��
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

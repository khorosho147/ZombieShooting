using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ZombieState
{
    Idle,
    Walk,
    Run,
    Attack,
    Hurt,
    Dead
}

public class ZombieController : MonoBehaviour
{
    [SerializeField]
    private ZombieState zombieState;
    private NavMeshAgent navMeshAgent;
    private AudioSource audioSource;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    public Zombie_Weapon weapon;

    private int hp = 100;
    public AudioClip[] FootstepAudioClips;  // Footstep sound effects
    public AudioClip[] IdleAudioClips;      // Idle sound effects
    public AudioClip[] HurtAudioClips;      // Hurt sound effects
    public AudioClip[] AttackAudioClips;    // Attack sound effects

    private Vector3 target;

    // Logic during state transitions
    public ZombieState ZombieState
    {
        get => zombieState;
        set
        {
            if (zombieState==ZombieState.Dead
               && value!=ZombieState.Idle )
            {
                return;
            }
            zombieState = value;

            switch (zombieState)
            {
                case ZombieState.Idle:
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    navMeshAgent.enabled = false;
                    Invoke("GoWalk", Random.Range(1, 3));
                    break;
                case ZombieState.Walk:
                    animator.SetBool("Walk", true);
                    animator.SetBool("Run", false);
                    navMeshAgent.enabled = true;
                    navMeshAgent.speed = 0.3f;
                    // Move to a target point
                    target = GameManager.Instance.GetPoints();
                    navMeshAgent.SetDestination(target);
                    break;
                case ZombieState.Run:
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", true);
                    navMeshAgent.enabled = true;
                    navMeshAgent.speed = 3.5f;
                    break;
                case ZombieState.Attack:
                    navMeshAgent.enabled = true;
                    animator.SetTrigger("Attack");
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    break;
                case ZombieState.Hurt:
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    animator.SetTrigger("Hurt");
                    break;
                case ZombieState.Dead:
                    navMeshAgent.enabled = false;
                    animator.SetTrigger("Dead");
                    capsuleCollider.enabled = false;
                    Invoke("Destroy", 5);
                    break;
                default:
                    break;
            }
        }
    }

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        weapon.Init(this);
        ZombieState = ZombieState.Idle;
    }

    // Handle dirty data
    public void Init()
    {
        animator.SetTrigger("Init");
  
        capsuleCollider.enabled = true;
        hp = 100;
        ZombieState = ZombieState.Idle;

    }

    void Update()
    {
        StateForUpdate();
    }

    void StateForUpdate()
    {
        // float dis = PlayerController.Instance.PlayerState==PlayerState.Shoot?30f: 10f;
        float dis = 10f;
        switch (zombieState)
        {
            case ZombieState.Idle:
                break;
            case ZombieState.Walk:
                if (Vector3.Distance(transform.position, Player_Controller.Instance.transform.position) < dis)
                {
                    // Chase the player
                    ZombieState = ZombieState.Run;
                    return;
                }
                if (Vector3.Distance(target, transform.position) <= 1)
                {
                    ZombieState = ZombieState.Idle;
                }

                break;
            case ZombieState.Run:
                // Keep following the player
                navMeshAgent.SetDestination(Player_Controller.Instance.transform.position);
                if (Vector3.Distance(transform.position, Player_Controller.Instance.transform.position) < 2.5f)
                {
                    ZombieState = ZombieState.Attack;
                }

                break;
            case ZombieState.Attack:
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Attack"
                     && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    ZombieState = ZombieState.Run;
                }
                break;
            case ZombieState.Hurt:
                break;
            case ZombieState.Dead:
                break;
            default:
                break;
        }
    }

    void GoWalk()
    {
        ZombieState = ZombieState.Walk;

    }

    public void Hurt(int value)
    {
        hp -= value;
        if (hp<=0)
        {
            ZombieState = ZombieState.Dead;
        }
        else
        {
            // Knockback
            StartCoroutine(MovePuase());
        }
    }

    void Destroy()
    {
        ZombieManager.Instance.ZombieDead(this);
    }

    IEnumerator MovePuase()
    {
        ZombieState = ZombieState.Hurt;
        navMeshAgent.enabled = false;
        yield return new WaitForSeconds(0.5f);
        if (ZombieState!=ZombieState.Run)
        {
            ZombieState = ZombieState.Run;
        }
    
    }


    #region Animation event
    void IdelAudio()
    {
        if (Random.Range(0, 4) == 1)
        {
            audioSource.PlayOneShot(IdleAudioClips[Random.Range(0, IdleAudioClips.Length)]);
        }
    }
    void FootStep()
    {
        audioSource.PlayOneShot(FootstepAudioClips[Random.Range(0, IdleAudioClips.Length)]);
    }
    private void HurtAudio()
    {
        audioSource.PlayOneShot(HurtAudioClips[Random.Range(0, HurtAudioClips.Length)]);
    }
    private void AttackAudio()
    {
        audioSource.PlayOneShot(AttackAudioClips[Random.Range(0, AttackAudioClips.Length)]);
    }
    public void StartAttack()
    {
        weapon.StartAttack();
    }
    public void EndAttack()
    {
        weapon.EndAttack();
    }

    #endregion
}

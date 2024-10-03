using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    public static ZombieManager Instance;
    public GameObject prefab_Zombie;

    public List<ZombieController> zombies;//Zombies in the current scene

    private Queue<ZombieController> zombiePool = new Queue<ZombieController>();//Backup zombies
    public Transform Pool;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        StartCoroutine(CheckZombie());
    }

    // Check zombies
    IEnumerator CheckZombie()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            // Not enough zombies, spawn more zombies
            if (zombies.Count<20)
            {
                // If available in pool，take from the pool
                if (zombiePool.Count>0)
                {
                    ZombieController zb = zombiePool.Dequeue();
                    zb.transform.SetParent(transform);
                    zb.transform.position = GameManager.Instance.GetPoints();
                    zombies.Add(zb);
                    zb.gameObject.SetActive(true);
                    zb.Init();
                    yield return new WaitForSeconds(2);
                }
                // Not in pool, instantiate
                else
                {
                    GameObject zb = Instantiate(prefab_Zombie, GameManager.Instance.GetPoints(), Quaternion.identity, transform);
                    zombies.Add(zb.GetComponent<ZombieController>());
                }
            }
        }
    }

    public void ZombieDead(ZombieController zombie)
    {
        zombies.Remove(zombie);
        zombiePool.Enqueue(zombie);
        zombie.gameObject.SetActive(false);
        zombie.transform.SetParent(Pool);

    }
}

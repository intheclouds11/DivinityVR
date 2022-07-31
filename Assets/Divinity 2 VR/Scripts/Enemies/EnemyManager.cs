using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IInteractable
{
    public static EnemyManager Instance;
    private GameObject[] _initialEnemyList;
    public List<GameObject> _currentEnemyList;
    public GameObject SpawnableGO;

    void Awake()
    {
        Instance = this;

        _initialEnemyList = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemyGO in _initialEnemyList)
        {
            _currentEnemyList.Add(enemyGO);
        }
    }

    public void SpawnEnemy(GameObject obj)
    {
        _currentEnemyList.Add(Instantiate(obj, new Vector3(Random.Range(-10, 10), .5f, Random.Range(-10, 10)),
            Quaternion.identity));
    }

    public void Interact()
    {
        SpawnEnemy(SpawnableGO);
    }
}
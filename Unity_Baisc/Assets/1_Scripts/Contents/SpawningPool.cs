using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField] int _monsterCount = 0;
    [SerializeField] int _reserveCount = 0;
    [SerializeField] int _keepMonsterCount;
    [SerializeField] Vector3 _spawnPos;

    [SerializeField] float _spawnRadius = 15;
    [SerializeField] float _spawnTime = 5;

    void Start()
    {
        Managers.Game.OnSpawn -= AddMonsterCount;
        Managers.Game.OnSpawn += AddMonsterCount;

        void AddMonsterCount(int count) => _monsterCount += count;
    }

    public void SetKeepMonsterCount(int newCount) => _keepMonsterCount = newCount;
    void Update()
    {
        while(_reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine(ReserveSpawn());
        }
    }

    IEnumerator ReserveSpawn()
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));
        NavMeshAgent nav = Managers.Game.Spawn(Define.WorldObject.Monster, "Knight").GetComponent<NavMeshAgent>();

        Vector3 randPos;
        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = randDir + _spawnPos;
            if(nav.CalculatePath(randPos, new NavMeshPath())) break;
        }
        nav.gameObject.transform.position = randPos;
        _reserveCount--;
    }
}

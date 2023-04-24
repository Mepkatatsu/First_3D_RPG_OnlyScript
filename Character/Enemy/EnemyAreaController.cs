using SingletonPattern;
using System.Collections;
using UnityEngine;

public class EnemyAreaController : MonoBehaviour
{
    #region Variables

    private Transform _player;
    public GameObject MonsterPrefab;

    public int numberOfEnemy = 5;

    public float checkPlayerRange = 20;
    public float enemyMoveRange = 15;

    #endregion Variables

    #region Properties

    public float DistanceToPlayer => Vector3.Distance(transform.position, _player.transform.position);

    public bool IsPlayerInArea
    {
        get
        {
            if (checkPlayerRange > DistanceToPlayer) return true;
            else return false;
        }
    }

    #endregion Properties

    #region Unity Methods

    void Start()
    {
        _player = PlayerCharacterController.Instance.transform;

        StartCoroutine(CheckEnemyCoroutine());
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkPlayerRange);
    }
#endif

    #endregion Unity Methods


    #region Methods
    private IEnumerator CheckEnemyCoroutine()
    {
        for (int i = 0; i < numberOfEnemy; ++i)
        {
            SpawnEnemy();
        }

        while(true)
        {
            while (transform.childCount < numberOfEnemy)
            {
                yield return new WaitForSeconds(1f);
                SpawnEnemy();
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private void SpawnEnemy()
    {
        float randomX = Random.Range(-10, 10f);
        float randomZ = Random.Range(-10, 10f);
        float randomRotation = Random.Range(0, 360);

        Vector3 spawnPosition = new Vector3(randomX, MonsterPrefab.transform.position.y, randomZ) + transform.position;
        Quaternion spawnRotation = Quaternion.Euler(0, randomRotation, 0);

        GameObject enemy = Instantiate(MonsterPrefab, spawnPosition, spawnRotation, transform);
        enemy.GetComponent<EnemyController>().patrolPosition = spawnPosition;
    }

    public bool CheckEnemyMoveRange(Vector3 position)
    {
        return Vector3.Distance(position, transform.position) < enemyMoveRange;
    }

    #endregion Methods
}

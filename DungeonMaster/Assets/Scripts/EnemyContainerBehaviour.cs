using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class EnemyContainerBehaviour : MonoBehaviour
{
    private int hitpoints;
    public int maxHitpoints;
    public List<Vector2> wayPoints;
    
    public GameObject hpPrefab;
    public int hpDropChance;
    
    public GameObject coinPrefab;
    public int maxMoneyCount;

    private int wayIndex;
    public float speed;
    
    public GameObject enemyPrefab;
    public int enemyCount;

    public int gainPoints;
    
    public EnemyContainerBehaviour(List<Vector2> wayPoints, float speed)
    {
        this.wayPoints = wayPoints;
        this.speed = speed;
    }


    private void Start()
    {
        hitpoints = maxHitpoints;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        var dir = wayPoints[wayIndex] - (Vector2) transform.position;

        transform.Translate(dir.normalized * (Time.deltaTime * speed));

        if (Vector3.Distance(transform.position, wayPoints[wayIndex]) < .2f)
        {
            if (wayIndex < wayPoints.Count - 1)
                wayIndex++;
            else
                Destroy(gameObject);
        }
    }

    public void TakeHit(int damage)
    {
        hitpoints -= damage;
        if (hitpoints <= 0)
        {
            var rnd = new Random();
            Destroy(gameObject);
            SpawnCoin(rnd.Next(1, maxMoneyCount + 1));
            SpawnHp(hpDropChance);
            SpawnEnemies(enemyCount);
            ScoreManager.instance.AddPoints(gainPoints);
        }
    }

    private void SpawnEnemies(int count)
    {
        var rnd = new Random();
        for (var i = 0; i < count; i++)
        {
            var posX = (float) rnd.NextDouble() * 2 - 1;
            var posY = (float) rnd.NextDouble() * 2 - 1;
            var tmpEnemy = 
                Instantiate(enemyPrefab, transform.position + new Vector3(posX, posY), Quaternion.Euler(0f, 0f, 0f));
            var enemyScr = tmpEnemy.GetComponent<EnemyBehaviour>();
            enemyScr.wayPoints = wayPoints;
            enemyScr.wayIndex = wayIndex;
            enemyScr.speed = speed;
        }
    }
    
    private void SpawnCoin(int count)
    {
        var rnd = new Random();
        for (var i = 0; i < count; i++)
        {
            var posX = (float) rnd.NextDouble() * 2 - 1;
            var posY = (float) rnd.NextDouble() * 2 - 1;
            Instantiate(coinPrefab, transform.position + new Vector3(posX, posY), 
                Quaternion.Euler(0, 0, 0));
        }
    }
    
    private void SpawnHp(int probability)
    {
        var rnd = new Random();
        if (rnd.Next(probability) != 0) return;
        
        var posX = (float) rnd.NextDouble() * 2 - 1;
        var posY = (float) rnd.NextDouble() * 2 - 1;
        Instantiate(hpPrefab, transform.position + new Vector3(posX, posY), 
            Quaternion.Euler(0, 0, 0));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var baseObj = other.collider.GetComponent<BaseBehaviour>();
        if (baseObj)
        {
            baseObj.TakeHit(1);
            Destroy(gameObject);
        }
    }
}
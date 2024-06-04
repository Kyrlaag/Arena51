using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public float gridPower;
    private GameObject[] enemies;
    private GameObject[] allies;

    private float timer = 0f;
    private float interval_of_attack = 2f;

    private bool canBattle = false;


    void Start()
    {
        enemies = new GameObject[GameObject.FindGameObjectsWithTag("Enemy").Length];
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        allies = new GameObject[GameObject.FindGameObjectsWithTag("Ally").Length];
        allies = GameObject.FindGameObjectsWithTag("Ally");
        FindObjectOfType<AudioManager>().Play("background");
        //InvokeRepeating("AttackEnemy", 0f, 5f);
    }

    void Update()
    {
        if (canBattle == true)
        {
            TimerCheck();

        }
    }

    void enemiesCheckingAllies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                Stats enemystats = enemy.GetComponent<Stats>();
                RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, Vector2.down, enemystats.range * gridPower, LayerMask.GetMask("Ally"));

                if (hit.collider == null)
                {
                    //move
                    MoveEnemy(enemy);
                }

                else if (hit.collider.gameObject.CompareTag("Ally"))
                {
                    //attack               
                    Attack(enemy, hit.collider.gameObject);
                }

                else if (hit.collider.gameObject.CompareTag("DeadZone"))
                {
                    FindObjectOfType<GameManager>().GameOver();
                }
            }
            
        }
    }

    void alliesCheckingEnemies()
    {
        foreach (var ally in allies)
        {
            if (ally != null)
            {
                Stats allystats = ally.GetComponent<Stats>();
                RaycastHit2D hit = Physics2D.Raycast(ally.transform.position, Vector2.up, allystats.range * gridPower, LayerMask.GetMask("Enemy"));

                if (hit.collider != null)
                {
                    //attack
                    Attack(ally, hit.collider.gameObject);
                }
            }
               
            
        }
        
    }

    void Attack(GameObject attacker, GameObject damageTaker)
    {
        damageTaker.gameObject.GetComponent<Stats>().TakeDamage(attacker.GetComponent<Stats>().damage);
    }
    private void MoveEnemy(GameObject enemy)
    {
        enemy.transform.Translate(0, -1f * gridPower, 0);
    }

    private void TimerCheck()
    {
        if (timer >= interval_of_attack)
        {
            timer = 0;
            //Check Enemies and Allies
            enemiesCheckingAllies();
            alliesCheckingEnemies();
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public void StartBattle()
    {
        canBattle = true;
    }

   
}

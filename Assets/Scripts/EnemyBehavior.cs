﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public Transform player;
    public Transform patrolRoute;
    public List<Transform> locations;
    private int locationIndex = 0;
    private NavMeshAgent agent;
    private int _lives = 3;
    public int enemyDamage = 1;
    public GameObject gunshotEffect;
    public Transform gunSpawnPoint;
    public int maxHealth = 3;
    private int currentHealth;
    //private bool isShooting = false;

    public int EnemyLives
    {
        get { return _lives; }
        private set
        {
            _lives = value;
            if (_lives <= 0)
            {
                DestroyEnemy();
            }
        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        InitializePatrolRoute();
        MoveToNextPatrolLocation();
    }

    private void Update()
    {
        if (agent.remainingDistance < 0.2f && !agent.pathPending)
        {
            MoveToNextPatrolLocation();
        }
    }

    void MoveToNextPatrolLocation()
    {
        if (locations.Count == 0)
            return;

        agent.destination = locations[locationIndex].position;
        locationIndex = (locationIndex + 1) % locations.Count;
    }

    private void InitializePatrolRoute()
    {
        foreach (Transform child in patrolRoute)
        {
            locations.Add(child);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            agent.destination = player.position;
            Debug.Log("Player detected - attack!");
        }
        if (other.CompareTag("Player"))
        {
            PlayerBehavior player = other.GetComponent<PlayerBehavior>();
            if (player != null)
            {
                player.TakeDamage(enemyDamage);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            Debug.Log("Player out of range, resume patrol");
        }
        if (other.CompareTag("Player"))
        {
            StopShooting();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Bullet(Clone)")
        {
            EnemyLives -= 1;
            Debug.Log("Critical hit!");
            StartShooting();
        }
    }

   private void StartShooting()
    {
        gunshotEffect.SetActive(true);
        Shoot();
    }

    private void StopShooting()
    {
        gunshotEffect.SetActive(false);
    }


    private void Shoot()
    {
        if (gunshotEffect != null && gunSpawnPoint != null)
        {
            GameObject gunshot = Instantiate(gunshotEffect, gunSpawnPoint.position, gunSpawnPoint.rotation);
            RaycastHit hit;
            if (Physics.Raycast(gunSpawnPoint.position, gunSpawnPoint.forward, out hit))
            {
                PlayerBehavior player = hit.collider.GetComponent<PlayerBehavior>();
                if (player != null)
                {
                    int damage = Random.Range(1, 4);
                    player.TakeDamage(damage);
                }
            }
            Destroy(gunshot, 1.0f);
        }
    }

    public void TakeDamage(int damage)
    {
        _lives -= damage;
        Debug.Log("Enemy took " + damage + " damage.");
        if (_lives <= 0)
        {
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        Destroy(this.gameObject);
        Debug.Log("Enemy down.");
    }
}

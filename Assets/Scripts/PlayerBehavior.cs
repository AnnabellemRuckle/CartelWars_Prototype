using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerBehavior : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 75f;
    public float jumpVelocity = 5f;
    public float duckedHeight = 0.5f;
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;
    public GameObject gunshotEffect; 
    public Transform gunSpawnPoint; 
    private float vInput;
    private float hInput;
    private float originalHeight;
    private Rigidbody _rb;
    private CapsuleCollider _col;
    public delegate void JumpingEvent();
    public event JumpingEvent playerJump;
    private bool isShooting = false;
    public int maxHealth = 3; 
    private int currentHealth;
    public int enemyDamage = 1; 
    
    

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        originalHeight = _col.height;
        playerJump += PerformJump;
        currentHealth = maxHealth;
    }

    void Update()
    {
        vInput = Input.GetAxis("Vertical") * moveSpeed;
        hInput = Input.GetAxis("Horizontal") * rotateSpeed;

        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            playerJump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartShooting();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopShooting();
        }

        if (Input.GetKey(KeyCode.S))
        {
            Duck();
        }
        else
        {
            ReleaseDuck();
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        Vector3 rotation = Vector3.up * hInput;
        Quaternion angleRot = Quaternion.Euler(rotation * Time.fixedDeltaTime);

        _rb.MovePosition(this.transform.position + this.transform.forward * vInput * Time.fixedDeltaTime);
        _rb.MoveRotation(_rb.rotation * angleRot);

        if (isShooting)
        {
            Shoot();
        }
    }

    private bool IsGrounded()
    {
        Vector3 capsuleBottom = new Vector3(_col.bounds.center.x, _col.bounds.min.y, _col.bounds.center.z);
        bool grounded = Physics.CheckCapsule(_col.bounds.center, capsuleBottom, distanceToGround, groundLayer, QueryTriggerInteraction.Ignore);

        return grounded;
    }

    private void PerformJump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, jumpVelocity, _rb.velocity.z);
    }

    private void Duck()
    {
        _col.height = duckedHeight;
    }

    private void ReleaseDuck()
    {
        _col.height = originalHeight;
    }

    private void StartShooting()
    {
        isShooting = true;
        gunshotEffect.SetActive(true); 
    }

    private void StopShooting()
    {
        isShooting = false;
        gunshotEffect.SetActive(false); 
    }

    private void Shoot()
    {
        if (gunshotEffect != null && gunSpawnPoint != null)
        {
           
            GameObject gunshot = Instantiate(gunshotEffect, gunSpawnPoint.position, gunSpawnPoint.rotation);

            // Check for hits on enemies with a raycast
            RaycastHit hit;
            if (Physics.Raycast(gunSpawnPoint.position, gunSpawnPoint.forward, out hit))
            {
                EnemyBehavior enemy = hit.collider.GetComponent<EnemyBehavior>();
                if (enemy != null)
                {
                    // damage to the enemy
                    int damage = Random.Range(1, 4); // damage between 1 and 3
                    enemy.TakeDamage(damage);
                }
            }

            Destroy(gunshot, 1.0f);
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead.");

      //  gameOverManager.ShowGameOver();
        GetComponent<PlayerBehavior>().enabled = false;
        SceneManager.LoadScene("MainLand");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(enemyDamage);
        }
    }
   

}
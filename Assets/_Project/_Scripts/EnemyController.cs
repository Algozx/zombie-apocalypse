using System;
using UnityEngine;
using Zombie.DataEvent;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Movement), typeof(Status))]
public class EnemyController : MonoBehaviour, InterfaceFuncoes
{
    public static event EventHandler OnZombieKill;
    
    [SerializeField] public ZombieSpawner spawner;
    
    [SerializeField] private Movement movementController;
    [SerializeField] private AnimationController animationController;
    [SerializeField] private Status enemyStatus;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private GameObject firstAid;
    
    private Vector3 _direction;
    private float _distance;
    private Vector3 _randomPos;
    private float _timeToWander;
    private float _timeBetweenPos = 4;
    private float _chanceToDrop = 0.1f;
    private GameObject _player;
    private InterfaceController _interfaceController;

    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _interfaceController = FindAnyObjectByType<InterfaceController>();
        RandomZombieSkin();
    }

    void Update()
    {
        ChasingPlayer();
    }
    
    /// <summary>
    /// Call this function to restore players health.
    /// </summary>
    void ChasingPlayer()
    {
        _distance = Vector3.Distance(transform.position, _player.transform.position);
        movementController.Rotate(_direction);

        animationController.Move(_direction.magnitude);
        
        int disToStopChasing = 15;
        float disToMoveNAttack = 2.4f;
        float disToAttack = 2.4f;

        switch (_distance)
        {
            case var dis when _distance > disToStopChasing:
                Wander();
                break;
            case var dis when _distance > disToMoveNAttack:
                _direction = _player.transform.position - transform.position;
                movementController.Move(_direction, enemyStatus.speed);
                animationController.Attack(false);
                break;
            case var dis when _distance < disToAttack:
                _direction = _player.transform.position - transform.position;
                animationController.Attack(true);
                break;
        }
    }

    /// <summary>
    /// Call this function to restore players health.
    /// </summary>
    void Wander()
    {
        bool isCloseToPlayer = Vector3.Distance(transform.position, _randomPos) <= 0.05;
        
        _timeToWander -= Time.deltaTime;
        if (_timeToWander <= 0)
        {
            _randomPos = SetRandomPosition();
            _timeToWander += _timeBetweenPos + Random.Range(-1f, 1f);
        }
        
        if (!isCloseToPlayer)
        {
            _direction = _randomPos - transform.position;
            movementController.Move(_direction, enemyStatus.speed);
        }
    }

    /// <summary>
    /// Call this function to restore players health.
    /// </summary>
    Vector3 SetRandomPosition()
    {
        Vector3 pos = Random.insideUnitSphere * 10;
        var position = transform.position;
        pos += position;
        pos.y = position.y;

        return pos;
    }

    //Change damage to event system?
    void AtacaJogador()
    {
        int dano = Random.Range(12, 18);
        _player.GetComponent<PlayerController>().TakeDamage(dano);
    }

    /// <summary>
    /// Call this function to restore players health.
    /// </summary>
    void RandomZombieSkin()
    {
        int randomSkin = Random.Range(1, transform.childCount);
        transform.GetChild(randomSkin).gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Call this function to restore players health.
    /// </summary>
    void FirstAidDrop(float chanceToDrop)
    {
        if (Random.value <= chanceToDrop)
        {
            Instantiate(firstAid, transform.position, Quaternion.identity);
        }
    }
    
    public void TakeDamage(int damage)
    {
        enemyStatus.initialHealth -= damage;
        if (enemyStatus.initialHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animationController.Died();
        movementController.Die();
        this.enabled = false;
        ControlaAudio.instancia.PlayOneShot(deathSound);
        FirstAidDrop(_chanceToDrop);
        OnZombieKill?.Invoke(this, EventArgs.Empty);
        spawner.DecreaseZombiesAlive();
        Destroy(gameObject, 4);
    }

    public void Heal(int healingQuantity)
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    private Player _player;

    private Animator _anim;
    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _laserPrefab;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Animator is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Explosion in enemy is NULL");
        }

        StartCoroutine(FireEnemyLaser());
  
    }


    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // If at bottom of screen respawn at top with new random x position 
        if (transform.position.y <= -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7f, 0);
        }

    }

    // Enemy behaviour when collided with
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        // Damage player then destroy enemy
        if (other.tag.Equals("Player"))
        {
            Player player = other.transform.GetComponent<Player>();
            
            if (player != null)
            {
                player.Damage();
            }

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(this.gameObject,2.3f);
        }

        // Destroy laser and enemy
        if (other.tag.Equals("Laser"))
        {

            Destroy(other.gameObject);

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject,2.3f);
            

            if (_player != null)
            {
                _player.AddScore(10);
            }
        }
    }

    IEnumerator FireEnemyLaser()
    {
        yield return new WaitForSeconds(Random.Range(3, 8));
        Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
    }
}

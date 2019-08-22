using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private float _fireRate = 0.15f; // Cooldown on laser shots
    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;

    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;

    [SerializeField]
    private GameObject _shieldVisual;

    [SerializeField]
    private int _score;

    private UIManager _uiManager;

    [SerializeField]
    private GameObject _rightEngine, _leftEngine;

    private int _randomEngine;

    [SerializeField]
    private AudioClip _laserSoundClip;

    private AudioSource _audioSource;

    

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        _audioSource = GetComponent<AudioSource>();


        if (_spawnManager == null)
        {
            Debug.LogError("Spawn manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on player is NULL");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }


        _score = 0;
    }

    void Update()
    { 
        CalculateMovement();

        // Fire laser if Space pressed and cooldown permits
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    // Handles everything related to player movement
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);
        
        // Clamp restricts the player's y position 
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);
        
        // Move player to opposite side if they reach screen end
        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    // Handles action of spawning a laser on cooldown, play sound effect
    void FireLaser()
    {
    
     _canFire = Time.time + _fireRate;
    
    if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
    else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();

    }

    // Calculate damage done to the player
    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisual.SetActive(false);
            return;
        }

        _lives--;

        if (_lives == 2)
        {
            _randomEngine = Random.Range(0, 2);

            if (_randomEngine == 0)
            {
                _rightEngine.SetActive(true);
            }
            else
            {
                _leftEngine.SetActive(true);
            }
        }
        else if (_lives == 1)
        {
            if (_rightEngine.activeSelf == true)
            {
                _leftEngine.SetActive(true);
            }
            else
            {
                _rightEngine.SetActive(true);
            }
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    // Toggle triple shot
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    // Timer for triple shot being active 
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
        
    }

    // Toggle speed boost
    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    // Timer for speed boost being active
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    // Toggle shield
    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisual.SetActive(true);
    }

    // Add points to player score
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("EnemyLaser"))
        {
            Damage();
            Destroy(other.gameObject);
        }
    }



}
 
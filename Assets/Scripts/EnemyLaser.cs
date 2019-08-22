using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6.0f;

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // If laser is off screen destroy it
        if (transform.position.y < -8.0f)
        {
            Destroy(this.gameObject);

            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }
}

using UnityEngine;

public class AlienBullet : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private GameManager gameManager;
    private bool hasHit;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void FixedUpdate()
    {
        Vector3 position = transform.position;
        position.y -= speed * Time.fixedDeltaTime;
        transform.position = position;

        if (position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
        {
            return;
        }

        Spaceship ship =
            collision.gameObject.GetComponentInParent<Spaceship>();

        if (ship != null)
        {
            hasHit = true;

            if (gameManager != null)
            {
                gameManager.TakeHit();
            }

            Destroy(gameObject);
            return;
        }

        // Player shots can intercept alien bullets.
        if (collision.gameObject.GetComponentInParent<Dart>() != null)
        {
            hasHit = true;
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
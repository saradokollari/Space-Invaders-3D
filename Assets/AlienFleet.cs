using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AlienFleet : MonoBehaviour
{
    [Header("Inscribed")]

    public GameObject alienPrefab;

    public Sprite grayMouseSprite;
    public Sprite brownMouseSprite;

    public AlienCounter alienCounter;

    public List<GameObject> aliens = new List<GameObject> { };

    float speed = .065f;
    float startingSpeed = .065f;
    float maximumSpeed = .16f;
    int initialAlienCount;
    float leftBound = -10.9f;
    float rightBound = 10.9f;
    float lowerBound = 4.8f;
    float downTime = .1f;
    bool movingDown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject alien = Instantiate<GameObject>(alienPrefab);
                alien.transform.position = new Vector3(
                    1.9f * i - 8.5f,
                    8.8f - 1.9f * j,
                    0f
                    );
                aliens.Add(alien);
                alien.GetComponent<Alien>().fleet = this;

                SpriteRenderer mouseRenderer =
                    alien.GetComponentInChildren<SpriteRenderer>();

                if (mouseRenderer != null)
                {
                    mouseRenderer.sprite = (i + j) % 2 == 0
                        ? grayMouseSprite
                        : brownMouseSprite;
                }
            }
        }

        initialAlienCount = aliens.Count;

    }

    // Update is called once per frame
    void Update()
    {
        alienCounter.score = aliens.Count;
    }

    void FixedUpdate()
    {

        if (aliens.Count == 0 || initialAlienCount == 0)
{
    return;
}

float destroyedFraction =
    1f - (float)aliens.Count / initialAlienCount;

float currentSpeed = Mathf.Lerp(
    startingSpeed,
    maximumSpeed,
    destroyedFraction
);

// Extra acceleration as the last few mice remain.
currentSpeed += 0.06f * Mathf.Pow(destroyedFraction, 4f);

// Preserve the fleet's current direction.
speed = Mathf.Sign(speed) * currentSpeed;

        //move all aliens
        foreach (GameObject alien in aliens)
        {
            Vector3 position = alien.transform.position;
            if (!movingDown)
            {
                position.x += speed;
            }
            else
            {
                position.y += -Mathf.Abs(speed);
                /*if (position.y <= lowerBound)
                {
                    movingDown = false;
                    lowerBound = position.y - 1;
                }*/
            }
            alien.transform.position = position;
        }

        //change alien direction
        float left = float.PositiveInfinity;
        float right = float.NegativeInfinity;
        foreach (GameObject alien in aliens)
        {
            Vector3 position = alien.transform.position;
            if (position.x > right)
            {
                right = position.x;
            }
            if (position.x < left)
            {
                left = position.x;
            }
        }
        if (speed > 0 && right >= rightBound || speed < 0 && left <= leftBound)
        {
            speed = -speed;
            movingDown = true;
            Invoke("startSidewaysMotion", downTime);
        }
    }

    void startSidewaysMotion()
    {
        movingDown = false;
    }
}

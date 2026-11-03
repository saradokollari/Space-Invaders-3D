using UnityEngine;

public class SpinSprite : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond = 180f;

    void Update()
    {
        transform.Rotate(
            0f,
            0f,
            degreesPerSecond * Time.deltaTime,
            Space.Self
        );
    }
}
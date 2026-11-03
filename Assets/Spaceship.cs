using UnityEngine;
using UnityEngine.InputSystem;

public class Spaceship : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject dartPrefab;
    [SerializeField] private float shotCooldown = 0.05f;

    [Header("Movement")]
    public float speed = 10f;

    private Camera gameCamera;
    private float leftExtent;
    private float rightExtent;
    [SerializeField] private float edgePadding = 0.15f;
    private float lastShot = float.NegativeInfinity;

    void Start()
    {
        gameCamera = Camera.main;

        // Measure the whole ship, including its child objects.
        Renderer[] shipRenderers = GetComponentsInChildren<Renderer>();

        if (shipRenderers.Length > 0)
        {
            Bounds bounds = shipRenderers[0].bounds;

            foreach (Renderer shipRenderer in shipRenderers)
            {
                bounds.Encapsulate(shipRenderer.bounds);
            }

            leftExtent = transform.position.x - bounds.min.x;
            rightExtent = bounds.max.x - transform.position.x;
        }
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null || gameCamera == null || Time.timeScale == 0f)
        {
            return;
        }

        float direction = 0f;

        if (keyboard.rightArrowKey.isPressed)
        {
            direction += 1f;
        }

        if (keyboard.leftArrowKey.isPressed)
        {
            direction -= 1f;
        }

        Vector3 position = transform.position;
        position.x += direction * speed * Time.deltaTime;

        // Find the screen edges at the ship's distance from the camera.
        float depth = gameCamera.WorldToViewportPoint(position).z;

        float leftEdge = gameCamera.ViewportToWorldPoint(
            new Vector3(0.135f, 0.5f, depth)
        ).x;

        float rightEdge = gameCamera.ViewportToWorldPoint(
            new Vector3(0.86f, 0.5f, depth)
        ).x;

        // Keep the entire ship inside the screen.
        position.x = Mathf.Clamp(
            position.x,
            leftEdge + leftExtent + edgePadding,
            rightEdge - rightExtent - edgePadding
);

        transform.position = position;

        if (keyboard.spaceKey.wasPressedThisFrame &&
            Time.time - lastShot >= shotCooldown &&
            dartPrefab != null)
        {
            Instantiate(dartPrefab, transform.position, Quaternion.identity);
            lastShot = Time.time;
        }
    }
}
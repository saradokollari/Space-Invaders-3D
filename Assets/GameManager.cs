using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float defenseLineY = -4f;
    [SerializeField] private Texture2D fishTexture;

    [Header("UI Appearance")]
    [SerializeField] private Font pixelFont;

    private enum GameState { Ready, Playing, Won, Lost }

    private GameState state = GameState.Ready;
    private AlienFleet fleet;
    private Camera gameCamera;
    private string lossReason;
    private int livesRemaining = 3;
    private float nextDamageTime;

    private GUIStyle titleStyle;
    private GUIStyle textStyle;
    private GUIStyle buttonStyle;
    private GUIStyle hudStyle;

    private readonly Color outline = new Color32(35, 18, 20, 255);
    private readonly Color wood = new Color32(130, 67, 35, 255);
    private readonly Color woodLight = new Color32(198, 126, 62, 255);
    private readonly Color panel = new Color32(55, 30, 37, 255);
    private readonly Color gold = new Color32(255, 211, 97, 255);

    void Awake()
    {
        Time.timeScale = 0f;
    }

    void Start()
    {
        fleet = FindFirstObjectByType<AlienFleet>();
        gameCamera = Camera.main;

        // Hide the old counter. The new HUD displays the count below.
        // Its AlienCounter script remains available to AlienFleet.
        if (fleet != null && fleet.alienCounter != null)
        {
            TMPro.TextMeshProUGUI oldText =
                fleet.alienCounter.GetComponent<TMPro.TextMeshProUGUI>();

            if (oldText != null)
                oldText.enabled = false;
        }
    }

    void Update()
    {
        if (state != GameState.Playing || fleet == null)
            return;

        if (fleet.aliens.Count == 0)
        {
            state = GameState.Won;
            Time.timeScale = 0f;
            return;
        }

        foreach (GameObject alien in fleet.aliens)
        {
            if (alien == null)
                continue;

            Renderer alienRenderer = alien.GetComponent<Renderer>();

            float alienBottom = alienRenderer != null
                ? alienRenderer.bounds.min.y
                : alien.transform.position.y;

            if (alienBottom <= defenseLineY)
            {
                LoseGame();
                return;
            }
        }
    }

    public void TakeHit()
    {
        if (state != GameState.Playing || Time.time < nextDamageTime)
            return;

        livesRemaining--;
        nextDamageTime = Time.time + 1f;

        if (livesRemaining <= 0)
            LoseGame("You ran out of lives!");
    }

    public void LoseGame(
        string reason = "The mice reached your fish!")
    {
        if (state != GameState.Playing)
            return;

        lossReason = reason;
        state = GameState.Lost;
        Time.timeScale = 0f;
    }

    private void StartGame()
    {
        state = GameState.Playing;
        Time.timeScale = 1f;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private GUIStyle MakeLabel(int size, TextAnchor alignment)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.font = pixelFont != null ? pixelFont : GUI.skin.font;
        style.fontSize = size;
        style.alignment = alignment;
        style.normal.textColor = gold;
        style.wordWrap = true;
        return style;
    }

    private void PrepareStyles()
    {
        if (titleStyle != null)
            return;

        titleStyle = MakeLabel(24, TextAnchor.MiddleCenter);
        textStyle = MakeLabel(12, TextAnchor.MiddleCenter);
        hudStyle = MakeLabel(12, TextAnchor.MiddleCenter);

        buttonStyle = new GUIStyle();
        buttonStyle.font = pixelFont != null ? pixelFont : GUI.skin.font;
        buttonStyle.fontSize = 16;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle.normal.textColor = gold;
        buttonStyle.hover.textColor = Color.white;
        buttonStyle.active.textColor = gold;
    }

    private void Fill(Rect rect, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previousColor;
    }

    private Rect Inset(Rect rect, float amount)
    {
        return new Rect(
            rect.x + amount,
            rect.y + amount,
            rect.width - amount * 2f,
            rect.height - amount * 2f
        );
    }

    private void DrawWoodFrame(Rect rect)
    {
        Fill(rect, outline);
        Fill(Inset(rect, 3f), wood);
        Fill(Inset(rect, 7f), woodLight);
        Fill(Inset(rect, 10f), outline);
        Fill(Inset(rect, 13f), panel);

        // Small square nails in the wooden corners.
        float[] cornersX = { rect.x + 5f, rect.xMax - 9f };
        float[] cornersY = { rect.y + 5f, rect.yMax - 9f };

        foreach (float x in cornersX)
        {
            foreach (float y in cornersY)
                Fill(new Rect(x, y, 4f, 4f), outline);
        }
    }

    private void DrawFishLine()
    {
        if (gameCamera == null || fishTexture == null)
            return;

        Vector3 screenPosition = gameCamera.WorldToScreenPoint(
            new Vector3(0f, defenseLineY, 0f)
        );

        float left = Screen.width * 0.135f;
        float right = Screen.width * 0.86f;
        float availableWidth = right - left;

        float fishWidth = Screen.height * 0.055f;
        float fishHeight =
            fishWidth * fishTexture.height / (float)fishTexture.width;

        float gap = fishWidth * 0.25f;

        int fishCount = Mathf.Max(
            1,
            Mathf.FloorToInt(
                (availableWidth + gap) / (fishWidth + gap)
            )
        );

        float rowWidth =
            fishCount * fishWidth + (fishCount - 1) * gap;

        float startX = left + (availableWidth - rowWidth) / 2f;
        float top = Screen.height - screenPosition.y;

        for (int i = 0; i < fishCount; i++)
        {
            GUI.DrawTexture(
                new Rect(
                    startX + i * (fishWidth + gap),
                    top,
                    fishWidth,
                    fishHeight
                ),
                fishTexture,
                ScaleMode.ScaleToFit,
                true
            );
        }
    }

    void OnGUI()
    {
        PrepareStyles();
        DrawFishLine();

        float scale = Mathf.Min(
            Screen.width / 960f,
            Screen.height / 540f
        );

        Matrix4x4 previousMatrix = GUI.matrix;

        GUI.matrix = Matrix4x4.TRS(
            new Vector3(
                (Screen.width - 960f * scale) / 2f,
                (Screen.height - 540f * scale) / 2f,
                0f
            ),
            Quaternion.identity,
            new Vector3(scale, scale, 1f)
        );

        // Compact wooden HUD plaques.
        Rect mouseCounter = new Rect(20f, 10f, 190f, 40f);
        Rect livesCounter = new Rect(808f, 10f, 132f, 40f);

        Fill(mouseCounter, outline);
        Fill(Inset(mouseCounter, 2f), wood);
        Fill(Inset(mouseCounter, 5f), panel);

        Fill(livesCounter, outline);
        Fill(Inset(livesCounter, 2f), wood);
        Fill(Inset(livesCounter, 5f), panel);

        int miceRemaining = fleet != null ? fleet.aliens.Count : 0;

        GUI.Label(
            mouseCounter,
            "Mice left: " + miceRemaining,
            hudStyle
        );

        GUI.Label(
            livesCounter,
            "Lives: " + livesRemaining,
            hudStyle
        );

        if (state != GameState.Playing)
        {
            DrawWoodFrame(new Rect(180f, 135f, 600f, 290f));

            string title = state == GameState.Ready
                ? "PANTRY PATROL"
                : state == GameState.Won
                    ? "PANTRY SAVED!"
                    : "GAME OVER";

            string message = state == GameState.Ready
                ? "Arrow Keys: Move\n\n Space: Shoot\n\n\n"
                    + "Chase away every mouse\n\n"
                    + "before they reach your fish!"
                : state == GameState.Won
                    ? "Every mouse is gone.\n\n"
                        + "Your fish are safe!"
                    : lossReason;

            GUI.Label(
                new Rect(205f, 160f, 550f, 50f),
                title,
                titleStyle
            );

            GUI.Label(
                new Rect(220f, 218f, 520f, 100f),
                message,
                textStyle
            );

            Rect buttonRect = new Rect(350f, 340f, 260f, 55f);
            bool hovered =
                buttonRect.Contains(Event.current.mousePosition);

            Fill(buttonRect, outline);
            Fill(
                Inset(buttonRect, 3f),
                hovered ? woodLight : wood
            );
            Fill(Inset(buttonRect, 7f), panel);

            if (GUI.Button(
                buttonRect,
                state == GameState.Ready ? "START GAME" : "PLAY AGAIN",
                buttonStyle
            ))
            {
                if (state == GameState.Ready)
                    StartGame();
                else
                    RestartGame();
            }
        }

        GUI.matrix = previousMatrix;
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
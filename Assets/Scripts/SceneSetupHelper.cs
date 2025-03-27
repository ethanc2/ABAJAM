using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
/// <summary>
/// Editor utility script to help set up the various scenes for ABA Jam.
/// This script provides methods to quickly create the required game objects,
/// components, and connections for each scene.
/// </summary>
public class SceneSetupHelper : MonoBehaviour
{
    [Header("Scene Setup")]
    public enum SceneType
    {
        TeamSelection,
        GameModeSelection,
        MainGame,
        ThreePointContest
    }
    public SceneType sceneToSetup;
    
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject ballPrefab;
    public GameObject teamButtonPrefab;
    public GameObject shootingSpotPrefab;
    
    /// <summary>
    /// Editor-only method to set up the selected scene type
    /// </summary>
    public void SetupSelectedScene()
    {
        switch (sceneToSetup)
        {
            case SceneType.TeamSelection:
                Debug.Log("Setting up Team Selection Scene...");
                SetupTeamSelectionScene();
                break;
            case SceneType.GameModeSelection:
                Debug.Log("Setting up Game Mode Selection Scene...");
                SetupGameModeSelectionScene();
                break;
            case SceneType.MainGame:
                Debug.Log("Setting up Main Game Scene...");
                SetupMainGameScene();
                break;
            case SceneType.ThreePointContest:
                Debug.Log("Setting up Three Point Contest Scene...");
                SetupThreePointContestScene();
                break;
        }
    }
    
    /// <summary>
    /// Sets up the Team Selection scene with all required objects and components
    /// </summary>
    void SetupTeamSelectionScene()
    {
        // Create GameManager
        CreateGameManager();
        
        // Create Canvas
        Canvas canvas = CreateCanvas();
        
        // Create TeamSelectionMenu component
        TeamSelectionMenu teamSelectionMenu = canvas.gameObject.AddComponent<TeamSelectionMenu>();
        
        // Create basic UI elements (title, scroll view, confirm button)
        GameObject titleObj = CreateTextObject("TitleText", canvas.transform, "SELECT YOUR TEAM", 36, TextAlignmentOptions.Center, new Vector2(0, 150));
        
        // Create scroll view for team list
        GameObject scrollViewObj = new GameObject("TeamScrollView");
        scrollViewObj.transform.SetParent(canvas.transform, false);
        RectTransform scrollRectTransform = scrollViewObj.AddComponent<RectTransform>();
        scrollRectTransform.anchoredPosition = new Vector2(0, 0);
        scrollRectTransform.sizeDelta = new Vector2(500, 300);
        
        // Create confirm button
        GameObject confirmButtonObj = CreateButtonObject("ConfirmButton", canvas.transform, "CONFIRM", new Vector2(0, -180), new Vector2(200, 60));
        Button confirmButton = confirmButtonObj.GetComponent<Button>();
        
        // Set up TeamSelectionMenu references
        teamSelectionMenu.teamButtonPrefab = teamButtonPrefab;
        teamSelectionMenu.confirmButton = confirmButton;
        
        Debug.Log("Team Selection Scene setup complete!");
    }
    
    /// <summary>
    /// Sets up the Game Mode Selection scene with all required objects and components
    /// </summary>
    void SetupGameModeSelectionScene()
    {
        // Create GameManager
        CreateGameManager();
        
        // Create Canvas
        Canvas canvas = CreateCanvas();
        
        // Create GameModeSelection component
        GameModeSelection gameModeSelection = canvas.gameObject.AddComponent<GameModeSelection>();
        
        // Create basic UI elements (title, team info, buttons)
        GameObject titleObj = CreateTextObject("TitleText", canvas.transform, "SELECT GAME MODE", 36, TextAlignmentOptions.Center, new Vector2(0, 150));
        
        // Create team info panel
        GameObject teamInfoObj = new GameObject("TeamInfoPanel");
        teamInfoObj.transform.SetParent(canvas.transform, false);
        RectTransform teamInfoRect = teamInfoObj.AddComponent<RectTransform>();
        teamInfoRect.anchoredPosition = new Vector2(0, 80);
        teamInfoRect.sizeDelta = new Vector2(400, 80);
        
        // Add team name text
        GameObject teamNameObj = CreateTextObject("TeamNameText", teamInfoObj.transform, "TEAM NAME", 24, TextAlignmentOptions.Center, Vector2.zero);
        TextMeshProUGUI teamNameText = teamNameObj.GetComponent<TextMeshProUGUI>();
        
        // Create 2v2 match button
        GameObject twoVsTwoButtonObj = CreateButtonObject("2v2MatchButton", canvas.transform, "2v2 MATCH", new Vector2(0, 0), new Vector2(300, 80));
        Button twoVsTwoButton = twoVsTwoButtonObj.GetComponent<Button>();
        
        // Create 3-point contest button
        GameObject threePointButtonObj = CreateButtonObject("3PointContestButton", canvas.transform, "3-POINT CONTEST", new Vector2(0, -100), new Vector2(300, 80));
        Button threePointButton = threePointButtonObj.GetComponent<Button>();
        
        // Set up GameModeSelection references
        gameModeSelection.twoVsTwoButton = twoVsTwoButton;
        gameModeSelection.threePointContestButton = threePointButton;
        gameModeSelection.teamNameText = teamNameText;
        
        Debug.Log("Game Mode Selection Scene setup complete!");
    }
    
    /// <summary>
    /// Sets up the Main Game scene with all required objects and components
    /// </summary>
    void SetupMainGameScene()
    {
        // Create GameManager
        CreateGameManager();
        
        // Create court
        GameObject courtObj = new GameObject("Court");
        SpriteRenderer courtRenderer = courtObj.AddComponent<SpriteRenderer>();
        courtRenderer.sortingOrder = -1;
        
        // Create baskets
        GameObject leftBasketObj = new GameObject("LeftBasket");
        leftBasketObj.transform.position = new Vector3(-9, 0, 0);
        leftBasketObj.tag = "LeftBasket";
        
        GameObject rightBasketObj = new GameObject("RightBasket");
        rightBasketObj.transform.position = new Vector3(9, 0, 0);
        rightBasketObj.tag = "RightBasket";
        
        // Create court center
        GameObject courtCenterObj = new GameObject("CourtCenter");
        courtCenterObj.transform.position = Vector3.zero;
        
        // Create players
        GameObject playerOneObj = CreatePlayer("PlayerOne", new Vector3(-3, 1, 0), Color.blue);
        GameObject playerTwoObj = CreatePlayer("PlayerTwo", new Vector3(-3, -1, 0), Color.blue);
        GameObject opponentOneObj = CreatePlayer("OpponentOne", new Vector3(3, 1, 0), Color.red);
        GameObject opponentTwoObj = CreatePlayer("OpponentTwo", new Vector3(3, -1, 0), Color.red);
        
        // Create Canvas
        Canvas canvas = CreateCanvas();
        
        // Create GameController component
        GameController gameController = canvas.gameObject.AddComponent<GameController>();
        
        // Create basic UI elements (scoreboard, timer, messages)
        GameObject scoreboardObj = new GameObject("Scoreboard");
        scoreboardObj.transform.SetParent(canvas.transform, false);
        
        // Create player team name and score
        GameObject playerTeamObj = CreateTextObject("PlayerTeamName", scoreboardObj.transform, "PLAYER TEAM", 24, TextAlignmentOptions.Left, Vector2.zero);
        GameObject playerScoreObj = CreateTextObject("PlayerScore", scoreboardObj.transform, "0", 36, TextAlignmentOptions.Left, Vector2.zero);
        
        // Create opponent team name and score
        GameObject opponentTeamObj = CreateTextObject("OpponentTeamName", scoreboardObj.transform, "OPPONENT TEAM", 24, TextAlignmentOptions.Right, Vector2.zero);
        GameObject opponentScoreObj = CreateTextObject("OpponentScore", scoreboardObj.transform, "0", 36, TextAlignmentOptions.Right, Vector2.zero);
        
        // Create timer and quarter display
        GameObject timerObj = CreateTextObject("Timer", scoreboardObj.transform, "1:00", 36, TextAlignmentOptions.Center, Vector2.zero);
        GameObject quarterObj = CreateTextObject("Quarter", scoreboardObj.transform, "Q1", 24, TextAlignmentOptions.Center, Vector2.zero);
        
        // Create message text
        GameObject messageObj = CreateTextObject("MessageText", canvas.transform, "", 48, TextAlignmentOptions.Center, Vector2.zero);
        messageObj.SetActive(false);
        
        // Set up GameController references
        gameController.playerTeamNameText = playerTeamObj.GetComponent<TextMeshProUGUI>();
        gameController.opponentTeamNameText = opponentTeamObj.GetComponent<TextMeshProUGUI>();
        gameController.playerScoreText = playerScoreObj.GetComponent<TextMeshProUGUI>();
        gameController.opponentScoreText = opponentScoreObj.GetComponent<TextMeshProUGUI>();
        gameController.timerText = timerObj.GetComponent<TextMeshProUGUI>();
        gameController.quarterText = quarterObj.GetComponent<TextMeshProUGUI>();
        gameController.messageText = messageObj.GetComponent<TextMeshProUGUI>();
        gameController.playerOneObj = playerOneObj;
        gameController.playerTwoObj = playerTwoObj;
        gameController.opponentOneObj = opponentOneObj;
        gameController.opponentTwoObj = opponentTwoObj;
        gameController.ballPrefab = ballPrefab;
        gameController.courtCenter = courtCenterObj.transform;
        
        Debug.Log("Main Game Scene setup complete!");
    }
    
    /// <summary>
    /// Sets up the Three Point Contest scene with all required objects and components
    /// </summary>
    void SetupThreePointContestScene()
    {
        // Create GameManager
        CreateGameManager();
        
        // Create court
        GameObject courtObj = new GameObject("Court");
        SpriteRenderer courtRenderer = courtObj.AddComponent<SpriteRenderer>();
        courtRenderer.sortingOrder = -1;
        
        // Create basket
        GameObject basketObj = new GameObject("Basket");
        basketObj.transform.position = new Vector3(9, 0, 0);
        basketObj.tag = "Basket";
        
        // Create ball spawn point
        GameObject ballSpawnObj = new GameObject("BallSpawnPoint");
        ballSpawnObj.transform.position = new Vector3(0, 3, 0);
        
        // Create player
        GameObject playerObj = CreatePlayer("Player", new Vector3(0, 0, 0), Color.blue);
        
        // Create shooting spots
        ShootingSpot[] shootingSpots = new ShootingSpot[5];
        float[] spotPositionsX = { -8, -4, 0, 4, 8 };
        float[] spotPositionsY = { 3, 4, 4, 4, 3 };
        
        for (int i = 0; i < 5; i++)
        {
            GameObject spotObj;
            
            if (shootingSpotPrefab != null)
            {
                spotObj = Instantiate(shootingSpotPrefab, new Vector3(spotPositionsX[i], spotPositionsY[i], 0), Quaternion.identity);
            }
            else
            {
                spotObj = new GameObject("ShootingSpot" + (i + 1));
                spotObj.transform.position = new Vector3(spotPositionsX[i], spotPositionsY[i], 0);
                shootingSpots[i] = spotObj.AddComponent<ShootingSpot>();
            }
        }
        
        // Create Canvas
        Canvas canvas = CreateCanvas();
        
        // Create ThreePointContest component
        ThreePointContest threePointContest = canvas.gameObject.AddComponent<ThreePointContest>();
        
        // Create basic UI elements
        GameObject timerObj = CreateTextObject("Timer", canvas.transform, "60", 48, TextAlignmentOptions.Center, new Vector2(0, 200));
        GameObject scoreObj = CreateTextObject("Score", canvas.transform, "0", 48, TextAlignmentOptions.Left, new Vector2(100, 200));
        GameObject teamNameObj = CreateTextObject("TeamName", canvas.transform, "TEAM NAME", 24, TextAlignmentOptions.Right, new Vector2(-100, 200));
        
        // Create message text
        GameObject messageObj = CreateTextObject("MessageText", canvas.transform, "", 48, TextAlignmentOptions.Center, Vector2.zero);
        messageObj.SetActive(false);
        
        // Create results panel
        GameObject resultsObj = new GameObject("ResultsPanel");
        resultsObj.transform.SetParent(canvas.transform, false);
        resultsObj.SetActive(false);
        
        // Set up ThreePointContest references
        threePointContest.timerText = timerObj.GetComponent<TextMeshProUGUI>();
        threePointContest.scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
        threePointContest.teamNameText = teamNameObj.GetComponent<TextMeshProUGUI>();
        threePointContest.messageText = messageObj.GetComponent<TextMeshProUGUI>();
        threePointContest.resultsPanel = resultsObj;
        threePointContest.playerObject = playerObj;
        threePointContest.ballPrefab = ballPrefab;
        threePointContest.ballSpawnPoint = ballSpawnObj.transform;
        threePointContest.basketTransform = basketObj.transform;
        
        Debug.Log("Three Point Contest Scene setup complete!");
    }
    
    /// <summary>
    /// Creates a GameManager object if one doesn't already exist
    /// </summary>
    GameObject CreateGameManager()
    {
        GameObject gameManagerObj = GameObject.Find("GameManager");
        
        if (gameManagerObj == null)
        {
            gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
            Debug.Log("Created GameManager");
        }
        else
        {
            Debug.Log("GameManager already exists");
        }
        
        return gameManagerObj;
    }
    
    /// <summary>
    /// Creates a Canvas for UI elements
    /// </summary>
    Canvas CreateCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create EventSystem if needed
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        return canvas;
    }
    
    /// <summary>
    /// Creates a player GameObject with required components
    /// </summary>
    GameObject CreatePlayer(string name, Vector3 position, Color color)
    {
        GameObject playerObj;
        
        if (playerPrefab != null)
        {
            playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
            playerObj.name = name;
        }
        else
        {
            playerObj = new GameObject(name);
            playerObj.transform.position = position;
            
            // Add required components
            SpriteRenderer spriteRenderer = playerObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            
            Rigidbody2D rb = playerObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            
            CircleCollider2D collider = playerObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            
            playerObj.AddComponent<PlayerController>();
        }
        
        return playerObj;
    }
    
    /// <summary>
    /// Creates a TextMeshPro text object
    /// </summary>
    GameObject CreateTextObject(string name, Transform parent, string text, int fontSize, TextAlignmentOptions alignment, Vector2 position)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        
        return textObj;
    }
    
    /// <summary>
    /// Creates a button with text
    /// </summary>
    GameObject CreateButtonObject(string name, Transform parent, string text, Vector2 position, Vector2 size)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        Button button = buttonObj.AddComponent<Button>();
        
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;
        
        // Add text
        GameObject textObj = CreateTextObject(name + "Text", buttonObj.transform, text, 24, TextAlignmentOptions.Center, Vector2.zero);
        
        return buttonObj;
    }
}
#endif

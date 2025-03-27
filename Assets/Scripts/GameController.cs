using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI playerTeamNameText;
    public TextMeshProUGUI opponentTeamNameText;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI opponentScoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI quarterText;
    public TextMeshProUGUI messageText;
    public Image playerTeamLogo;
    public Image opponentTeamLogo;
    
    [Header("Game Settings")]
    public float quarterLength = 60f; // 1 minute per quarter
    public int totalQuarters = 4;
    public float shotClockDuration = 24f;
    public GameObject ballPrefab;
    public Transform courtCenter;
    
    [Header("Player References")]
    public GameObject playerOneObj;
    public GameObject playerTwoObj;
    public GameObject opponentOneObj;
    public GameObject opponentTwoObj;
    
    [Header("Audio")]
    public AudioClip whistleSound;
    public AudioClip crowdCheerSound;
    public AudioClip buzzerSound;
    
    // Private variables
    private int playerScore = 0;
    private int opponentScore = 0;
    private float gameTime;
    private float shotClockTime;
    private int currentQuarter = 1;
    private bool isGameActive = false;
    private bool isGamePaused = false;
    private bool isHalftime = false;
    private GameObject ballObj;
    private AudioSource audioSource;
    
    // Special effects
    private bool isPlayerOnFire = false;
    private bool isOpponentOnFire = false;
    private int playerConsecutiveBaskets = 0;
    private int opponentConsecutiveBaskets = 0;

    void Start()
    {
        // Initialize audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Set up the game
        SetupGame();
        
        // Start the game after a short delay
        StartCoroutine(StartGameAfterDelay(3f));
    }
    
    void SetupGame()
    {
        // Initialize scores and timer
        playerScore = 0;
        opponentScore = 0;
        gameTime = quarterLength;
        shotClockTime = shotClockDuration;
        currentQuarter = 1;
        
        // Update UI
        UpdateScoreUI();
        UpdateTimerUI();
        
        // Set team information from GameManager
        if (GameManager.Instance != null)
        {
            Team playerTeam = GameManager.Instance.SelectedTeam;
            Team opponentTeam = GameManager.Instance.OpponentTeam;
            
            if (playerTeam != null && opponentTeam != null)
            {
                // Set team names
                playerTeamNameText.text = playerTeam.Name;
                opponentTeamNameText.text = opponentTeam.Name;
                
                // Set team colors
                playerTeamNameText.color = playerTeam.PrimaryColor;
                opponentTeamNameText.color = opponentTeam.PrimaryColor;
                
                if (playerTeamLogo != null)
                {
                    playerTeamLogo.color = playerTeam.PrimaryColor;
                }
                
                if (opponentTeamLogo != null)
                {
                    opponentTeamLogo.color = opponentTeam.PrimaryColor;
                }
                
                // Set player team colors
                SetupPlayerAppearance(playerOneObj, playerTeam, true, 0);
                SetupPlayerAppearance(playerTwoObj, playerTeam, true, 1);
                SetupPlayerAppearance(opponentOneObj, opponentTeam, false, 0);
                SetupPlayerAppearance(opponentTwoObj, opponentTeam, false, 1);
            }
        }
        
        // Create the ball
        SpawnBall();
        
        // Set up player references
        SetupPlayerReferences();
        
        // Hide message text initially
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }
    
    void SetupPlayerAppearance(GameObject playerObj, Team team, bool isPlayerTeam, int playerIndex)
    {
        if (playerObj != null && team != null)
        {
            // Set player color
            SpriteRenderer spriteRenderer = playerObj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = team.PrimaryColor;
            }
            
            // Set player stats
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null && team.Players.Count > playerIndex)
            {
                Player playerData = team.Players[playerIndex];
                playerController.speedStat = playerData.Speed;
                playerController.shootingStat = playerData.Shooting;
                playerController.dunkingStat = playerData.Dunking;
                
                // Set AI flag
                playerController.isAI = !isPlayerTeam;
                playerController.isPlayerOne = isPlayerTeam;
            }
        }
    }
    
    void SetupPlayerReferences()
    {
        // Set up player references to each other and the ball
        if (playerOneObj != null && playerTwoObj != null && opponentOneObj != null && opponentTwoObj != null && ballObj != null)
        {
            // Get player controllers
            PlayerController p1 = playerOneObj.GetComponent<PlayerController>();
            PlayerController p2 = playerTwoObj.GetComponent<PlayerController>();
            PlayerController o1 = opponentOneObj.GetComponent<PlayerController>();
            PlayerController o2 = opponentTwoObj.GetComponent<PlayerController>();
            
            if (p1 != null && p2 != null && o1 != null && o2 != null)
            {
                // Set ball reference
                p1.ball = ballObj.transform;
                p2.ball = ballObj.transform;
                o1.ball = ballObj.transform;
                o2.ball = ballObj.transform;
                
                // Set basket references (assuming baskets are at x = -10 and x = 10)
                Transform leftBasket = GameObject.FindGameObjectWithTag("LeftBasket")?.transform;
                Transform rightBasket = GameObject.FindGameObjectWithTag("RightBasket")?.transform;
                
                if (leftBasket != null && rightBasket != null)
                {
                    // Player team baskets
                    p1.basket = leftBasket;
                    p1.opponentBasket = rightBasket;
                    p2.basket = leftBasket;
                    p2.opponentBasket = rightBasket;
                    
                    // Opponent team baskets
                    o1.basket = rightBasket;
                    o1.opponentBasket = leftBasket;
                    o2.basket = rightBasket;
                    o2.opponentBasket = leftBasket;
                }
                
                // Set teammate references
                p1.teammates = new Transform[] { p2.transform };
                p2.teammates = new Transform[] { p1.transform };
                o1.teammates = new Transform[] { o2.transform };
                o2.teammates = new Transform[] { o1.transform };
                
                // Set opponent references
                p1.opponents = new Transform[] { o1.transform, o2.transform };
                p2.opponents = new Transform[] { o1.transform, o2.transform };
                o1.opponents = new Transform[] { p1.transform, p2.transform };
                o2.opponents = new Transform[] { p1.transform, p2.transform };
            }
        }
    }
    
    void SpawnBall()
    {
        // Create the ball at center court
        if (ballPrefab != null && courtCenter != null)
        {
            ballObj = Instantiate(ballPrefab, courtCenter.position, Quaternion.identity);
            
            // Give the ball to a random player to start
            StartCoroutine(GiveBallToRandomPlayer(1f));
        }
    }
    
    IEnumerator GiveBallToRandomPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Randomly decide which team gets the ball
        bool playerTeamStarts = Random.value > 0.5f;
        
        if (playerTeamStarts)
        {
            // Give ball to player one
            if (playerOneObj != null && ballObj != null)
            {
                PlayerController playerController = playerOneObj.GetComponent<PlayerController>();
                BallController ballController = ballObj.GetComponent<BallController>();
                
                if (playerController != null && ballController != null)
                {
                    playerController.SetHasBall(true);
                    ballController.PickUp(playerOneObj);
                }
            }
        }
        else
        {
            // Give ball to opponent one
            if (opponentOneObj != null && ballObj != null)
            {
                PlayerController opponentController = opponentOneObj.GetComponent<PlayerController>();
                BallController ballController = ballObj.GetComponent<BallController>();
                
                if (opponentController != null && ballController != null)
                {
                    opponentController.SetHasBall(true);
                    ballController.PickUp(opponentOneObj);
                }
            }
        }
        
        // Reset shot clock
        shotClockTime = shotClockDuration;
    }
    
    IEnumerator StartGameAfterDelay(float delay)
    {
        // Show countdown message
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            
            for (int i = 3; i > 0; i--)
            {
                messageText.text = i.ToString();
                
                // Play whistle sound at the start of countdown
                if (i == 3 && audioSource != null && whistleSound != null)
                {
                    audioSource.PlayOneShot(whistleSound);
                }
                
                yield return new WaitForSeconds(1f);
            }
            
            messageText.text = "GO!";
            yield return new WaitForSeconds(0.5f);
            messageText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }
        
        // Start the game
        isGameActive = true;
    }

    void Update()
    {
        if (isGameActive && !isGamePaused)
        {
            // Update game timer
            gameTime -= Time.deltaTime;
            
            // Update shot clock
            shotClockTime -= Time.deltaTime;
            
            // Check for shot clock violation
            if (shotClockTime <= 0)
            {
                HandleShotClockViolation();
            }
            
            // Check for end of quarter
            if (gameTime <= 0)
            {
                EndQuarter();
            }
            
            // Update UI
            UpdateTimerUI();
        }
        
        // Handle pause input
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    void UpdateTimerUI()
    {
        // Update main game timer
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        
        // Update quarter display
        quarterText.text = "Q" + currentQuarter;
        
        // Update shot clock (could be a separate UI element)
        // shotClockText.text = Mathf.CeilToInt(shotClockTime).ToString();
    }
    
    void UpdateScoreUI()
    {
        playerScoreText.text = playerScore.ToString();
        opponentScoreText.text = opponentScore.ToString();
    }
    
    void HandleShotClockViolation()
    {
        // Reset shot clock
        shotClockTime = shotClockDuration;
        
        // Change possession
        ChangePossession();
        
        // Show message
        ShowTemporaryMessage("Shot Clock Violation!", 1.5f);
        
        // Play whistle sound
        if (audioSource != null && whistleSound != null)
        {
            audioSource.PlayOneShot(whistleSound);
        }
    }
    
    void ChangePossession()
    {
        // Find who has the ball
        BallController ballController = ballObj?.GetComponent<BallController>();
        if (ballController != null)
        {
            GameObject currentHolder = ballController.GetCurrentHolder();
            
            if (currentHolder != null)
            {
                PlayerController holderController = currentHolder.GetComponent<PlayerController>();
                if (holderController != null)
                {
                    // Remove ball from current holder
                    holderController.SetHasBall(false);
                    
                    // Give ball to the opposite team
                    bool isPlayerTeam = holderController.isPlayerOne;
                    GameObject newHolder = isPlayerTeam ? opponentOneObj : playerOneObj;
                    
                    // Position the ball and new holder at center court
                    if (newHolder != null && courtCenter != null)
                    {
                        newHolder.transform.position = courtCenter.position + new Vector3(-2f, 0, 0);
                        
                        PlayerController newHolderController = newHolder.GetComponent<PlayerController>();
                        if (newHolderController != null)
                        {
                            newHolderController.SetHasBall(true);
                            ballController.PickUp(newHolder);
                        }
                    }
                }
            }
            else
            {
                // If no one has the ball, reset it to center court
                ballController.ResetBall();
                StartCoroutine(GiveBallToRandomPlayer(0.5f));
            }
        }
    }
    
    void EndQuarter()
    {
        // Reset timer
        gameTime = 0;
        
        // Play buzzer sound
        if (audioSource != null && buzzerSound != null)
        {
            audioSource.PlayOneShot(buzzerSound);
        }
        
        // Check if game is over
        if (currentQuarter >= totalQuarters)
        {
            EndGame();
        }
        else
        {
            // Move to next quarter or halftime
            currentQuarter++;
            
            if (currentQuarter == 3)
            {
                // Halftime
                isHalftime = true;
                ShowTemporaryMessage("Halftime!", 3f);
                StartCoroutine(StartNextQuarter(5f)); // Longer break for halftime
            }
            else
            {
                // Regular quarter break
                string quarterMessage = "End of Q" + (currentQuarter - 1) + "!";
                ShowTemporaryMessage(quarterMessage, 2f);
                StartCoroutine(StartNextQuarter(3f));
            }
        }
    }
    
    IEnumerator StartNextQuarter(float delay)
    {
        // Pause the game during the break
        isGameActive = false;
        
        yield return new WaitForSeconds(delay);
        
        // Reset for next quarter
        gameTime = quarterLength;
        shotClockTime = shotClockDuration;
        
        // Reset ball position
        if (ballObj != null)
        {
            BallController ballController = ballObj.GetComponent<BallController>();
            if (ballController != null)
            {
                ballController.ResetBall();
            }
        }
        
        // Reset player positions
        ResetPlayerPositions();
        
        // Show quarter start message
        string startMessage = isHalftime ? "Start of Second Half!" : "Start of Q" + currentQuarter + "!";
        ShowTemporaryMessage(startMessage, 2f);
        
        // Reset halftime flag if needed
        if (isHalftime)
        {
            isHalftime = false;
        }
        
        // Give ball to a team
        StartCoroutine(GiveBallToRandomPlayer(1f));
        
        // Resume the game
        isGameActive = true;
    }
    
    void ResetPlayerPositions()
    {
        // Reset player positions to their starting positions
        if (courtCenter != null)
        {
            if (playerOneObj != null)
                playerOneObj.transform.position = courtCenter.position + new Vector3(-3f, 1f, 0);
                
            if (playerTwoObj != null)
                playerTwoObj.transform.position = courtCenter.position + new Vector3(-3f, -1f, 0);
                
            if (opponentOneObj != null)
                opponentOneObj.transform.position = courtCenter.position + new Vector3(3f, 1f, 0);
                
            if (opponentTwoObj != null)
                opponentTwoObj.transform.position = courtCenter.position + new Vector3(3f, -1f, 0);
        }
    }
    
    void EndGame()
    {
        isGameActive = false;
        
        // Determine winner
        string endMessage;
        if (playerScore > opponentScore)
        {
            endMessage = "You Win!";
        }
        else if (opponentScore > playerScore)
        {
            endMessage = "You Lose!";
        }
        else
        {
            endMessage = "It's a Tie!";
        }
        
        // Show end game message
        ShowTemporaryMessage(endMessage, 5f);
        
        // Play appropriate sound
        if (audioSource != null && crowdCheerSound != null)
        {
            audioSource.PlayOneShot(crowdCheerSound);
        }
        
        // Return to menu after delay
        StartCoroutine(ReturnToMenuAfterDelay(7f));
    }
    
    IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Return to game mode selection
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGameModeSelectionScene();
        }
    }
    
    void TogglePause()
    {
        isGamePaused = !isGamePaused;
        
        // Show pause message
        if (messageText != null)
        {
            messageText.gameObject.SetActive(isGamePaused);
            messageText.text = isGamePaused ? "PAUSED" : "";
        }
        
        // Pause physics and animations
        Time.timeScale = isGamePaused ? 0f : 1f;
    }
    
    void ShowTemporaryMessage(string message, float duration)
    {
        StartCoroutine(ShowMessageCoroutine(message, duration));
    }
    
    IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = message;
            
            yield return new WaitForSeconds(duration);
            
            messageText.gameObject.SetActive(false);
        }
    }
    
    public void ScorePoints(string team, int points)
    {
        // Reset shot clock
        shotClockTime = shotClockDuration;
        
        // Update score
        if (team == "Player")
        {
            playerScore += points;
            playerConsecutiveBaskets++;
            opponentConsecutiveBaskets = 0;
            
            // Check for "on fire" status
            if (playerConsecutiveBaskets >= 3)
            {
                isPlayerOnFire = true;
                ShowTemporaryMessage("Player On Fire!", 2f);
                
                // Apply "on fire" effects to player team
                ApplyOnFireEffects(true);
            }
        }
        else // "Opponent"
        {
            opponentScore += points;
            opponentConsecutiveBaskets++;
            playerConsecutiveBaskets = 0;
            
            // Check for "on fire" status
            if (opponentConsecutiveBaskets >= 3)
            {
                isOpponentOnFire = true;
                ShowTemporaryMessage("Opponent On Fire!", 2f);
                
                // Apply "on fire" effects to opponent team
                ApplyOnFireEffects(false);
            }
        }
        
        // Update UI
        UpdateScoreUI();
        
        // Show points message
        ShowTemporaryMessage("+" + points + " Points!", 1f);
        
        // Play crowd cheer
        if (audioSource != null && crowdCheerSound != null)
        {
            audioSource.PlayOneShot(crowdCheerSound, 0.7f);
        }
        
        // Change possession
        ChangePossession();
    }
    
    void ApplyOnFireEffects(bool isPlayerTeam)
    {
        // Apply visual effects and stat boosts to the "on fire" team
        if (isPlayerTeam)
        {
            // Boost player stats
            ApplyStatBoost(playerOneObj, 1.5f);
            ApplyStatBoost(playerTwoObj, 1.5f);
            
            // Add visual effects (would be implemented with particle systems)
            // AddFireEffect(playerOneObj);
            // AddFireEffect(playerTwoObj);
            
            // Schedule the effect to end after some time
            StartCoroutine(EndOnFireEffect(true, 15f));
        }
        else
        {
            // Boost opponent stats
            ApplyStatBoost(opponentOneObj, 1.5f);
            ApplyStatBoost(opponentTwoObj, 1.5f);
            
            // Add visual effects
            // AddFireEffect(opponentOneObj);
            // AddFireEffect(opponentTwoObj);
            
            // Schedule the effect to end
            StartCoroutine(EndOnFireEffect(false, 15f));
        }
    }
    
    void ApplyStatBoost(GameObject playerObj, float multiplier)
    {
        if (playerObj != null)
        {
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Boost speed, shooting, and dunking
                playerController.speed *= multiplier;
                playerController.shootingStat *= multiplier;
                playerController.dunkingStat *= multiplier;
            }
        }
    }
    
    IEnumerator EndOnFireEffect(bool isPlayerTeam, float duration)
    {
        yield return new WaitForSeconds(duration);
        
        if (isPlayerTeam)
        {
            isPlayerOnFire = false;
            playerConsecutiveBaskets = 0;
            
            // Reset stats
            ResetPlayerStats(playerOneObj);
            ResetPlayerStats(playerTwoObj);
            
            // Remove visual effects
            // RemoveFireEffect(playerOneObj);
            // RemoveFireEffect(playerTwoObj);
        }
        else
        {
            isOpponentOnFire = false;
            opponentConsecutiveBaskets = 0;
            
            // Reset stats
            ResetPlayerStats(opponentOneObj);
            ResetPlayerStats(opponentTwoObj);
            
            // Remove visual effects
            // RemoveFireEffect(opponentOneObj);
            // RemoveFireEffect(opponentTwoObj);
        }
    }
    
    void ResetPlayerStats(GameObject playerObj)
    {
        if (playerObj != null)
        {
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Reset to base stats (this is simplified; would need to store original values)
                if (GameManager.Instance != null)
                {
                    Team team = playerController.isPlayerOne ? 
                        GameManager.Instance.SelectedTeam : 
                        GameManager.Instance.OpponentTeam;
                    
                    if (team != null && team.Players.Count > 0)
                    {
                        Player playerData = team.Players[0]; // Simplified
                        playerController.speed = 5f + (playerData.Speed * 0.5f);
                        playerController.shootingStat = playerData.Shooting;
                        playerController.dunkingStat = playerData.Dunking;
                    }
                }
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ThreePointContest : MonoBehaviour
{
    [Header("Game Settings")]
    public float timeLimit = 60f;
    public int ballsPerRack = 5;
    public int moneyBallsPerRack = 1;
    public float ballRespawnDelay = 0.5f;
    
    [Header("References")]
    public ShootingSpot[] shootingSpots;
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    public Transform basketTransform;
    public GameObject playerObject;
    
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI teamNameText;
    public TextMeshProUGUI messageText;
    public Image progressBar;
    public GameObject resultsPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    
    [Header("Audio")]
    public AudioClip whistleSound;
    public AudioClip buzzerSound;
    public AudioClip scoreSound;
    public AudioClip moneyBallSound;
    public AudioClip crowdCheerSound;
    
    // Private variables
    private float timeRemaining;
    private int score = 0;
    private int currentSpotIndex = 0;
    private int shotsTakenAtSpot = 0;
    private bool isGameActive = false;
    private bool isGamePaused = false;
    private GameObject currentBall;
    private AudioSource audioSource;
    private int highScore = 0;
    private bool isMoneyBallActive = false;
    private List<GameObject> activeBalls = new List<GameObject>();

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
        
        // Hide results panel initially
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }
        
        // Start the game after a short delay
        StartCoroutine(StartGameAfterDelay(3f));
    }
    
    void SetupGame()
    {
        // Initialize variables
        timeRemaining = timeLimit;
        score = 0;
        currentSpotIndex = 0;
        shotsTakenAtSpot = 0;
        isGameActive = false;
        
        // Update UI
        UpdateTimerUI();
        UpdateScoreUI();
        
        // Set team information from GameManager
        if (GameManager.Instance != null && GameManager.Instance.SelectedTeam != null)
        {
            Team playerTeam = GameManager.Instance.SelectedTeam;
            
            // Set team name and color
            if (teamNameText != null)
            {
                teamNameText.text = playerTeam.Name;
                teamNameText.color = playerTeam.PrimaryColor;
            }
            
            // Set player appearance
            if (playerObject != null)
            {
                SpriteRenderer playerSprite = playerObject.GetComponent<SpriteRenderer>();
                if (playerSprite != null)
                {
                    playerSprite.color = playerTeam.PrimaryColor;
                }
                
                // Set player stats if applicable
                PlayerController playerController = playerObject.GetComponent<PlayerController>();
                if (playerController != null && playerTeam.Players.Count > 0)
                {
                    Player playerData = playerTeam.Players[0];
                    playerController.shootingStat = playerData.Shooting;
                    
                    // Disable AI for the player
                    playerController.isAI = false;
                }
            }
        }
        
        // Set up shooting spots
        if (shootingSpots.Length > 0)
        {
            // Mark the last ball in each rack as a money ball
            for (int i = 0; i < shootingSpots.Length; i++)
            {
                shootingSpots[i].VacateSpot();
                shootingSpots[i].SetMoneyBall(false);
            }
        }
        
        // Hide message text initially
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
        
        // Load high score (in a real game, this would use PlayerPrefs)
        highScore = 30; // Example high score
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
        
        // Move to first spot
        MoveToNextSpot();
        
        // Spawn first ball
        SpawnBall();
    }

    void Update()
    {
        if (isGameActive && !isGamePaused)
        {
            // Update timer
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(0, timeRemaining);
            UpdateTimerUI();
            
            // Check for game over
            if (timeRemaining <= 0)
            {
                EndGame();
            }
            
            // Check for player input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttemptShot();
            }
            
            // Check if player has moved to a different spot
            CheckPlayerPosition();
        }
        
        // Handle pause input
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.FloorToInt(timeRemaining);
            timerText.text = seconds.ToString();
            
            // Change color when time is running low
            if (seconds <= 10)
            {
                timerText.color = Color.red;
            }
        }
        
        // Update progress bar
        if (progressBar != null)
        {
            progressBar.fillAmount = timeRemaining / timeLimit;
        }
    }
    
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
    
    void CheckPlayerPosition()
    {
        if (playerObject != null && currentSpotIndex < shootingSpots.Length)
        {
            // Check if player has moved away from current spot
            if (!shootingSpots[currentSpotIndex].IsPlayerInSpot(playerObject))
            {
                // Find if player is in a different spot
                for (int i = 0; i < shootingSpots.Length; i++)
                {
                    if (i != currentSpotIndex && shootingSpots[i].IsPlayerInSpot(playerObject))
                    {
                        // Player has moved to a different spot
                        shootingSpots[currentSpotIndex].VacateSpot();
                        currentSpotIndex = i;
                        shotsTakenAtSpot = 0;
                        shootingSpots[currentSpotIndex].OccupySpot(playerObject);
                        
                        // Reset money ball status
                        isMoneyBallActive = false;
                        
                        // Clear any existing balls
                        ClearActiveBalls();
                        
                        // Spawn a new ball
                        SpawnBall();
                        
                        break;
                    }
                }
            }
        }
    }
    
    void MoveToNextSpot()
    {
        if (currentSpotIndex < shootingSpots.Length)
        {
            // Vacate current spot if occupied
            if (currentSpotIndex > 0)
            {
                shootingSpots[currentSpotIndex - 1].VacateSpot();
            }
            
            // Occupy new spot
            shootingSpots[currentSpotIndex].OccupySpot(playerObject);
            
            // Reset shots taken
            shotsTakenAtSpot = 0;
            
            // Reset money ball status
            isMoneyBallActive = false;
        }
        else
        {
            // All spots completed
            EndGame();
        }
    }
    
    void SpawnBall()
    {
        if (ballPrefab != null && ballSpawnPoint != null)
        {
            // Check if this should be a money ball
            isMoneyBallActive = (shotsTakenAtSpot == ballsPerRack - 1);
            
            // Create the ball
            currentBall = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
            activeBalls.Add(currentBall);
            
            // Set ball appearance for money ball
            if (isMoneyBallActive)
            {
                SpriteRenderer ballSprite = currentBall.GetComponent<SpriteRenderer>();
                if (ballSprite != null)
                {
                    ballSprite.color = Color.red; // Money ball is red
                }
            }
            
            // Give the ball to the player
            BallController ballController = currentBall.GetComponent<BallController>();
            if (ballController != null && playerObject != null)
            {
                ballController.PickUp(playerObject);
                
                PlayerController playerController = playerObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.SetHasBall(true);
                }
            }
        }
    }
    
    void AttemptShot()
    {
        if (currentBall != null && playerObject != null)
        {
            // Get controllers
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            BallController ballController = currentBall.GetComponent<BallController>();
            
            if (playerController != null && ballController != null && playerController.HasBall())
            {
                // Calculate direction to basket
                Vector2 directionToBasket = (basketTransform.position - playerObject.transform.position).normalized;
                
                // Calculate force based on shooting stat and distance
                float distance = Vector2.Distance(playerObject.transform.position, basketTransform.position);
                float shootingStat = playerController.shootingStat;
                float forceMagnitude = 10f + (shootingStat * 0.5f);
                
                // Add some arc to the shot
                Vector2 shootForceVector = directionToBasket * forceMagnitude;
                shootForceVector.y += distance * 0.5f;
                
                // Shoot the ball
                ballController.Shoot(playerObject.transform.position, shootForceVector, playerObject);
                playerController.SetHasBall(false);
                
                // Start tracking this shot
                StartCoroutine(TrackShot(currentBall));
                
                // Increment shots taken
                shotsTakenAtSpot++;
                
                // Spawn next ball after delay if we haven't taken all shots from this spot
                if (shotsTakenAtSpot < ballsPerRack)
                {
                    StartCoroutine(SpawnBallAfterDelay(ballRespawnDelay));
                }
                else
                {
                    // Move to next spot after all balls are shot
                    currentSpotIndex++;
                    if (currentSpotIndex < shootingSpots.Length)
                    {
                        StartCoroutine(MoveToNextSpotAfterDelay(1.5f));
                    }
                }
            }
        }
    }
    
    IEnumerator TrackShot(GameObject ball)
    {
        if (ball != null)
        {
            bool scored = false;
            float trackingTime = 0f;
            
            // Track the ball for up to 3 seconds
            while (trackingTime < 3f && !scored && ball != null)
            {
                trackingTime += Time.deltaTime;
                
                // Check if ball is near the basket
                if (basketTransform != null)
                {
                    float distanceToBasket = Vector2.Distance(ball.transform.position, basketTransform.position);
                    
                    if (distanceToBasket < 1f)
                    {
                        // Determine if shot was successful based on player skill and some randomness
                        float successChance = 0.5f; // Base 50% chance
                        
                        // Adjust based on player's shooting stat (0-10)
                        PlayerController playerController = playerObject.GetComponent<PlayerController>();
                        if (playerController != null)
                        {
                            successChance += playerController.shootingStat * 0.05f; // Up to +50% for max stat
                        }
                        
                        // Adjust based on distance
                        float distanceFromSpot = Vector2.Distance(
                            shootingSpots[currentSpotIndex].transform.position, 
                            basketTransform.position);
                        successChance -= distanceFromSpot * 0.02f; // Harder from further away
                        
                        // Random roll
                        if (Random.value < successChance)
                        {
                            // Successful shot
                            OnShotMade(isMoneyBallActive);
                            scored = true;
                        }
                        else
                        {
                            // Missed shot
                            OnShotMissed();
                        }
                        
                        // Destroy the ball
                        if (ball != null)
                        {
                            activeBalls.Remove(ball);
                            Destroy(ball);
                        }
                        break;
                    }
                }
                
                yield return null;
            }
            
            // If we exited the loop without scoring, it's a miss
            if (!scored && ball != null)
            {
                OnShotMissed();
                activeBalls.Remove(ball);
                Destroy(ball);
            }
        }
    }
    
    void OnShotMade(bool isMoneyBall)
    {
        // Award points
        int points = isMoneyBall ? 2 : 1;
        score += points;
        
        // Update UI
        UpdateScoreUI();
        
        // Show message
        ShowTemporaryMessage("+" + points + " Points!", 1f);
        
        // Play sound
        if (audioSource != null)
        {
            AudioClip clipToPlay = isMoneyBall ? moneyBallSound : scoreSound;
            if (clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay);
            }
        }
    }
    
    void OnShotMissed()
    {
        // Show message
        ShowTemporaryMessage("Miss!", 0.5f);
    }
    
    IEnumerator SpawnBallAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (isGameActive)
        {
            SpawnBall();
        }
    }
    
    IEnumerator MoveToNextSpotAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (isGameActive)
        {
            MoveToNextSpot();
            SpawnBall();
        }
    }
    
    void ClearActiveBalls()
    {
        foreach (GameObject ball in activeBalls)
        {
            if (ball != null)
            {
                Destroy(ball);
            }
        }
        
        activeBalls.Clear();
        currentBall = null;
    }
    
    void EndGame()
    {
        isGameActive = false;
        
        // Play buzzer sound
        if (audioSource != null && buzzerSound != null)
        {
            audioSource.PlayOneShot(buzzerSound);
        }
        
        // Clear any remaining balls
        ClearActiveBalls();
        
        // Check for high score
        bool isNewHighScore = score > highScore;
        if (isNewHighScore)
        {
            highScore = score;
            // In a real game, save the high score using PlayerPrefs
            // PlayerPrefs.SetInt("ThreePointHighScore", highScore);
            // PlayerPrefs.Save();
        }
        
        // Show results
        ShowResults(isNewHighScore);
    }
    
    void ShowResults(bool isNewHighScore)
    {
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
            
            // Update final score
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + score;
            }
            
            // Update high score
            if (highScoreText != null)
            {
                highScoreText.text = "High Score: " + highScore;
                
                if (isNewHighScore)
                {
                    highScoreText.text += " (New!)";
                    highScoreText.color = Color.yellow;
                }
            }
            
            // Play appropriate sound
            if (audioSource != null)
            {
                if (isNewHighScore && crowdCheerSound != null)
                {
                    audioSource.PlayOneShot(crowdCheerSound);
                }
            }
        }
        
        // Return to menu after delay
        StartCoroutine(ReturnToMenuAfterDelay(5f));
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
            
            if (messageText.text == message) // Only hide if it hasn't been changed
            {
                messageText.gameObject.SetActive(false);
            }
        }
    }
}
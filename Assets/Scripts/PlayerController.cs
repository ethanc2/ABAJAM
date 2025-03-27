using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed = 5f;
    public float jumpForce = 5f;
    public float shootForce = 10f;
    public bool isAI = false;
    public bool isPlayerOne = true;
    
    [Header("References")]
    public Transform ball;
    public Transform basket;
    public Transform opponentBasket;
    public Transform[] teammates;
    public Transform[] opponents;
    
    [Header("Player Stats")]
    public float speedStat = 7f;
    public float shootingStat = 7f;
    public float dunkingStat = 7f;
    
    // Private variables
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool hasBall = false;
    private bool isJumping = false;
    private bool isDunking = false;
    private float aiDecisionTimer = 0f;
    private Vector2 moveDirection = Vector2.zero;
    private Team playerTeam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set player team and appearance
        if (GameManager.Instance != null)
        {
            playerTeam = isPlayerOne ? GameManager.Instance.SelectedTeam : GameManager.Instance.OpponentTeam;
            if (playerTeam != null)
            {
                spriteRenderer.color = playerTeam.PrimaryColor;
                
                // Set player stats based on team data
                if (playerTeam.Players.Count > 0)
                {
                    Player playerData = playerTeam.Players[0]; // Use first player for now
                    speedStat = playerData.Speed;
                    shootingStat = playerData.Shooting;
                    dunkingStat = playerData.Dunking;
                    
                    // Adjust actual speed based on speed stat
                    speed = 5f + (speedStat * 0.5f);
                }
            }
        }
    }

    void Update()
    {
        if (isAI)
        {
            AIBehavior();
        }
        else
        {
            PlayerMovement();
        }
        
        // Update animations
        UpdateAnimations();
    }
    
    void UpdateAnimations()
    {
        // Set animation parameters based on state
        if (animator != null)
        {
            animator.SetBool("IsRunning", moveDirection.magnitude > 0.1f);
            animator.SetBool("HasBall", hasBall);
            animator.SetBool("IsJumping", isJumping);
            animator.SetBool("IsDunking", isDunking);
            
            // Set direction (for sprite flipping)
            if (moveDirection.x != 0)
            {
                spriteRenderer.flipX = moveDirection.x < 0;
            }
        }
    }

    void PlayerMovement()
    {
        // Get input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        
        // Create movement vector
        moveDirection = new Vector2(moveHorizontal, moveVertical).normalized;
        
        // Apply movement
        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
        
        // Shooting/passing
        if (Input.GetKeyDown(KeyCode.Space) && hasBall)
        {
            // Determine if we're close enough to dunk
            float distanceToBasket = Vector2.Distance(transform.position, opponentBasket.position);
            if (distanceToBasket < 2f)
            {
                Dunk();
            }
            else
            {
                Shoot();
            }
        }
        
        // Steal attempt
        if (Input.GetKeyDown(KeyCode.LeftControl) && !hasBall)
        {
            AttemptSteal();
        }
    }

    void AIBehavior()
    {
        // Update AI decision timer
        aiDecisionTimer -= Time.deltaTime;
        if (aiDecisionTimer <= 0f)
        {
            // Make a new decision
            DecideAIAction();
            aiDecisionTimer = Random.Range(0.5f, 2f);
        }
        
        // Apply movement
        rb.velocity = moveDirection * speed;
        
        // AI actions based on state
        if (hasBall)
        {
            // Determine if we're close enough to dunk or shoot
            float distanceToBasket = Vector2.Distance(transform.position, opponentBasket.position);
            
            // Random chance to shoot or dunk based on distance
            if (distanceToBasket < 2f && Random.value < 0.1f)
            {
                Dunk();
            }
            else if (distanceToBasket < 5f && Random.value < 0.05f)
            {
                Shoot();
            }
            // Random chance to pass to teammate
            else if (Random.value < 0.02f && teammates.Length > 0)
            {
                PassToTeammate();
            }
        }
        else
        {
            // If we don't have the ball, try to steal occasionally
            if (Random.value < 0.01f)
            {
                AttemptSteal();
            }
        }
    }
    
    void DecideAIAction()
    {
        if (hasBall)
        {
            // Move toward opponent's basket
            moveDirection = (opponentBasket.position - transform.position).normalized;
        }
        else
        {
            // Find the ball or defend
            if (ball != null)
            {
                // 70% chance to go for the ball, 30% to defend
                if (Random.value < 0.7f)
                {
                    moveDirection = (ball.position - transform.position).normalized;
                }
                else
                {
                    // Defend by moving between our basket and the opponent with the ball
                    Transform ballHandler = FindBallHandler();
                    if (ballHandler != null)
                    {
                        Vector2 defendPosition = Vector2.Lerp(basket.position, ballHandler.position, 0.7f);
                        moveDirection = (defendPosition - (Vector2)transform.position).normalized;
                    }
                    else
                    {
                        // Default to moving toward the ball
                        moveDirection = (ball.position - transform.position).normalized;
                    }
                }
            }
        }
    }
    
    Transform FindBallHandler()
    {
        // Find which player has the ball
        foreach (Transform opponent in opponents)
        {
            PlayerController opponentController = opponent.GetComponent<PlayerController>();
            if (opponentController != null && opponentController.hasBall)
            {
                return opponent;
            }
        }
        
        foreach (Transform teammate in teammates)
        {
            PlayerController teammateController = teammate.GetComponent<PlayerController>();
            if (teammateController != null && teammateController.hasBall)
            {
                return teammate;
            }
        }
        
        return null;
    }

    void Shoot()
    {
        if (ball != null && hasBall)
        {
            // Calculate direction to basket
            Vector2 directionToBasket = (opponentBasket.position - transform.position).normalized;
            
            // Calculate force based on shooting stat and distance
            float distance = Vector2.Distance(transform.position, opponentBasket.position);
            float forceMagnitude = shootForce + (shootingStat * 0.5f);
            
            // Add some arc to the shot
            Vector2 shootForceVector = directionToBasket * forceMagnitude;
            shootForceVector.y += distance * 0.5f;
            
            // Apply force to the ball
            BallController ballController = ball.GetComponent<BallController>();
            if (ballController != null)
            {
                ballController.Shoot(transform.position, shootForceVector, gameObject);
                hasBall = false;
                
                // Play shooting animation
                if (animator != null)
                {
                    animator.SetTrigger("Shoot");
                }
                
                // Play sound effect
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
        }
    }
    
    void Dunk()
    {
        if (ball != null && hasBall)
        {
            // Start dunking animation
            isDunking = true;
            
            // Jump toward the basket
            Vector2 directionToBasket = (opponentBasket.position - transform.position).normalized;
            rb.velocity = directionToBasket * (speed * 1.5f);
            
            // Score after a short delay
            Invoke("CompleteDunk", 0.5f);
            
            // Play sound effect
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
    
    void CompleteDunk()
    {
        isDunking = false;
        
        // Score points
        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
        {
            gameController.ScorePoints(isPlayerOne ? "Player" : "Opponent", 2);
        }
        
        // Reset ball
        BallController ballController = ball.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.ResetBall();
            hasBall = false;
        }
    }
    
    void PassToTeammate()
    {
        if (ball != null && hasBall && teammates.Length > 0)
        {
            // Find closest teammate
            Transform closestTeammate = null;
            float closestDistance = float.MaxValue;
            
            foreach (Transform teammate in teammates)
            {
                float distance = Vector2.Distance(transform.position, teammate.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTeammate = teammate;
                }
            }
            
            if (closestTeammate != null)
            {
                // Calculate direction to teammate
                Vector2 directionToTeammate = (closestTeammate.position - transform.position).normalized;
                
                // Apply force to the ball
                BallController ballController = ball.GetComponent<BallController>();
                if (ballController != null)
                {
                    ballController.Pass(transform.position, directionToTeammate * shootForce, closestTeammate.gameObject);
                    hasBall = false;
                    
                    // Play passing animation
                    if (animator != null)
                    {
                        animator.SetTrigger("Pass");
                    }
                }
            }
        }
    }
    
    void AttemptSteal()
    {
        // Find nearby players with the ball
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);
        
        foreach (Collider2D collider in colliders)
        {
            PlayerController otherPlayer = collider.GetComponent<PlayerController>();
            if (otherPlayer != null && otherPlayer.hasBall)
            {
                // Chance to steal based on relative speed stats
                float stealChance = 0.3f + (speedStat - otherPlayer.speedStat) * 0.05f;
                if (Random.value < stealChance)
                {
                    // Successful steal
                    otherPlayer.hasBall = false;
                    hasBall = true;
                    
                    // Update ball position
                    if (ball != null)
                    {
                        ball.position = transform.position + new Vector3(0.5f, 0.5f, 0);
                    }
                    
                    // Play steal animation
                    if (animator != null)
                    {
                        animator.SetTrigger("Steal");
                    }
                    
                    break;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we collided with the ball
        if (other.CompareTag("Ball") && !hasBall)
        {
            BallController ballController = other.GetComponent<BallController>();
            if (ballController != null && !ballController.IsHeld())
            {
                // Pick up the ball
                hasBall = true;
                ballController.PickUp(gameObject);
                
                // Play pickup animation
                if (animator != null)
                {
                    animator.SetTrigger("PickupBall");
                }
            }
        }
    }
    
    public bool HasBall()
    {
        return hasBall;
    }
    
    public void SetHasBall(bool value)
    {
        hasBall = value;
    }
}
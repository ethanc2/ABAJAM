using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    public float bounceForce = 5f;
    public float maxSpeed = 15f;
    public float dragInAir = 0.5f;
    public float dragOnGround = 3f;
    
    [Header("Visual Effects")]
    public GameObject trailEffect;
    public GameObject scoreEffect;
    
    // Private variables
    private Rigidbody2D rb;
    private CircleCollider2D ballCollider;
    private SpriteRenderer spriteRenderer;
    private GameObject currentHolder;
    private bool isHeld = false;
    private bool isShot = false;
    private bool isPassed = false;
    private GameObject targetPlayer;
    private Vector3 lastPosition;
    private float spinAmount = 0f;
    
    // Ball colors (ABA red, white, and blue)
    private Color redColor = new Color(0.9f, 0.2f, 0.2f);
    private Color whiteColor = Color.white;
    private Color blueColor = new Color(0.2f, 0.4f, 0.9f);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;
        
        // Set initial ball appearance
        SetupBallAppearance();
        
        // Disable trail effect initially
        if (trailEffect != null)
        {
            trailEffect.SetActive(false);
        }
    }
    
    void SetupBallAppearance()
    {
        // For a proper ABA ball, we would use a custom sprite
        // But for now, we'll just set the color to red (ABA ball was red, white, and blue)
        spriteRenderer.color = redColor;
    }

    void Update()
    {
        // Handle ball following the player if held
        if (isHeld && currentHolder != null)
        {
            // Position the ball slightly offset from the player
            Vector3 offset = new Vector3(0.5f, 0.3f, 0);
            
            // If the player is facing left, flip the offset
            PlayerController playerController = currentHolder.GetComponent<PlayerController>();
            if (playerController != null)
            {
                SpriteRenderer playerSprite = currentHolder.GetComponent<SpriteRenderer>();
                if (playerSprite != null && playerSprite.flipX)
                {
                    offset.x = -offset.x;
                }
            }
            
            transform.position = currentHolder.transform.position + offset;
            
            // Disable physics while held
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        else
        {
            // Enable physics when not held
            rb.isKinematic = false;
            
            // Apply drag based on whether the ball is on the ground
            rb.drag = IsOnGround() ? dragOnGround : dragInAir;
            
            // Limit max speed
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
            
            // Handle ball spin visual effect
            if (rb.velocity.magnitude > 1f)
            {
                // Spin the ball based on velocity
                spinAmount += rb.velocity.magnitude * Time.deltaTime * 50f;
                transform.rotation = Quaternion.Euler(0, 0, spinAmount);
                
                // Change ball color based on spin for ABA style
                float colorCycle = (Mathf.Sin(Time.time * 5f) + 1f) / 2f;
                if (colorCycle < 0.33f)
                {
                    spriteRenderer.color = Color.Lerp(redColor, whiteColor, colorCycle * 3f);
                }
                else if (colorCycle < 0.66f)
                {
                    spriteRenderer.color = Color.Lerp(whiteColor, blueColor, (colorCycle - 0.33f) * 3f);
                }
                else
                {
                    spriteRenderer.color = Color.Lerp(blueColor, redColor, (colorCycle - 0.66f) * 3f);
                }
            }
            else
            {
                // Reset rotation when nearly stopped
                transform.rotation = Quaternion.identity;
                spriteRenderer.color = redColor;
            }
        }
        
        // Handle trail effect
        if (trailEffect != null)
        {
            trailEffect.SetActive(isShot && rb.velocity.magnitude > 5f);
        }
        
        // Check if the ball is moving toward the target player (for passes)
        if (isPassed && targetPlayer != null)
        {
            Vector2 directionToTarget = (targetPlayer.transform.position - transform.position).normalized;
            Vector2 ballDirection = rb.velocity.normalized;
            
            // If the ball is moving away from the target, it missed
            float dotProduct = Vector2.Dot(directionToTarget, ballDirection);
            if (dotProduct < 0.5f)
            {
                isPassed = false;
                targetPlayer = null;
            }
            
            // If the ball is very close to the target, let them catch it
            float distanceToTarget = Vector2.Distance(transform.position, targetPlayer.transform.position);
            if (distanceToTarget < 1f)
            {
                // Auto-catch for the target player
                PlayerController targetController = targetPlayer.GetComponent<PlayerController>();
                if (targetController != null)
                {
                    targetController.SetHasBall(true);
                    isHeld = true;
                    isPassed = false;
                    currentHolder = targetPlayer;
                    rb.velocity = Vector2.zero;
                }
            }
        }
        
        // Store position for velocity calculations
        lastPosition = transform.position;
    }
    
    bool IsOnGround()
    {
        // Simple ground check using raycasting
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));
        return hit.collider != null;
    }

    public void Shoot(Vector2 shooterPosition, Vector2 force, GameObject shooter)
    {
        // Position the ball slightly above the shooter
        transform.position = shooterPosition + new Vector2(0, 0.5f);
        
        // Apply force to the ball
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        
        // Set state
        isHeld = false;
        isShot = true;
        isPassed = false;
        currentHolder = null;
        
        // Enable trail effect
        if (trailEffect != null)
        {
            trailEffect.SetActive(true);
        }
        
        // Start checking for basket collision
        StartCoroutine(CheckForBasket());
    }
    
    public void Pass(Vector2 passerPosition, Vector2 force, GameObject target)
    {
        // Position the ball slightly above the passer
        transform.position = passerPosition + new Vector2(0, 0.3f);
        
        // Apply force to the ball
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        
        // Set state
        isHeld = false;
        isShot = false;
        isPassed = true;
        targetPlayer = target;
        currentHolder = null;
    }
    
    public void PickUp(GameObject player)
    {
        isHeld = true;
        isShot = false;
        isPassed = false;
        currentHolder = player;
        targetPlayer = null;
        
        // Disable trail effect
        if (trailEffect != null)
        {
            trailEffect.SetActive(false);
        }
    }
    
    public void ResetBall()
    {
        // Reset ball to center court
        transform.position = new Vector3(0, 1, 0);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;
        
        // Reset state
        isHeld = false;
        isShot = false;
        isPassed = false;
        currentHolder = null;
        targetPlayer = null;
        
        // Reset appearance
        spriteRenderer.color = redColor;
        
        // Disable trail effect
        if (trailEffect != null)
        {
            trailEffect.SetActive(false);
        }
    }
    
    IEnumerator CheckForBasket()
    {
        // Wait a short time before checking for basket (to avoid immediate scoring)
        yield return new WaitForSeconds(0.1f);
        
        bool scored = false;
        
        while (isShot && !scored)
        {
            // Check if the ball is passing through either basket
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Basket"))
                {
                    // Determine which basket and award points
                    bool isPlayerOneBasket = collider.transform.position.x < 0;
                    string scoringTeam = isPlayerOneBasket ? "Opponent" : "Player";
                    
                    // Calculate points based on distance
                    int points = 2;
                    float shotDistance = Mathf.Abs(lastPosition.x - collider.transform.position.x);
                    if (shotDistance > 6f) // 3-point line distance
                    {
                        points = 3;
                    }
                    
                    // Award points
                    GameController gameController = FindObjectOfType<GameController>();
                    if (gameController != null)
                    {
                        gameController.ScorePoints(scoringTeam, points);
                    }
                    
                    // Show score effect
                    if (scoreEffect != null)
                    {
                        Instantiate(scoreEffect, transform.position, Quaternion.identity);
                    }
                    
                    // Reset the ball
                    ResetBall();
                    scored = true;
                    break;
                }
            }
            
            // Wait a frame before checking again
            yield return null;
            
            // If the ball has nearly stopped, end the shot
            if (rb.velocity.magnitude < 1f)
            {
                isShot = false;
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle bounce sound and effects
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            // Play bounce sound
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
            
            // Add a little bounce force
            if (rb.velocity.y < 0.1f)
            {
                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
            }
        }
    }
    
    public bool IsHeld()
    {
        return isHeld;
    }
    
    public GameObject GetCurrentHolder()
    {
        return currentHolder;
    }
}
using UnityEngine;

public class ShootingSpot : MonoBehaviour
{
    [Header("Spot Settings")]
    public bool isOccupied = false;
    public bool isMoneyBall = false;
    public GameObject nextSpot;
    public float spotRadius = 1f;
    
    [Header("Visual Elements")]
    public SpriteRenderer spotMarker;
    public Color normalColor = Color.white;
    public Color activeColor = Color.yellow;
    public Color moneyBallColor = Color.red;
    public GameObject highlightEffect;
    
    private void Start()
    {
        // Set up visual appearance
        if (spotMarker != null)
        {
            spotMarker.color = isMoneyBall ? moneyBallColor : normalColor;
        }
        
        // Disable highlight initially
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }
    }

    public void OccupySpot(GameObject player)
    {
        if (player != null)
        {
            // Move player to this spot
            player.transform.position = transform.position;
            
            // Update state
            isOccupied = true;
            
            // Update visuals
            if (spotMarker != null)
            {
                spotMarker.color = isMoneyBall ? moneyBallColor : activeColor;
            }
            
            // Show highlight effect
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(true);
            }
            
            // Face the basket (assuming basket is at positive X)
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
                if (playerSprite != null)
                {
                    // Ensure player is facing the basket
                    playerSprite.flipX = transform.position.x > 0;
                }
            }
        }
    }
    
    public void VacateSpot()
    {
        // Update state
        isOccupied = false;
        
        // Update visuals
        if (spotMarker != null)
        {
            spotMarker.color = isMoneyBall ? moneyBallColor : normalColor;
        }
        
        // Hide highlight effect
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }
    }
    
    public bool IsPlayerInSpot(GameObject player)
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            return distance < spotRadius;
        }
        return false;
    }
    
    public void SetMoneyBall(bool value)
    {
        isMoneyBall = value;
        
        // Update visuals
        if (spotMarker != null)
        {
            spotMarker.color = isMoneyBall ? moneyBallColor : (isOccupied ? activeColor : normalColor);
        }
    }
    
    private void OnDrawGizmos()
    {
        // Visual aid for spot radius in the editor
        Gizmos.color = isMoneyBall ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spotRadius);
    }
}
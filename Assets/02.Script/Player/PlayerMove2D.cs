


using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove2D : MonoBehaviour
{
    [Header("Ref")]
    private Rigidbody2D rb;
    private PlayerInputReader playerInputReader;

    [SerializeField]
    Direction currentDirection;
    public Direction CurrentDirection => currentDirection;

    Vector2 inputDirection;
    public float moveSpeed = 5.0f;


    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (playerInputReader == null) playerInputReader = GetComponent<PlayerInputReader>();
    }

    private void Start()
    {
        currentDirection = Direction.Down;
    }

    private void Update()
    {
        
        inputDirection = playerInputReader != null ? playerInputReader.MoveVector : Vector2.zero;
        UpdateDirection();
    }

    private void FixedUpdate() 
    {
        Vector2 newPosition = rb.position + inputDirection.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void UpdateDirection()
    {
        const float deadzoneValue = 0.1f;

        if (inputDirection.sqrMagnitude < 0.01f) return;

        float x = inputDirection.x;
        float y = inputDirection.y;

        if (x > deadzoneValue) 
        {
            if (y > deadzoneValue) currentDirection = Direction.RightUp;
            else if (y < -1 * deadzoneValue) currentDirection = Direction.RightDown;
            else currentDirection = Direction.Right;
        }
        else if (x < -1 * deadzoneValue) 
        {
          
            if (y > deadzoneValue) currentDirection = Direction.LeftUp;
           
            else if (y < -1 * deadzoneValue) currentDirection = Direction.LeftDown;
            else currentDirection = Direction.Left;
        }
        else 
        {
            if (y > deadzoneValue) currentDirection = Direction.Up;
            else if (y < -1 * deadzoneValue) currentDirection = Direction.Down;
        }

    }
}

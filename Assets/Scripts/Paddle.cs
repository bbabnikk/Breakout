using UnityEditor;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 startPos;
    [SerializeField] private int speed = 35;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
        Messenger.AddListener(GameEvent.PADDLE_WIDTH_CHANGED, OnPaddleWidthChanged);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.PADDLE_WIDTH_CHANGED, OnPaddleWidthChanged);
    }

    void OnPaddleWidthChanged()
    {
        transform.localScale = new Vector3(PlayerPrefs.GetFloat("paddleWidth", .75f), 1, 1);
    }
    void Start()
    {
        transform.localScale = new Vector3(PlayerPrefs.GetFloat("paddleWidth", .75f), 1, 1);
        startPos = transform.position;
    }

    public void Reset()
    {
        rb.position = startPos;
        rb.velocity = Vector2.zero;
    }

    void Update()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
    }

    private void FixedUpdate()
    {
        rb.AddForce(movement*speed);
    }
}

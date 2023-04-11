using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 startPos;
    private int speed;
    public int ballPower { get; private set; } = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Messenger.AddListener(GameEvent.BALL_SPEED_CHANGED, OnBallSpeedChanged);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.BALL_SPEED_CHANGED, OnBallSpeedChanged);
    }

    void OnBallSpeedChanged()
    {
        speed = (int)PlayerPrefs.GetFloat("ballSpeed");
    }
    // Start is called before the first frame update
    void Start()
    {
        speed = (int)PlayerPrefs.GetFloat("ballSpeed", 15);
        startPos = transform.position;
        Reset();

    }

    public void Reset()
    {
        rb.velocity = Vector2.zero;
        rb.position = startPos;
        Invoke(nameof(launchBall), 3);
    }

    private void launchBall()
    {
        //gives an initial direction speed will be fixed in FixedUpdate
        Vector2 movement = new Vector2(Random.Range(-1f, 1f), -1);
        rb.AddForce(movement);
    }

    private void FixedUpdate()
    {
        //give a nudge if we've hit a perfect 90% angle (hey it happened to me atleast once in testing.)
        if (rb.velocity != Vector2.zero)
        {
            if (rb.velocity.normalized.y > -0.05 && rb.velocity.normalized.y <= 0.0)
            {
                rb.velocity = rb.velocity.normalized + new Vector2(0, -0.01f);
            }
            else if (rb.velocity.normalized.y < 0.05 && rb.velocity.normalized.y == 0.0)
            {
                rb.velocity = rb.velocity.normalized + new Vector2(0, 0.01f);
            }
        }
        //used to maintain constant speed despite physics interations, this will also fix the speed from the previous procedure.
        rb.velocity = rb.velocity.normalized * speed;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //I really just wanted to use an extermely oval collider, but capsule collider wasn't configurable enough, circle collider had to STAY an exact circle for some god awful reason, and this was ultimately easier than making a poygon collider with a smooth transition.
        
        Paddle paddle = collision.gameObject.GetComponent<Paddle>();

        if (paddle != null)
        {
            float maxBounceAngle = 75;

            //figure out where on the paddle we hit
            float offset = paddle.transform.position.x - collision.GetContact(0).point.x;
            //get the width of the paddle (can'thard code this) because it could change with difficulty or powerups
            float halfPaddleWidth = collision.collider.bounds.size.x / 2;

            //current travel angle downwords as a signed float
            float currentAngle = Vector2.SignedAngle(Vector2.up, rb.velocity);
            //the angle we want to impart on the ball based on where it hit the paddle
            float bounceAngle = (offset / halfPaddleWidth) * maxBounceAngle;
            //combine those two angangles, and clamp it to our max angle range
            float newAngle = Mathf.Clamp(currentAngle + bounceAngle, -maxBounceAngle, maxBounceAngle);

            //apply the new travel direction to our ball
            Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
            rb.velocity = rotation * Vector2.up;

            //is angle even a real word.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Floor")
        {
            Messenger.Broadcast(GameEvent.BALL_LOST);
        }
    }
}

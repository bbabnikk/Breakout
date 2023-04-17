using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public int health { get; private set; }
    private SpriteRenderer sprite;
    [SerializeField]
    private Sprite[] states;
    [SerializeField]
    public bool unbreakable;
    public int value { get; private set; } = 0;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        Messenger.AddListener(GameEvent.EXPLODE_POWERUP_COLLECTED, OnExplodeCollected);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.EXPLODE_POWERUP_COLLECTED, OnExplodeCollected);
    }

    private void OnExplodeCollected()
    {
        Invoke(nameof(hit), Vector2.Distance(this.transform.position, new Vector2(5, 0))/10);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!unbreakable)
        {
            health = states.Length;
            sprite.sprite = states[health - 1];
            value = 20 * health;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void hit()
    {
        //quick fix for invoke notbeing able to call methods with parameters even if those parameters have a default value
        hit(1);
    }

    public void hit(int damage)
    {
        if (!unbreakable)
        {
            health -= damage;
            if (health <= 0)
            {
                Messenger<Brick>.Broadcast(GameEvent.BRICK_DESTROYED, this);
                Destroy(this.gameObject);
            }
            else
            {
                sprite.sprite = states[health - 1];
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball != null)
        {
            hit(ball.ballPower);
        }
    }
}

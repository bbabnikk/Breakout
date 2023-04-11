using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private int health;
    private SpriteRenderer sprite;
    [SerializeField]
    private Sprite[] states;
    [SerializeField]
    public bool unbreakable;
    public int value { get; private set; } = 0;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball != null)
        {
            if (!unbreakable) {
                health -= ball.ballPower;
                if (health <= 0)
                {
                    gameObject.SetActive(false);
                    Messenger<Brick>.Broadcast(GameEvent.BRICK_DESTROYED, this);
                } else
                {
                    sprite.sprite = states[health - 1];
                }
            }
        }
    }
}

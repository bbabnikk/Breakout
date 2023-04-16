using UnityEngine;
using static PowerUp;

public class PowerUp : MonoBehaviour
{
    private Animator anim;
    private float speed = 5;
    public PowerUpName powerName { get; private set; }
    // Start is called before the first frame update

    //define an new power ups here, link to animator by adding a boolean of the same name to the animator, add an action trigger to the case statement on the ontrigger Enter
    public enum PowerUpName {
        blockade,
        explode,
        life,
        multiball,
        powerIncrease,
        wide,
        Length
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();

    }
    void Start()
    {
        powerName = (PowerUpName)Random.Range(0, (int)PowerUpName.Length);
        anim.SetBool(powerName.ToString(), true);
        Debug.Log(""+ this + powerName);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Paddle")
        {
            Destroy(this.gameObject);
            switch (powerName)
            {
                case PowerUpName.blockade:
                    Messenger.Broadcast(GameEvent.BLOCKADE_POWERUP_COLLECTED);
                    break;
                case PowerUpName.explode:
                    Messenger.Broadcast(GameEvent.EXPLODE_POWERUP_COLLECTED);
                    break;
                case PowerUpName.life:
                    Messenger.Broadcast(GameEvent.LIFE_POWERUP_COLLECTED);
                    break;
                case PowerUpName.multiball:
                    Messenger.Broadcast(GameEvent.MULTIBALL_POWERUP_COLLECTED);
                    break;
                case PowerUpName.powerIncrease:
                    Messenger.Broadcast(GameEvent.POWERINCREASE_POWERUP_COLLECTED);
                    break;
                case PowerUpName.wide:
                    Messenger.Broadcast(GameEvent.WIDE_POWERUP_COLLECTED);
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Floor") { Destroy(this.gameObject); }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private UIManager ui;
    private Ball ball;
    private Paddle paddle;
    private Brick[] bricks;
    private AudioSource levelCompleteSound;
    private int score;
    private int lives;
    private int level;
    private float timeSinceLastBrickBroke;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        ui = FindObjectOfType<UIManager>();
        levelCompleteSound = gameObject.AddComponent<AudioSource>();
        SceneManager.sceneLoaded += OnLevelLoaded;
        Messenger<Brick>.AddListener(GameEvent.BRICK_DESTROYED, OnBrickDestroyed);
        Messenger.AddListener(GameEvent.BALL_LOST, OnBallLost);
        Messenger.AddListener(GameEvent.NEW_GAME, OnNewGameClicked);
        Messenger.AddListener(GameEvent.OPTIONS_OPENED, OnOptionsOpened);
        Messenger.AddListener(GameEvent.OPTIONS_CLOSED, OnOptionsClosed);
        Messenger.AddListener(GameEvent.SCORES_CLEARED, OnScoresCleared);
        Messenger.AddListener(GameEvent.QUIT, OnQuit);
        Messenger.AddListener(GameEvent.LIFE_POWERUP_COLLECTED, OnLifePickupCollected);
    }

    private void OnDestroy()
    {
        Messenger<Brick>.RemoveListener(GameEvent.BRICK_DESTROYED, OnBrickDestroyed);
        Messenger.RemoveListener(GameEvent.BALL_LOST, OnBallLost);
        Messenger.RemoveListener(GameEvent.NEW_GAME, OnNewGameClicked);
        Messenger.RemoveListener(GameEvent.OPTIONS_OPENED, OnOptionsOpened);
        Messenger.RemoveListener(GameEvent.OPTIONS_CLOSED, OnOptionsClosed);
        Messenger.RemoveListener(GameEvent.SCORES_CLEARED, OnScoresCleared);
        Messenger.RemoveListener(GameEvent.QUIT, OnQuit);
        Messenger.RemoveListener(GameEvent.LIFE_POWERUP_COLLECTED, OnLifePickupCollected);
    }

    void OnLifePickupCollected()
    {
        ui.setLivesText(++lives);
    }
    void OnQuit()
    {
        if (score > PlayerPrefs.GetInt("highScore"))
        {
            PlayerPrefs.SetInt("highScore", score);
            ui.setHighScoreText(score);
        }
        Application.Quit();
    }
    void OnScoresCleared()
    {
        ui.setHighScoreText(0);
    }
    void OnOptionsOpened()
    {
        if (SceneManager.GetActiveScene().name != "Splash") 
        {
            Time.timeScale = 0;
        }
    }
    void OnOptionsClosed()
    {
        Time.timeScale = 1;
    }
    void OnNewGameClicked()
    {
        NewGame();
    }
    private void OnBallLost()
    {
        

        ui.setLivesText(--lives);
        if (lives > 0)
        {
            //the original ball that we have a reference to may have been one of the multiballs that was lost, so we get a new reference here.
            ball = FindObjectOfType<Ball>();
            ball.Reset();
            paddle.Reset();
        } else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (score > PlayerPrefs.GetInt("highScore"))
        {
            PlayerPrefs.SetInt("highScore", score);
            ui.setHighScoreText(score);
        }
        SceneManager.LoadScene("Splash");
    }

    private void OnBrickDestroyed(Brick brick)
    {
        int breakValue = brick.value - (int)(timeSinceLastBrickBroke * 2);
        score += breakValue < 0 ? 0 : breakValue;
        timeSinceLastBrickBroke = 0;
        if (winConMet())
        {
            levelCompleteSound.Play();
            score += level * 500;
            if (level < 2)
            {
                LoadLevel(level + 1);
            }
            else
            {
                GameOver();
            }
        }
        ui.setScoreText(score);
    }

    private bool winConMet()
    {
        foreach (Brick b in bricks) { 
            if (b.gameObject.activeInHierarchy && !b.unbreakable) 
                return false;
        }
        return true;
    }

    private void Start()
    {
        ui.setHighScoreText(PlayerPrefs.GetInt("highScore"));
        ui.setScoreText(score);
        ui.setLivesText(lives);
        SceneManager.LoadScene("Splash");
    }

    private void NewGame()
    {
        if (score > PlayerPrefs.GetInt("highScore"))
        {
            PlayerPrefs.SetInt("highScore", score);
            ui.setHighScoreText(score);
        }
        score = 0;
        ui.setScoreText(score);
        lives = 3;
        ui.setLivesText(lives);
        level = 1;
        LoadLevel(level);
        timeSinceLastBrickBroke = -3f;
    }

    private void LoadLevel(int level)
    {
        this.level = level;
        SceneManager.LoadScene("Level" + level);
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        {
            paddle = FindObjectOfType<Paddle>();
            bricks = FindObjectsOfType<Brick>();
            CancelInvoke(nameof(enableBrickColliders));
            foreach (Brick b in bricks)
            {
                Collider2D collider = b.GetComponent<Collider2D>();

                collider.enabled = false;
                Vector2 targetPos = b.transform.position;
                Vector2 randomPos = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
                b.transform.position = randomPos;
                iTween.MoveTo(b.gameObject, targetPos, Random.Range(0.5f, 2f));
            }
            Invoke(nameof(enableBrickColliders), 3);
        }
    }
    private void enableBrickColliders()
    {
        foreach(Brick b in bricks)
        {
            Collider2D collider = b.GetComponent<Collider2D>();
            collider.enabled = true;
        }
    }

    private void Update()
    {
        timeSinceLastBrickBroke += Time.deltaTime;
    }
}

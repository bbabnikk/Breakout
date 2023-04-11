using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private UIManager ui;
    private Ball ball;
    private Paddle paddle;
    private Brick[] bricks;
    private int score;
    private int lives;
    private int level;
    private float timeSinceLastBrickBroke;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        ui = FindObjectOfType<UIManager>();
        SceneManager.sceneLoaded += OnLevelLoaded;
        Messenger<Brick>.AddListener(GameEvent.BRICK_DESTROYED, OnBrickDestroyed);
        Messenger.AddListener(GameEvent.BALL_LOST, OnBallLost);
        Messenger.AddListener(GameEvent.NEW_GAME, OnNewGameClicked);
        Messenger.AddListener(GameEvent.OPTIONS_OPENED, OnOptionsOpened);
        Messenger.AddListener(GameEvent.OPTIONS_CLOSED, OnOptionsClosed);
        Messenger.AddListener(GameEvent.SCORES_CLEARED, OnScoresCleared);
        Messenger.AddListener(GameEvent.QUIT, OnQuit);
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
        
    }

    private void OnBrickDestroyed(Brick brick)
    {
        int breakValue = brick.value - (int)(timeSinceLastBrickBroke * 2);
        score += breakValue < 0 ? 0 : breakValue;
        timeSinceLastBrickBroke = 0;
        if (winConMet())
        {
            score += level * 500;
            try
            {
                LoadLevel(level + 1);
            } 
            catch
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
            ball = FindObjectOfType<Ball>();
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

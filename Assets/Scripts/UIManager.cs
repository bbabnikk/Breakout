using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI lives;
    [SerializeField] private GameObject mainSideBar;
    [SerializeField] private GameObject optionsSideBar;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider paddleWidthSlider;
    private AudioSource clickSound;
    // Start is called before the first frame update
    private void Start()
    {
        clickSound = GetComponent<AudioSource>();
        speedSlider.value = PlayerPrefs.GetFloat("ballSpeed", 15);
        paddleWidthSlider.value = PlayerPrefs.GetFloat("paddleWidth", .75f);
    }
    public void setLivesText(int lives)
    {
        this.lives.text = "Lives:\n" + lives;
    }
    public void setScoreText(int score)
    {
        this.score.text = "Score:\n" + score;
    }
    public void setHighScoreText(int highScore)
    {
        this.highScore.text = "High Score:\n" + highScore;
    }

    public void onNewGameClicked()
    {
        clickSound.Play();
        Messenger.Broadcast(GameEvent.NEW_GAME);
    }

    public void onOptionsClicked()
    {
        clickSound.Play();
        mainSideBar.SetActive(false);
        optionsSideBar.SetActive(true);
        Messenger.Broadcast(GameEvent.OPTIONS_OPENED);
    }
    public void OnBallSpeedChanged()
    {
        clickSound.Play();
        PlayerPrefs.SetFloat("ballSpeed", speedSlider.value);
        Messenger.Broadcast(GameEvent.BALL_SPEED_CHANGED);
    }
    public void OnPaddleSizeChanged()
    {
        clickSound.Play();
        PlayerPrefs.SetFloat("paddleWidth", paddleWidthSlider.value);
        Messenger.Broadcast(GameEvent.PADDLE_WIDTH_CHANGED);
    }

    public void OnClearHighScoresClicked()
    {
        clickSound.Play();
        PlayerPrefs.SetInt("highScore", 0);
        Messenger.Broadcast(GameEvent.SCORES_CLEARED);
    }

    public void OnReturnClicked() 
    {
        clickSound.Play();
        mainSideBar.SetActive(true);
        optionsSideBar.SetActive(false);
        Messenger.Broadcast(GameEvent.OPTIONS_CLOSED);
    }

    public void OnQuitClicked()
    {
        clickSound.Play();
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour, IDataPersistence
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public Text gameOverText;
    public Text scoreText;
    public Text livesText;
    public Text highestScoreText;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip pacmaneatenClip;
    [SerializeField]
    private AudioClip gameoverClip;
    [SerializeField]
    private AudioClip gamewonClip;

    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int lives { get; private set; }
    public int highestScore { get; private set; } = 0;

    private void Start()
    {
        //NewGame();
        this.pacman.transform.position = this.pacman.position;
        this.pacman.movement.SetDirection(this.pacman.direction);
    }

    private void Update()
    {
        if (this.lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        gameOverText.enabled = false;
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        ResetGhostMultiplier();

        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
        }

        this.pacman.ResetState();
    }

    private void GameOver()
    {
        gameOverText.enabled = true;
        this.audioSource.PlayOneShot(gameoverClip);
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(2, '0');
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "x" + lives.ToString();
    }

    public void GhostEaten(Ghost ghost)
    {
        SetScore(this.score + (ghost.points * this.ghostMultiplier));
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        this.pacman.DeathSequence();
        //this.pacman.gameObject.SetActive(false);
        SetLives(this.lives - 1);
        if (this.lives > 0)
        {
            Invoke(nameof(ResetState), 2.0f);
            this.audioSource.PlayOneShot(pacmaneatenClip);
        }
        else 
        {
            GameOver();
            if (this.score > this.highestScore)
            {
                this.highestScore = this.score;
                highestScoreText.text = "highest: " + highestScore.ToString().PadLeft(2, '0');
            }
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        pellet.collected = true;
        SetScore(this.score + pellet.point);

        if (!HasPelletsLeft())
        {
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3.0f);
            this.audioSource.PlayOneShot(gamewonClip);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i=0; i<this.ghosts.Length; i++)
        {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasPelletsLeft() //named HasRemainingPellets in video
    {
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }

    public void LoadData(GameData data)
    {
        this.score = data.score;
        this.highestScore = data.highestScore;
        this.lives = data.lives;
        this.pacman.position = data.playerPosition;
        this.pacman.direction = data.pacmanDirection;
        this.LoadDisplay();
    }

    public void SaveData(GameData data)
    {
        data.score = this.score;
        data.highestScore = this.highestScore;
        data.lives = this.lives;
        data.playerPosition = this.pacman.transform.position;
        data.pacmanDirection = this.pacman.movement.direction;
    }

    public void LoadDisplay()
    {
        scoreText.text = score.ToString().PadLeft(2, '0');
        highestScoreText.text = "highest: " + highestScore.ToString().PadLeft(2, '0');
        livesText.text = "x" + lives.ToString();
    }
}

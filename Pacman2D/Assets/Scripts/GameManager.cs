using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;

    public Pacman pacman;

    public Transform pellets;

    public Text gameOverText;
    public Text scoreText;
    public Text livesText;

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

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (this.lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }

    private void NewGame()
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
            this.ghosts[i].ResetState();
        }

        this.pacman.ResetState();
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
        pacman.DeathSequence();
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
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);

        SetScore(this.score + pellet.points);

        if (!HasPelletsLeft())
        {
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 2.0f);
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
}

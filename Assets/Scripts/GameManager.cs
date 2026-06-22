using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int maxLives = 5;
    private int currentLives;

    public Transform checkpoint;
    public GameObject player;

    [Header("UI References")]
    public TextMeshProUGUI livesText;
    public Image[] heartImages;
    public Color fullHeart = Color.white;
    public Color emptyHeart = Color.gray;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    private AudioSource bgmSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentLives = maxLives;
    }

    void Start()
    {
        UpdateUI();

        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            UpdateHearts(playerHealth.GetMaxHealth());
        }

        bgmSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        PlayMusicBasedOnScene(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicBasedOnScene(scene);
    }

    private void PlayMusicBasedOnScene(Scene scene)
    {
        if (bgmSource == null) return;

        if (scene.name == "MainMenu")
        {
            if (bgmSource.isPlaying) bgmSource.Stop();
        }
        else
        {
            if (!bgmSource.isPlaying)
            {
                bgmSource.Play();
            }
        }
    }

    public void SetMusicVolume(float sliderValue)
    {
        float dBValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;

        audioMixer.SetFloat("MusicVol", dBValue);
    }

    public void PlayerDied()
    {
        currentLives--;
        UpdateUI();

        if (currentLives > 0)
        {
            RespawnPlayer();
        }
        else
        {
            Debug.Log("Game Over!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void RespawnPlayer()
    {
        player.SetActive(true);
        player.transform.position = checkpoint.position;

        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Collider2D col = player.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        player.GetComponent<Health>().ResetHealth();
    }

    public void UpdateUI()
    {
        if (livesText != null)
        {
            livesText.text = "x " + currentLives.ToString();
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                heartImages[i].color = fullHeart;
            }
            else
            {
                heartImages[i].color = emptyHeart;
            }
        }
    }

    public bool TryAddLife()
    {
        if (currentLives >= maxLives)
        {
            return false;
        }

        currentLives++;
        UpdateUI();
        return true;
    }
}
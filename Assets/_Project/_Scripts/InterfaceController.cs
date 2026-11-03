using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zombie.DataEvent;

public class InterfaceController : MonoBehaviour
{
    [Header("To characters")]
    [SerializeField] private BossSpawner bossSpawner;
    [SerializeField] private PlayerController playerController;

    [Header("Panels")] 
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    
    [Header("Text's HUD")]
    [SerializeField] private TextMeshProUGUI currentKilledZombies;
    [SerializeField] private TextMeshProUGUI survivedTime;
    [SerializeField] private TextMeshProUGUI highestSurvivedTime;
    [SerializeField] private TextMeshProUGUI textDeadZombies;
    [SerializeField] private TextMeshProUGUI spawnBossWarning;
    [SerializeField] private TextMeshProUGUI highestDeadZombies;
    [SerializeField] private TextMeshProUGUI gameTime;
    
    [Header("Others")]
    [SerializeField] private GameObject sliderGameObject;
    [SerializeField] private Slider bossHealthSlider;

    
    private int _deadZombies;
    private int _highestDeadZombies;
    private AudioSource scriptAudioSource;
    private BossController _bossController;
    private float _highestSavedTimer;
    void Start()
    {
        scriptAudioSource = FindAnyObjectByType<AudioSource>();
        Time.timeScale = 1;
        _highestSavedTimer = PlayerPrefs.GetFloat("highestScore");
        _highestDeadZombies = PlayerPrefs.GetInt("highestDeadZombies");
        
        bossSpawner.OnBossSpawn += OnBossSpawn;
        EnemyController.OnZombieKill += OnZombieKill;
    }

    private void OnDestroy()
    {
        EnemyController.OnZombieKill -= OnZombieKill;
    }

    private void OnBossDied(object sender, EventArgs e)
    {
        sliderGameObject.SetActive(false);
    }

    private void OnBossTookDamage(object sender, EventArgs e)
    {
        UpdateHealthBossBar();
    }

    private void OnZombieKill(object sender, EventArgs e)
    {
        UpdateDeadZombies();
    }

    private void OnBossSpawn(object sender, EventArgs e)
    {
        // O Boss é instanciado em runtime, então só dá para assinar os eventos dele agora.
        _bossController = GameObject.FindWithTag("Boss").GetComponent<BossController>();
        _bossController.OnBossTookDamage += OnBossTookDamage;
        _bossController.OnBossDied += OnBossDied;

        ShowBossSpawnText();
        sliderGameObject.SetActive(true);
    }

    private void Update()
    {
        Timer();
        textDeadZombies.text = $"Kills: {_deadZombies}";;

        if (_deadZombies > _highestDeadZombies)
        {
            _highestDeadZombies = _deadZombies;
            PlayerPrefs.SetInt("highestDeadZombies", _highestDeadZombies);
        }
        Pause();
    }

    private void UpdateHealthBossBar()
    {
        bossHealthSlider.value = _bossController.bossHealth;
    }

    private void UpdateDeadZombies()
    {
        _deadZombies++;
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;

        var min = (int) Time.timeSinceLevelLoad / 60;
        var seconds = (int)Time.timeSinceLevelLoad % 60;

        survivedTime.text = $"{min}m{seconds}s";
        currentKilledZombies.text = $"{_deadZombies}";
        highestDeadZombies.text = $"{_highestDeadZombies}";
        
        Score(min, seconds);
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    void Score(int min, int seconds)
    {
        if (highestSurvivedTime.text == "") highestSurvivedTime.text = $"{min}m{seconds}s";

        if (!(Time.timeSinceLevelLoad > _highestSavedTimer)) return;
        
        _highestSavedTimer = Time.timeSinceLevelLoad;
        highestSurvivedTime.text = $"{min}m{seconds}s";
        PlayerPrefs.SetFloat("highestScore", _highestSavedTimer);
    }

    private void Timer()
    {
        var min = (int)Time.timeSinceLevelLoad / 60;
        var seconds = (int)Time.timeSinceLevelLoad % 60;

        gameTime.text = $"{min} : {seconds}";
    }

    private void ShowBossSpawnText()
    {
        StartCoroutine(TextFading(1, spawnBossWarning));
    }

    IEnumerator TextFading(float timeToFade, TextMeshProUGUI textToFade)
    {
        textToFade.gameObject.SetActive(true);
        Color textColor = textToFade.color;
        textColor.a = 1;
        textToFade.color = textColor;
        yield return new WaitForSeconds(3);
        float counter = 0;
        while (textToFade.color.a > 0)
        {

            counter += Time.deltaTime / timeToFade;
            textColor.a = Mathf.Lerp(1, 0, counter);
            textToFade.color = textColor;
            if (textToFade.color.a <= 0)
            {
                textToFade.gameObject.SetActive(false);
            }
            yield return null;
        }
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        scriptAudioSource.UnPause();
    }

    private void Pause()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.pKey.wasPressedThisFrame || keyboard.escapeKey.wasPressedThisFrame)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            scriptAudioSource.Pause();
        }
    }

    public void Quit()
    {
        StartCoroutine(QuitTheGame());
    }

    IEnumerator QuitTheGame()
    {
        yield return new WaitForSecondsRealtime(0.02f);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

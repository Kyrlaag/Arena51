using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool isGamePaused = false;
    public GameObject pauseObjects;
    public GameObject panel;

    private bool isSettingsActive;
    public GameObject settingsObjects;

    private float volume;


    private void Awake()
    {
        Time.timeScale = 1f;

    }

    [SerializeField] private bool isMainMenu = false;
    private void Update()
    {
        if (isMainMenu == false)
        {
            if (Input.GetKeyDown(KeyCode.R))
                RestartLevel();

            if (Input.GetKeyDown(KeyCode.Escape))
                Back();
        }
        
    }


    public void PauseGame()
    {
        SetSliderVolume();

        if (isGamePaused == false)
        {
            Time.timeScale = 0f;
            pauseObjects.SetActive(true);
            panel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseObjects.SetActive(false);
            panel.SetActive(false);
        }
        isGamePaused = !isGamePaused;
    }


    public void ShowSettings()
    {
        pauseObjects.SetActive(false);
        settingsObjects.SetActive(true);
        isSettingsActive = true;
    }


    //ESC works to
    public void Back()
    {
        if (isSettingsActive == true)
        {
            pauseObjects.SetActive(true);
            settingsObjects.SetActive(false);
            isSettingsActive = false;
        }
        else
            PauseGame();


    }


    //R works to
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //can be use for main menu and next level
    public void LoadLevel(string LevelName)
    {
        try
        {
            SceneManager.LoadScene(LevelName);

        }
        catch 
        {

            Debug.Log(LevelName + " not found");
        }
        
    }

    public void VolumeChange(GameObject slider)
    {
        volume = slider.GetComponent<Slider>().value;

        if (slider.name == "Music") 
            FindObjectOfType<AudioManager>().VolumeUpdate(volume, true);
        else
            FindObjectOfType<AudioManager>().VolumeUpdate(volume, false);

    }
    //music dogru calismasi icin theme adli dosya olmasi zorunlu
    public GameObject musicSlider;
    public GameObject sfxSlider;
    public void SetSliderVolume()
    {
        musicSlider.GetComponent<Slider>().value = FindObjectOfType<AudioManager>().GetAudioVolume(true);
        sfxSlider.GetComponent<Slider>().value = FindObjectOfType<AudioManager>().GetAudioVolume(false);
    }


    public GameObject gameOverObjects;
    public GameObject nextLevelObjects;
    public void GameOver()
    {
        Time.timeScale = 0f;
        panel.SetActive(true);
        gameOverObjects.SetActive(true);
    }
    public void NextLevel()
    {
        Time.timeScale = 0f;
        panel.SetActive(true);
        nextLevelObjects.SetActive(true);
    }

    private bool isBattleStarted = false;
    public void StartBattle(GameObject button)
    {
        if (isBattleStarted == false)
        {
            isBattleStarted = true;
            button.SetActive(false);
            StartCoroutine(StartBattleEnum());

        }
    }

    IEnumerator StartBattleEnum()
    {     
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");

        foreach (var item in enemies)
        {
            item.GetComponent<Stats>().startCor();
        }
        foreach (var item in allies)
        {
            item.GetComponent<Stats>().startCor();
        }
        yield return new WaitForSeconds(2f);

        FindObjectOfType<BattleSystem>().StartBattle();
        FindObjectOfType<GridManager>().canInteract = false;
    }


    public void CheckForEnemies()
    {
        StartCoroutine(CheckingEnemy());
    }

    IEnumerator CheckingEnemy()
    {
        yield return new WaitForSeconds(0.1f);

        if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 0)
        {
            NextLevel();
        }

        if (GameObject.FindGameObjectsWithTag("Ally").Length <= 0)
        {
            GameOver();
        }

        yield return null;
    }

    [SerializeField] private Sprite buttonSelectedSprite;
    [SerializeField] private Sprite buttonNormalSprite;
    public void LevelButtonSelected(GameObject button)
    {
        button.GetComponent<Image>().sprite = buttonSelectedSprite;
        FindObjectOfType<AudioManager>().Play("ButtonSelect");
    }

    public void LevelButtonUnelected(GameObject button)
    {
        button.GetComponent<Image>().sprite = buttonNormalSprite;

    }
}

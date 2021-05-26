using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image m_colorChanger;
    [SerializeField] private TextMeshProUGUI m_timerCooldown;
    [SerializeField] private GameObject m_endScreen;
    [SerializeField] private GameObject m_startScreen;

    [SerializeField] private TextMeshProUGUI m_score;

    [SerializeField] private Button m_startButton;
    [SerializeField] private Button m_restartButton;
    [SerializeField] private Button m_quitButton;
    
    private LevelManager m_currentLevel;

    private float m_timer;

    private void Awake()
    {
        m_endScreen.SetActive(false);

        m_colorChanger.gameObject.SetActive(false);
        m_timerCooldown.gameObject.SetActive(false);

        m_restartButton.onClick.AddListener(restart);
        m_startButton.onClick.AddListener(startGame);
        m_quitButton.onClick.AddListener(exit);
    }

    private void Update()
    {
        if (m_timer >= 0)
        {
            m_timer -= Time.deltaTime;
            m_timerCooldown.text = Mathf.RoundToInt(m_timer).ToString();
        }
    }
    private void exit()
    {
        Application.Quit();
    }

    private void restart()
    {
        m_currentLevel.restartGame();
    }

    private void startGame()
    {
        m_startScreen.SetActive(false);
        m_currentLevel.startGame();
    }
    public void changeColorChanger(Color color, bool activation)
    {
        m_colorChanger.color = color;
        m_colorChanger.gameObject.SetActive(activation);

        m_timerCooldown.color = color;
        m_timerCooldown.gameObject.SetActive(activation);
    }

    public void gameEnded()
    {
        m_endScreen.SetActive(true);
    }

    public void setCurrentLevel(LevelManager level)
    {
        m_currentLevel = level;
    }

    public void setTimer(float timer)
    {
        m_timer = timer;
    }

    public void updateScore(int score)
    {
        m_score.text = score.ToString();
    }
}

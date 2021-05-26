using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager: MonoBehaviour
{

    private const float WARNING_TIMER = 5.0f;
    private const float SCORE_DIVIDER = 5.0f;

    private const float DETECTOR_VISIBILITY = 0.25f;

    [SerializeField] private CylinderControl m_mainScene;
    [SerializeField] private float m_movementSpeed;

    [SerializeField] private List<Fluid> m_fluid;

    [SerializeField] private Transform m_startDetectorLimit;
    [SerializeField] private Transform m_endDetectorLimit;

    [SerializeField] private Vector2 m_changeDirectionCooldown;
    [SerializeField] private bool m_changeDirections;

    [SerializeField] private Vector2 m_changeFluidCooldown;
    [SerializeField] private bool m_changeFluid;

    [SerializeField] private UIController m_uiController;
    [SerializeField] private Material m_detector;

    private bool m_startMoving;
    private List<int> m_fluidDirections;

    private AudioSource m_backgroundMusic;

    private int m_score;
    private int m_fluidChangeCount;

    private int m_currentActivatedFluid;

    private bool m_gameStarted;

    private void Awake()
    {
        Time.timeScale = 1.0f;

        m_startMoving = false;

        m_fluidDirections = new List<int>();
        for(int i = -1; i < 2; i++)
            m_fluidDirections.Add(i);

        m_detector.color = new Color(1.0f, 0, 0, DETECTOR_VISIBILITY);

        m_uiController.setCurrentLevel(this);

        m_fluid[0].activateDetection(true);

        m_gameStarted = false;

        m_backgroundMusic = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (m_gameStarted)
        {
            updateScore();

            move();

            if (!m_fluid[m_currentActivatedFluid].wasDetected())
                gameEnded();

        }
    }

    //give the illusion that the camera is moving
    private void move()
    {
        if (!m_startMoving && m_fluid[0].getFinalPosition().y <= m_startDetectorLimit.position.y)
            m_startMoving = true;

        if (m_startMoving)
            m_mainScene.transform.position += Time.deltaTime * m_movementSpeed * Vector3.up;
    }

    private void gameEnded()
    {
        Time.timeScale = 0.0f;
        m_uiController.gameEnded();

    }

    private void updateScore()
    {
        m_score = m_fluidChangeCount + Mathf.RoundToInt(m_mainScene.transform.position.y / SCORE_DIVIDER);
        m_uiController.updateScore(m_score);
    }

    //change the moving direction of all fluids
    private void changeDirection()
    {
        List<int> currentDirections = new List<int>();

        do
        {
            int index = Random.Range(0, m_fluidDirections.Count);
            currentDirections.Add(m_fluidDirections[index]);
            m_fluidDirections.RemoveAt(index);

        } while (m_fluidDirections.Count > 0);

        m_fluidDirections = currentDirections;
        for (int i = 0; i < m_fluidDirections.Count; i++)
            m_fluid[i].setDirection(m_fluidDirections[i]);

        float timerCooldown = Random.Range(m_changeDirectionCooldown.x, m_changeDirectionCooldown.y);
        Invoke("changeDirection", timerCooldown);
    }

    //change which fluid will being detected next
    private IEnumerator changeActivatedFluid()
    {

        float cooldown = Random.Range(m_changeFluidCooldown.x, m_changeFluidCooldown.y);

        yield return new WaitForSeconds(cooldown);

        int index = Random.Range(0, m_fluid.Count);

        while(index == m_currentActivatedFluid)
            index = Random.Range(0, m_fluid.Count);

        m_fluid[m_currentActivatedFluid].activateDetection(false);

        Color currentColor = m_fluid[index].getColor();
        m_uiController.setTimer(WARNING_TIMER);
        m_uiController.changeColorChanger(currentColor, true);

        m_detector.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);

        yield return new WaitForSeconds(WARNING_TIMER);

        currentColor.a = DETECTOR_VISIBILITY;
        m_detector.color = currentColor;

        m_uiController.changeColorChanger(m_fluid[index].getColor(), false);

        m_currentActivatedFluid = index;
        m_fluid[m_currentActivatedFluid].activateDetection(true);

        m_fluidChangeCount++;

        StartCoroutine(changeActivatedFluid());

    }

    public void restartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void startGame()
    {
        foreach (Fluid fluid in m_fluid)
            fluid.startGame();

        m_mainScene.startGame();

        if (m_changeDirections)
        {
            float timerCooldown = Random.Range(m_changeDirectionCooldown.x, m_changeDirectionCooldown.y);
            Invoke("changeDirection", timerCooldown);
        }

        if (m_changeFluid)
            StartCoroutine(changeActivatedFluid());

        m_backgroundMusic.Play();
        
        m_gameStarted = true;

    }

}

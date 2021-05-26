using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderControl : MonoBehaviour
{
    private const float MIN_DISTANCE_ROTATION = 50.0f;

    [SerializeField] private List<GameObject> m_cylinders;
    [SerializeField] private float m_rotationSpeed;

    private Camera m_mainCamera;
    private float m_length;

    private Vector2 m_oldMousePosition;

    private bool m_gameStarted;

    private void Awake()
    {
        m_mainCamera = Camera.main;
        m_gameStarted = false;

        m_length = m_cylinders[0].transform.localScale.y * 2.0f;
    }

    private void Update()
    {
        if (m_gameStarted)
        {
            updatePositions();

            updateRotation();
        }
    }

    //parallax effect
    private void updatePositions()
    {
        if (m_mainCamera.transform.position.y < m_cylinders[m_cylinders.Count - 1].transform.position.y + m_length)
        {
            Vector3 currentPosition = m_cylinders[0].transform.position;
            currentPosition.y = m_cylinders[m_cylinders.Count - 1].transform.position.y - m_length;
            m_cylinders[0].transform.position = currentPosition;

            GameObject currentCylinder = m_cylinders[0];
            m_cylinders.Remove(currentCylinder);
            m_cylinders.Add(currentCylinder);
        }
    }

    //rotate the cylinders
    private void updateRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_oldMousePosition = Input.mousePosition;
        }else if (Input.GetMouseButton(0)) {
            Vector2 currentPosition = Input.mousePosition;
            float x = currentPosition.x - m_oldMousePosition.x;
            if (Mathf.Abs(x) > MIN_DISTANCE_ROTATION)
                rotate(Mathf.Sign(x));
        }
    }

    private void rotate(float direction)
    {
        transform.Rotate(new Vector3(0, m_rotationSpeed * Time.deltaTime * -direction, 0));
    }

    public void startGame()
    {
        m_gameStarted = true;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fluid : MonoBehaviour
{
    private const int SEGMENTS = 20;
    private const int MAX_POSITIONS = 7000;

    private const float Z_LIMIT = -0.90f;
    private const float START_DIRECTION_TIMER = 1.0f;

    [SerializeField] private float m_xRadius;
    [SerializeField] private float m_zRadius;

    [SerializeField] private float m_rotationSpeed;
    [SerializeField] private float m_movementSpeed;

    [SerializeField] private Transform m_heightLimit;

    [SerializeField] private Transform m_startDetection;
    [SerializeField] private Transform m_endDetection;

    [SerializeField] private float m_startAngle;
    [SerializeField] private int m_startDirection;


    private LineRenderer m_line;

    private List<Vector3> m_linePositions;

    private float m_yPosition;
    private float m_angle;

    private int m_direction;
    private bool m_detectPosition;

    private bool m_gameStart;

    private void Awake()
    {
        m_line = GetComponent<LineRenderer>();
        m_line.enabled = false;

        m_line.useWorldSpace = false;
        m_line.positionCount = SEGMENTS + 1;

        m_linePositions = new List<Vector3>();

        m_direction = 0;

        m_angle = m_startAngle;

        m_gameStart = false;
        m_line.numCornerVertices = 6;
    }

    private void Update()
    {

        if (m_gameStart)
        {
            updatePosition();

            updatePositionsList();
        }
    }

    //set the next position of the fluid
    private void updatePosition()
    {
        if (m_linePositions.Count < MAX_POSITIONS)
        {
            if (m_direction != 0)
            {
                m_angle += Time.deltaTime * m_rotationSpeed * Mathf.Sign(m_direction);
                m_angle = m_angle % 360.0f;
            }


            m_yPosition -= Time.deltaTime * m_movementSpeed;

            float x = Mathf.Sin(Mathf.Deg2Rad * m_angle) * m_xRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * m_angle) * m_zRadius;

            Vector3 nextPosition = new Vector3(x, m_yPosition, z);

            m_linePositions.Add(nextPosition);

            m_line.positionCount = m_linePositions.Count;
            m_line.SetPositions(m_linePositions.ToArray());
        }
    }

    //verify if the fluid is rotated correctly when passing through the desired threshold
    private bool fluidDetected()
    {
        List<Vector3> insidePoints = m_linePositions.FindAll((x) => transform.TransformPoint(x).y < m_startDetection.position.y &&
        transform.TransformPoint(x).y > m_endDetection.position.y);

        if (insidePoints.Count > 0)
        {
            foreach(Vector3 position in insidePoints)
            {
                float zPosition = transform.TransformPoint(position).z;
                if (transform.TransformPoint(position).z > Z_LIMIT)
                    return false;
            }
        }

        return true;
    }


    private void updatePositionsList()
    {
        if (transform.TransformPoint(m_linePositions[0]).y > m_heightLimit.position.y)
            m_linePositions.RemoveAt(0);
    }

    private void changeDirection()
    {
        m_direction = m_startDirection;
    }

    public Vector3 getFinalPosition()
    {

        return (m_linePositions.Count > 0) ? transform.TransformPoint(m_linePositions[m_linePositions.Count - 1]) : Vector3.up;
    }

    public void setDirection(int direction)
    {
        m_direction = direction;
    }


    public void activateDetection(bool state)
    {
        m_detectPosition = state;
    }

    public bool wasDetected()
    {
        return (!m_detectPosition || fluidDetected());
    }

    public Color getColor()
    {
        return m_line.startColor;
    }

    public void startGame()
    {
        m_gameStart = true;

        m_line.enabled = true;
        Invoke("changeDirection", START_DIRECTION_TIMER);
    }

}

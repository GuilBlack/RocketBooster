using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacleController : MonoBehaviour
{
    [Header("Movement Params")]
    [Tooltip("Where you want the object to move to." +
        " It will take the z position from the starting position of the object.")]
    [SerializeField] Vector2 m_EndPosition;
    [Tooltip("Time it takes to move from the starting position to the ending position and back to starting positon in seconds." +
        "If it's equal to 0, it will not move.")]
    [SerializeField] [Range(0.0001f, 30)] float m_TimeToMove = 1f;
    Vector3 m_StartPosition;
    Vector3 m_RealEndPosition;
    float m_LerpIteration = 0f;
    float m_Tau = (float)Math.PI * 2f;

    [Header("Rotation Params")]
    [SerializeField] float m_RotationSpeed;
    [Tooltip("Either spins the object with positive or negative rotation speed.")]
    [SerializeField] bool m_RotationDirection;

    // Start is called before the first frame update
    void Start()
    {
        m_StartPosition = transform.position;
        m_RealEndPosition = new Vector3(m_EndPosition.x, m_EndPosition.y, m_StartPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (m_RotationSpeed != 0f)
            Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(new Vector3(0, 0, m_RotationSpeed * Time.deltaTime * (m_RotationDirection ? -1 : 1)), Space.World);
    }

    private void Move()
    {
        m_LerpIteration += Time.deltaTime / m_TimeToMove;

        float offset = Mathf.Sin(m_LerpIteration * m_Tau) * 0.5f + 0.5f;

        transform.position = Vector3.Lerp(m_StartPosition, m_RealEndPosition, offset);
    }
}

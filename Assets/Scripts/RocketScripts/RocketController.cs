using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class RocketController : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    [SerializeField] float m_Force = 1000.0f;
    [SerializeField] float m_RotationSpeed = 90.0f;

    [Header("Audio Clips")]
    [SerializeField] AudioClip m_EngineAudio;
    [SerializeField] AudioClip m_SuccessAudio;
    [SerializeField] AudioClip m_CrashAudio;

    [Header("Particles")]
    [SerializeField] ParticleSystem m_SuccessParticles;
    [SerializeField] ParticleSystem m_CrashParticles;
    [SerializeField] ParticleSystem m_MainThrusterParticles;
    [SerializeField] ParticleSystem m_SideThrusterParticlesRight;
    [SerializeField] ParticleSystem m_SideThrusterParticlesLeft;

    Rigidbody m_Rb;
    Vector2 m_Move;
    bool m_MovementEnabled = true;
    AudioSource m_AudioRocket;

    bool m_IsLoading = false;
    public static bool HasFinished { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        HasFinished = false;
        m_Rb = GetComponent<Rigidbody>();
        m_AudioRocket = GetComponent<AudioSource>();
        m_AudioRocket.volume = 0.5f;
        m_AudioRocket.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenuHandler.IsPaused)
            return;

        RocketThrust();
        RocketRotate();
    }

    /////////////////////////////////////////////////////////////////////////////////
    //////Movements Handler//////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    public void OnMove(InputValue val)
    {
        if (m_MovementEnabled)
            m_Move = val.Get<Vector2>();
    }

    void RocketThrust()
    {
        m_Rb?.AddRelativeForce(new Vector3(0, m_Move.y * m_Force * Time.deltaTime, 0));

        if (!m_MovementEnabled)
            return;

        if (m_Move.y != 0f)
        {
            if (!m_AudioRocket.isPlaying)
                m_AudioRocket.PlayOneShot(m_EngineAudio);

            StartThrusterParticles(m_MainThrusterParticles);
        }
        else
        {
            m_AudioRocket.Stop();
            m_MainThrusterParticles.Stop();
        }

    }

    void RocketRotate()
    {
        m_Rb.freezeRotation = true;
        transform.Rotate(0, 0, -m_Move.x * m_RotationSpeed * Time.deltaTime);
        m_Rb.freezeRotation = false;

        if (m_Move.x == 1f)
        {
            StartThrusterParticles(m_SideThrusterParticlesLeft);
        }
        else if (m_Move.x == -1f)
        {
            StartThrusterParticles(m_SideThrusterParticlesRight);
        }
        else
        {
            m_SideThrusterParticlesLeft.Stop();
            m_SideThrusterParticlesRight.Stop();
        }
    }

    void StartThrusterParticles(ParticleSystem thruster)
    {
        if (!thruster.isPlaying)
            thruster.Play();
    }


    /////////////////////////////////////////////////////////////////////////////////
    //////Collision Handler//////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    void OnCollisionEnter(Collision collision)
    {
        if (m_IsLoading)
            return;

        int activeScene = SceneManager.GetActiveScene().buildIndex;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                SuccessSequence(activeScene);
                break;
            default:
                CrashSequence(activeScene);
                break;
        }
    }

    #region Crash Section

    void CrashSequence(int activeScene)
    {
        m_IsLoading = true;

        //disabling collision + movements + hide object
        m_MovementEnabled = false;
        m_Move = new Vector2(0f, 0f);
        m_Rb.velocity = Vector3.zero;
        m_Rb.useGravity = false;
        m_Rb.freezeRotation = false;

        //audio crash
        m_AudioRocket.Stop();
        m_AudioRocket.PlayOneShot(m_CrashAudio);

        //particles
        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in mrs)
        {
            mr.enabled = false;
        }
        m_CrashParticles.Play();

        //basically waits for 1 sec before beginning new scene
        StartCoroutine("CrashCoroutine", activeScene);
    }

    IEnumerator CrashCoroutine(int activeScene)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(activeScene);
        yield return null;
    }

    #endregion

    #region Success Section

    void SuccessSequence(int activeScene)
    {
        HasFinished = true;
        m_IsLoading = true;

        //disabling collision + movements
        m_MovementEnabled = false;
        m_Move = new Vector2(0f, 0f);
        m_Rb.velocity = Vector3.zero;
        m_Rb.useGravity = false;
        m_Rb.freezeRotation = false;

        //success audio
        m_AudioRocket.Stop();
        m_AudioRocket.PlayOneShot(m_SuccessAudio);

        //particles
        m_SuccessParticles.Play();

        StartCoroutine("SuccessCoroutine", activeScene);
    }

    IEnumerator SuccessCoroutine(int activeScene)
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene((activeScene + 1) % SceneManager.sceneCountInBuildSettings);

        yield return null;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    TextMeshProUGUI m_TMProText;
    RectTransform m_RectTransform;

    bool m_HasFinished = false;
    double m_Time;
    double m_BestTime;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Time = 0;
        m_TMProText = GetComponent<TextMeshProUGUI>();
        m_RectTransform = GetComponent<RectTransform>();

        float width = Screen.width / 4f;
        float height = Screen.height / 4;
        m_RectTransform.sizeDelta = new Vector2(width, height);
        m_RectTransform.anchoredPosition = new Vector2(width / 2f, -height / 2f);
        m_TMProText.fontSize = m_RectTransform.sizeDelta.y / 2f;
        m_BestTime = DataManager.Instance.LoadLevel(SceneManager.GetActiveScene().buildIndex).time;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenuHandler.IsPaused || m_HasFinished)
            return;

        m_Time += Time.deltaTime;
        m_TMProText.text = ((int)(m_Time / 60d)) + ":" + ((int)m_Time % 60) + "\nBest: " +
            (m_BestTime == 0d ? "???" : ((int)(m_BestTime / 60d)) + ":" + ((int)m_BestTime % 60));

        if (RocketController.HasFinished)
        {
            m_HasFinished = true;
            double timeToSave = (m_BestTime == 0d || m_Time <= m_BestTime) ? m_Time : m_BestTime;
            DataManager.Instance.SaveLevel(new LevelData(timeToSave, true), SceneManager.GetActiveScene().buildIndex);
            return;
        }
    }
}

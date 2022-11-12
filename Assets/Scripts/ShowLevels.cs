using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShowLevels : MonoBehaviour
{
    [SerializeField] GameObject LevelContainer;
    // Start is called before the first frame update
    void Start()
    {
        PopulateContentGrid();
    }

    void PopulateContentGrid()
    {
        GameObject newObj;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings - 2; ++i)
        {
            newObj = Instantiate(LevelContainer, transform);
            
            TextMeshProUGUI TMProText = newObj.GetComponentInChildren<TextMeshProUGUI>();
            if (TMProText != null)
                TMProText.text = (i + 1).ToString();

            LevelContainerHandler handler = newObj.GetComponent<LevelContainerHandler>();
            if (handler != null)
                handler.level = i + 2;
        }
    }
}

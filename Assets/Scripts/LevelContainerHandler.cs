using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelContainerHandler : MonoBehaviour
{
    public int level;

    public void OnLevelContainerClick()
    {
        SceneManager.LoadScene(level);
    }
}

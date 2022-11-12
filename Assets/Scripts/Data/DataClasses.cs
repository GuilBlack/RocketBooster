using UnityEngine.SceneManagement;
using UnityEngine;

public class GameData
{
    public LevelData[] levelData;

    public GameData()
    {
        levelData = new LevelData[SceneManager.sceneCountInBuildSettings - 2];

        for (int i = 0; i < levelData.Length; i++)
        {
            levelData[i] = new LevelData();
        }
    }

    public override string ToString()
    {
        string r = "";
        r += "{\nlevelData: [\n";

        for (int i = 0; i < levelData.Length - 1; ++i)
        {
            r += "    " + levelData[i].ToString() + ",\n";
        }

        r += "    " + levelData[levelData.Length - 1].ToString() + "\n    ]" + "\n}";

        return r;
    }
}

public class LevelData
{
    public double time;
    bool finished;

    public LevelData()
    {
        time = 0f;
        finished = false;
    }

    public LevelData(double time, bool finished)
    {
        this.time = time;
        this.finished = finished;
    }

    public override string ToString()
    {
        return "{" + time + ", " + finished + "}";
    }
}

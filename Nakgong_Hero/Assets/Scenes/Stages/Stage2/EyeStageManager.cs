using Scenes.Stages;
using Source.PlayerCode;
using UnityEngine;

public class EyeStageManager : StageManager
{
    public void StopPlayer()
    {
        PlayerController.instance.Stop();
    }

    public void StartPlayer()
    {
        PlayerController.instance.DisStop();
    }
}

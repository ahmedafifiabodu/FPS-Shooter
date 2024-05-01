using UnityEngine;

public class StopTime : MonoBehaviour
{
    public void Stop() => Time.timeScale = 0;

    public void Resume() => Time.timeScale = 1;
}
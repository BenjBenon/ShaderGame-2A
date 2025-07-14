using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{
    public GameObject canvas;

    public void OnPauseExit()
    {
        canvas.SetActive(false);
        Time.timeScale = 1.0f;
    }
}

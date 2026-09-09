using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameHUB : MonoBehaviour
{
    [Tooltip("A reference to the score text in the ui for the scene .")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [Tooltip("A reference to the  time text in the ui for the scene .")]
    [SerializeField] private TextMeshProUGUI timeText;

    /// <summary>
    /// Toggles the visibility and locks state of the mouse cursor 
    /// </summary>
    public bool CursorEnabled
    {
        set
        {
            Cursor.visible = value;
            if (value == true)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        UpdateScoreDisplay(0);
    }

    /// <summary>
    /// update score 
    /// </summary>
    /// <param name="score"></param>
    public void UpdateScoreDisplay(int score)
    {
        scoreText.text = "score:" + score;

    }
    /// <summary>
    /// 
    /// Formats the passed time in stopwatch format format and updates the timer text 
    /// </summary>
    /// <param name="time"></param>

    public void UpdateTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.CeilToInt(time % 60);
        if (time> 0 && seconds %60 == 0)
        {
            seconds = 0;
            minutes++;
        }
        string timeString = string.Format("time left:{0:00}:{1:00}",minutes,seconds);
        timeText.text = timeString;

    }




}

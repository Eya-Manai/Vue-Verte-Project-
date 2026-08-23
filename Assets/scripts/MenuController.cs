using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    public void startBtn1()
    {
        SceneManager.LoadScene("SampleScene");

    }
    //scene 2
    public void startBtn2() 
    {
        SceneManager.LoadScene("SampleScene");

    } 
}

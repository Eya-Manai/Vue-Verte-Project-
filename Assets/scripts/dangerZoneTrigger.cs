using UnityEngine;

public class DangerZoneTrigger : MonoBehaviour
{
    private AudioSource dangerAudio;
    private bool ready = false;

    void Awake()
    {
        dangerAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        Invoke(nameof(EnableTrigger), 0.3f);
    }

    void EnableTrigger()
    {
        ready = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ready) return; 

        if (!dangerAudio.isPlaying)
        {
            dangerAudio.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (dangerAudio.isPlaying)
        {
            dangerAudio.Stop();
        }
    }
}
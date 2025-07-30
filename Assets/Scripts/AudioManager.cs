using System.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    // add sfx here

    [Header("Audio Clips")]
    public AudioClip background;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.volume = 0.1f;
        musicSource.Play();
        
        
    }
}

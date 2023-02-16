using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource explodeSound;

    public void PlayDestroySound()
    {
        explodeSound.Play(0);
    }


}

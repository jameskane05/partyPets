using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PetCtrl : MonoBehaviour
{

    public static AccountCtrl accountCtrlInstance;

    public Pet pet;
    [HideInInspector] public ParticleSystem heartParticles;
    [HideInInspector] public AudioSource petAudioSource;
    [HideInInspector] public bool didPath = true;
    [HideInInspector] public int audioClipIndex = 0;
    public AudioClip[] audioClips;

    // Use this for initialization
    void Awake() {
        pet = AccountCtrl.GetInstance().GetActivePet();
        heartParticles = gameObject.GetComponent<ParticleSystem>();
        petAudioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void increasePetHappiness()
    {

    }

    public void playPetNoise()
    {
        petAudioSource.clip = audioClips[audioClipIndex];
        petAudioSource.Play();
        audioClipIndex++;
    }
}
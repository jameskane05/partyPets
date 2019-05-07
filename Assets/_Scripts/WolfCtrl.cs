using UnityEngine;
using UnityEngine.UI;
using SpeechRecognition;
using System;
using System.Collections.Generic;
using UnityEngine.AI;

public class WolfCtrl : PetCtrl
{
    private Text inputText;
    private string[] words;
    private Transform playerCameraPosition;
    private Animator petAnimator;
    private NavMeshAgent petNavMeshAgent;

    void Start()
    {
        SpeechToText.instance.onResultCallback = OnResultSpeech;
        Setting("en-US");
        inputText = GameObject.Find("Speech-to-Text Input").GetComponent<Text>();
        petAnimator = gameObject.GetComponent<Animator>();
        petNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        petNavMeshAgent.updatePosition = false;
    }

    void Update()
    {
        bool isWalking = petNavMeshAgent.velocity.magnitude > .25f;

        Debug.Log(petNavMeshAgent.velocity.magnitude);
        Debug.Log(isWalking);
        Debug.Log(didPath);

        if (!didPath && !isWalking) // Pathing ended, update state
        {
            Debug.Log("isWalking if satisfied");
            didPath = true;
            heartParticles.Play();
            pet.happiness += 2;
            pet.love += 2;
            petAnimator.SetBool("isWalking", false);
            StateMeterCtrl.SetStateMeter(pet);
        }
    }

    void OnAnimatorMove()
    {
        // Update position to agent position
        transform.position = petNavMeshAgent.nextPosition;
    }

    public void Setting(string code)
    {
        SpeechToText.instance.Setting(code);
    }

    public void LookInPlace(Vector3 direction)
    {
        if (petNavMeshAgent != null && this.petNavMeshAgent.enabled)
        {
            petNavMeshAgent.updatePosition = false;
            petNavMeshAgent.updateRotation = true;
            petNavMeshAgent.SetDestination(direction);
        }
    }

    public void GoToUser()
    {
        playerCameraPosition = GetCameraPosition();
        Debug.Log("playerCameraPosition");
        Debug.Log(playerCameraPosition);
        didPath = false;
        petNavMeshAgent.updatePosition = true; // As opposed to the 'look' command
        petNavMeshAgent.destination = new Vector3(playerCameraPosition.position.x, 0.25f, playerCameraPosition.position.z);
        petAnimator.SetBool("isWalking", true);
    }

    public Transform GetCameraPosition()
    {
        Transform cameraPosition = GameObject.Find("AR Scene Camera").transform;
        return cameraPosition;
    }

    void OnResultSpeech(string _data)
    {
        if (_data == null || _data.Length < 1 || _data == "nil")
        {
            return;
        }
        else
        {
            Debug.Log(_data.ToString());
            inputText.text = _data;
            words = _data.Split(' ');

            if (words.Length > 0)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    switch (words[i].ToLower())
                    {
                        case "quiet":
                            {
                                if (petAnimator != null) petAnimator.SetBool("isHowling", false);
                                break;
                            }
                        case "dead":
                            {
                                // play death animation
                                Debug.Log(petAnimator.GetBool("isDead"));
                                if (petAnimator != null) petAnimator.SetBool("isDead", true);
                                Debug.Log(petAnimator.GetBool("isDead"));
                                break;
                            }
                        case "up":
                            {
                                if (petAnimator != null) petAnimator.SetBool("isDead", false);
                                break;
                            }
                        case "stay":
                            {
                                petNavMeshAgent.destination = petNavMeshAgent.transform.position;
                                break;
                            }
                        case "come":
                            {
                                Debug.Log("Come called");
                                GoToUser();
                                break;
                            }
                        case "look":
                            {
                                LookInPlace(GetCameraPosition().transform.position);
                                break;
                            }
                        case "sing":
                            {
                                Debug.Log(petAnimator.GetBool("isHowling"));
                                if (petAnimator != null) petAnimator.SetBool("isHowling", true);
                                Debug.Log(petAnimator.GetBool("isHowling"));
                                petAudioSource.clip = audioClips[0];
                                petAudioSource.Play();
                                // play howl animation
                                break;
                            }
                        default:
                            {

                                playPetNoise();
                                break;
                            }
                    }
                }
            }
        }
    }
}

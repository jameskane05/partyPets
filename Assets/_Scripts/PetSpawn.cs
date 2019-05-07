using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SixDegrees;
using UnityEngine.AI;

public class PetSpawn : MonoBehaviour
{
    public AccountCtrl instance;
    public Text petName;
    public Transform stateMeter;
    public Slider xpSlider;
    public Text levelText;
    public Text tendaCounter;
    public PetType activePetType;
    public bool hasRelocalized = false;
    public GameObject petGameObj;
    public GameObject[] availablePets;

    private Account account;
    private Pet activePet;
    private bool hasSpawned = false;

    public static AccountCtrl accountCtrlInstance;

    private void Start()
    {
        instance = AccountCtrl.GetInstance();
        activePetType = instance.GetActivePet().type;
        activePet = instance.GetActivePet();
        petName.text = activePet.name;
        StateMeterCtrl.SetStateMeter(activePet, stateMeter);
        xpSlider.minValue = instance.XpForNextLevel(activePet.level - 1);
        xpSlider.maxValue = instance.XpForNextLevel(activePet.level);
        xpSlider.value = activePet.xp;
        levelText.text = activePet.level.ToString();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Tapped on a GameObject, which was:");
                Debug.Log(EventSystem.current.currentSelectedGameObject);

                // determine if the GameObject is the pet, toy or UI


                return;  // on the UI, let other scripts handle it");

            }
            else if (
                EventSystem.current.currentSelectedGameObject == null
                && touch.phase == TouchPhase.Began)
            {
                // Get the x & y position of the touch on the screen
                Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, .2f);
                Vector3 position = Camera.main.ScreenToWorldPoint(screenPosition);
                Debug.Log("AR position:");
                Debug.Log(position);
                // TODO: Handle touch interactions
                if (!hasSpawned)
                {
                    hasSpawned = true;
                    SpawnPet();
                }
            }
        } 
    }
        
    void UpdatePetStateOnInit() {
        DateTime timeSinceLastVisit = DateTime.Parse(activePet.lastVisited);
        TimeSpan ts = DateTime.Now.Subtract(timeSinceLastVisit);

        // TODO: Update pet state vals depending on how long it's been
    }

    void SpawnPet()
    {
        Quaternion rotation = petGameObj.transform.rotation;
        rotation = Quaternion.Euler(rotation.x, -180f, rotation.y);
        petGameObj = Instantiate(
            availablePets[(int)activePetType],
            activePet.positionOnMesh,
            rotation);
        petGameObj.name = activePet.type.ToString();  // this refers to the GO name, not the user's pet's name
        StateMeterCtrl.SetStateMeter(activePet);
    }

    private void OnDestroy()
    {
        activePet.positionOnMesh = petGameObj.transform.position;
        activePet.rotationOnMesh = petGameObj.transform.rotation;
        activePet.lastVisited = System.DateTime.Now.ToString();
        instance.account.pets[activePet.id] = activePet;
        instance.SaveAccount();
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuCtrl : MonoBehaviour {
    public GameObject petList;
    public GameObject spaceList;
    public GameObject petCardPrefab;
    public GameObject spaceCardPrefab;
    public Text tendaCounter;

    public ToggleGroup activePetToggle;
    public ToggleGroup activeSpaceToggle;

    public Texture[] spacePortraits;
    public Texture[] petPortraits;
    public Texture[] genderIcons;

    public static AccountCtrl instance;

    // Use this for initialization
    void Start () {
        instance = AccountCtrl.GetInstance();
        CreatePetCards();
        CreateSpaceCards();
        tendaCounter.text = instance.account.tenda.ToString();
    }

    void CreateSpaceCards ()
    {
        foreach (Space space in instance.account.spaces)
        {
            GameObject newSpaceCard = Instantiate(spaceCardPrefab);
            Transform spaceNameObj = newSpaceCard.transform.Find("Name");

            if (space.isActive)
            {
                newSpaceCard.GetComponent<Toggle>().isOn = true;
            }

            Transform portraitMask = newSpaceCard.transform.Find("Portrait");
            Transform spacePortrait = portraitMask.Find("RawImage");
            spacePortrait.GetComponent<RawImage>().texture = spacePortraits[0];

            newSpaceCard.transform.parent = spaceList.transform;
            newSpaceCard.transform.SetSiblingIndex(space.id);
        }
        ContentSizeFitter sizeFitter = spaceList.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;  // must add and fit after list is created
    }

    void CreatePetCards ()
    {
        foreach (Pet pet in instance.account.pets)
        {
            GameObject newPetCard = Instantiate(petCardPrefab);
            Transform petNameObj = newPetCard.transform.Find("Name");
            Transform petGenderObj = newPetCard.transform.Find("Gender");
            Text petAge = newPetCard.transform.Find("Age").GetComponent<Text>();
            petNameObj.GetComponent<Text>().text = pet.name + " the " + pet.type.ToString();
            if (pet.gender == Gender.Male)
            {
                petGenderObj.GetComponent<RawImage>().texture = genderIcons[0];
            }
            if (pet.gender == Gender.Female)
            {
                petGenderObj.GetComponent<RawImage>().texture = genderIcons[1];
            }

            DateTime when = DateTime.Parse(pet.createdAt);
            TimeSpan ts = DateTime.Now.Subtract(when);
            if (ts.TotalHours < 1)
                petAge.text = (int)ts.TotalMinutes + " minutes old";
            else if (ts.TotalDays < 1)
                petAge.text = (int)ts.TotalHours + " hours old";


            Transform stateMeter = newPetCard.gameObject.transform.Find("State");
           
            StateMeterCtrl.SetStateMeter(pet, stateMeter);

            Toggle activeToggle = newPetCard.GetComponent<Toggle>();
            if (pet.isActive)
            {
                activeToggle.isOn = true;
            }

            Transform portraitMask = newPetCard.transform.Find("Portrait");
            Transform petPortrait = portraitMask.Find("RawImage");
            if (pet.type == PetType.Wolf)
            {
                petPortrait.GetComponent<RawImage>().texture = petPortraits[0];
            }
            else if (pet.type == PetType.Cat)
            {
                petPortrait.GetComponent<RawImage>().texture = petPortraits[1];
                petPortrait.GetComponent<RectTransform>().localPosition = new Vector3(35,(float)-19.5,0);
            }
            else if (pet.type == PetType.Chihuahua)
            {
                petPortrait.GetComponent<RawImage>().texture = petPortraits[2];
            }
            else if (pet.type == PetType.Horse)
            {
                petPortrait.GetComponent<RawImage>().texture = petPortraits[3];
            }
            else if (pet.type == PetType.Penguin)
            {
                petPortrait.GetComponent<RawImage>().texture = petPortraits[4];
            }
            else if (pet.type == PetType.Penguin)
            {
                petPortrait.GetComponent<RawImage>().texture = petPortraits[5];
            }

            newPetCard.transform.parent = petList.transform;
            newPetCard.transform.SetSiblingIndex(pet.id);
        }
        ContentSizeFitter sizeFitter = petList.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;  // must add and fit after list is created
    }

    public void LoadGame()
    {
        // get active pet from UI

        // get active space from UI
        // save account
        SceneManager.LoadScene(1);
    }

    void SetActivePet()
    {
        foreach (Pet pet in instance.account.pets)
        {
            pet.isActive = false;
        }

        IEnumerable<Toggle> activeToggle = activePetToggle.ActiveToggles();
        foreach (Toggle toggle in activeToggle)
        {
            instance.account.pets[toggle.transform.GetSiblingIndex()].isActive = true; 
        }
    }

    void SetActiveSpace()
    {
        foreach (Space space in instance.account.spaces)
        {
            space.isActive = false;
        }

        IEnumerable<Toggle> activeToggle = activeSpaceToggle.ActiveToggles();
        foreach (Toggle toggle in activeToggle)
        {
            instance.account.spaces[toggle.transform.GetSiblingIndex()].isActive = true;
        }
    }
}

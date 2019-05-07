using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePet : MonoBehaviour {

    public InputField petName;
    public ToggleGroup petTypeToggle;
    public ToggleGroup genderToggle;
    public InputField spaceName;

    public static AccountCtrl instance = AccountCtrl.GetInstance();

    public void NewPet()
    {

        if (this.petTypeToggle.AnyTogglesOn() && this.petName.text != "")
        {
            IEnumerable<Toggle> selectedToggle = petTypeToggle.ActiveToggles();
            PetType petType = PetType.Wolf;  // need to set a default or linter complains, immediately override
            foreach (Toggle toggle in selectedToggle)
            {
                petType = (PetType) Enum.Parse(typeof(PetType), toggle.name);
            }

            selectedToggle = genderToggle.ActiveToggles();
            Gender petGender = Gender.Transgender;  // need to set a default or linter complains, immediately override
            foreach (Toggle toggle in selectedToggle)
            {
                petGender = (Gender)Enum.Parse(typeof(Gender), toggle.name);
            }

            Pet newPet = new Pet
            {
                id = instance.account.pets.Length,
                isActive = false,
                name = petName.text,
                type = petType,
                gender = petGender,
                positionOnMesh = new Vector3(0, 0, 0),
                rotationOnMesh = new Quaternion(0, 0, 0, 0),
                persistenceId = "12345",
                love = 5,
                food = 5,
                water = 5,
                health = 6,
                happiness = 5,
                createdAt = System.DateTime.Now.ToString(),
                lastVisited = System.DateTime.Now.ToString()
            };

            List<Pet> petList = new List<Pet>();
            foreach (Pet pet in instance.account.pets)
            {
                petList.Add(pet);

            }
            petList.Add(newPet);
            instance.account.pets = petList.ToArray();

            instance.SaveAccount();
        }
    }
}

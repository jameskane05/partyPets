using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class AccountCtrl {

    public Account account;

    private static AccountCtrl _instance;

    public static AccountCtrl GetInstance()
    {
        if (_instance == null) _instance = new AccountCtrl();
        return _instance;
    }

    private AccountCtrl() {
        account = GetAccountFromDisk();
    }

    public void SaveAccount()
    {
        string json = JsonUtility.ToJson(account);
        Debug.Log(json);
        File.WriteAllText(Application.persistentDataPath + "/account.json", json);
    }

    public Account GetAccountFromDisk()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/account.json"))
        {
            string savedAccount = File.ReadAllText(Application.persistentDataPath + "/account.json");
            Debug.Log(savedAccount);
            return JsonUtility.FromJson<Account>(savedAccount);
        }
        else
        {
            Account newAccount = GenerateExampleAccount();
            account = newAccount;
            SaveAccount();
            return newAccount; ;
        }
    }

    public Pet GetActivePet()
    {
        Pet activePet = Array.Find<Pet>(account.pets, (pet) => { return pet.isActive; });
        return activePet;
    }

    public Space GetActiveSpace()
    {
        Space activeSpace = Array.Find<Space>(account.spaces, (space) => { return space.isActive; });
        return activeSpace;
    }

    public void GrantXp(int xp)
    {
        Pet activePet = GetActivePet();
        activePet.xp += xp;

        if (activePet.xp >= activePet.nextLevelAt)
        {
            activePet.level += 1;
            activePet.nextLevelAt = XpForNextLevel(activePet.level);
        }

        account.pets[activePet.id] = activePet;
        SaveAccount();
    }

    public int XpForNextLevel(int level)
    {
        return 500 * (level ^ 2) - (500 * level);
    }

    Account GenerateExampleAccount()
    {
        Pet newPetA = new Pet
        {
            id = 0,
            isActive = true,
            name = "Balto",
            type = PetType.Wolf,
            gender = Gender.Male,
            positionOnMesh = new Vector3(0, 0, 0),
            rotationOnMesh = new Quaternion(0, 0, 0, 0),
            persistenceId = "12345",
            love = 5,
            food = 5,
            water = 5,
            health = 6,
            happiness = 5,
            createdAt = System.DateTime.Now.ToString(),
            lastVisited = System.DateTime.Now.ToString(),
            level = 1,
            xp = 1001
        };

        Pet newPetB = new Pet
        {
            id = 1,
            isActive = false,
            name = "Crookshanks",
            type = PetType.Cat,
            gender = Gender.Female,
            positionOnMesh = new Vector3(0, 0, 0),
            rotationOnMesh = new Quaternion(0, 0, 0, 0),
            persistenceId = "123456",
            love = 5,
            food = 5,
            water = 5,
            health = 6,
            happiness = 5,
            createdAt = System.DateTime.Now.ToString(),
            lastVisited = System.DateTime.Now.ToString(),
            level = 2,
            xp = 3001
        };

        Space newSpaceA = new Space
        {
            id = 0,
            name = "Home",
            isActive = true,
            persistenceId = "321",
            createdAt = System.DateTime.Now.ToString()
        };

        Pet[] petArray = new Pet[2];
        petArray[0] = newPetA;
        petArray[1] = newPetB;

        Space[] spaceArray = new Space[1];
        spaceArray[0] = newSpaceA;

        Account newAccount = new Account
        {
            id = 0,
            firstName = "James",
            lastName = "Kane",
            email = "jameskane05@gmail.com",
            password = "password",
            pets = petArray,
            spaces = spaceArray,
            createdAt = System.DateTime.Now.ToString(),
            tenda = 100
        };

        return newAccount;
    }
}

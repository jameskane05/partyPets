using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public static class ToggleGroupExtension
{
    public static Toggle GetActive(this ToggleGroup aGroup)
    {
        return aGroup.ActiveToggles().FirstOrDefault();
    }
}

public enum PetType
{
    Wolf, Cat, Chihuahua, Horse, Penguin, Shark, Unicorn, RedPanda, SugarGlider
}

public enum Gender
{
    Female, Male, Transgender
}

[System.Serializable]
public class Account
{
    public int id;
    public string firstName;
    public string lastName;
    public string email;
    public string password;
    public string createdAt;
    public Pet[] pets;
    public Space[] spaces;
    public Toy[] toys;
    public int tenda;
}

[System.Serializable]
public class Space
{
    public int id;
    public string name;
    public bool isActive;
    public string persistenceId;
    public string createdAt;
}

[System.Serializable]
public class Pet {
    public int id;
    public string name;
    public bool isActive;
    public PetType type;
    public Gender gender;
    public Vector3 positionOnMesh;
    public Quaternion rotationOnMesh;
    public string lastVisited;
    public string createdAt;
    public string persistenceId;
    public string portraitFilePath;
    public float love;
    public float food;
    public float water;
    public float health;
    public float happiness;
    public int level;
    public int nextLevelAt;
    public int xp;
    public Skill[] learnedSkills;
}

[System.Serializable]
public class Skill {
    public int id;
    public string name;
    public PetType[] availableToPetsOfType;
    public string voiceCommand;
    public string requiredLevel;
    public AnimationClip animation;
}

[System.Serializable]
public class Toy
{
    public int id;
    public string name;
    public PetType[] availableToPetsOfType;
    public GameObject model;
}

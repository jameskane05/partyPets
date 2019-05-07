using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateMeterCtrl : MonoBehaviour {
    public static void SetStateMeter(Pet pet, Transform stateMeter = null)
    {
        if (stateMeter == null)
        {
            stateMeter = GameObject.Find("State").transform;
        }

        Slider loveMeter = stateMeter.Find("Love").GetComponent<Slider>();
        Slider happinessMeter = stateMeter.Find("Happiness").GetComponent<Slider>();
        Slider foodMeter = stateMeter.Find("Food").GetComponent<Slider>();
        Slider waterMeter = stateMeter.Find("Water").GetComponent<Slider>();
        Slider healthMeter = stateMeter.Find("Health").GetComponent<Slider>();

        loveMeter.value = pet.love;
        happinessMeter.value = pet.happiness;
        foodMeter.value = pet.food;
        waterMeter.value = pet.water;
        healthMeter.value = pet.health;
    }
}

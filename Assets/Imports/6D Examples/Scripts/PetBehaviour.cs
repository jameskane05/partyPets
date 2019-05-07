using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBehaviour : MonoBehaviour
{
	public float maxPetDistance = 30.0f;

    void Update()
    {
        if (Vector3.Distance(this.transform.position, Camera.main.transform.position) > maxPetDistance)
        {
            Destroy(this.gameObject);
        }
    }
}

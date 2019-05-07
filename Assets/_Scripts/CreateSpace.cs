using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateSpace : MonoBehaviour {

    public InputField spaceName;
    public static AccountCtrl accountCtrlInstance = AccountCtrl.GetInstance();

    public void NewSpace()
    {
        Space newSpace = new Space
        {
            id = accountCtrlInstance.account.spaces.Length,
            isActive = false,
            name = spaceName.text,
            createdAt = System.DateTime.Now.ToString()
        };

        List<Space> spaceList = new List<Space>();
        foreach (Space space in accountCtrlInstance.account.spaces)
        {
            spaceList.Add(space);

        }
        spaceList.Add(newSpace);
        accountCtrlInstance.account.spaces = spaceList.ToArray();

        accountCtrlInstance.SaveAccount();
    }
}

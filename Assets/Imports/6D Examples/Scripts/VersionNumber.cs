/***********************************************************
* Copyright (C) 2018 6degrees.xyz Inc.
*
* This file is part of the 6D.ai Beta SDK and is not licensed
* for commercial use.
*
* The 6D.ai Beta SDK can not be copied and/or distributed without
* the express permission of 6degrees.xyz Inc.
*
* Contact developers@6d.ai for licensing requests.
***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SixDegrees;

public class VersionNumber : MonoBehaviour
{
    public Text versionNumber;

    void Update()
    {
        if (SDPlugin.IsSDKReady && SDPlugin.Version != null)
        {
            versionNumber.text = "v" + SDPlugin.Version;
            this.enabled = false;
        }
    }
}

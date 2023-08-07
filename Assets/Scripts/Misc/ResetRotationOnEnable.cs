﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRotationOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
}

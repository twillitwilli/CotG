﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public RoomModel roomModel;

    public bool isSpecialRoom, disableSpawning;
    public List<GameObject> roomSpawners, roomOpenings;

    [HideInInspector]
    public GameObject specialRoomParent;

    private void Awake()
    {
        if (!isSpecialRoom) 
        { 
            DungeonGenerationV3.Instance.roomCount++; 
            DungeonGenerationV3.Instance.spawnedRooms.spawnedRooms.Add(transform.parent.gameObject);
        }

        else
            specialRoomParent = GetComponentInParent<SpecialRoom>().gameObject;
    }
}

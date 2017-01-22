﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundSetup : MonoBehaviour {

    List<PlayerControl> players;

    public void SetupPlayers(List<GameVariable.controlerType> cntrlTypes)
    {
        players = new List<PlayerControl>();
        int currentPid = 0;
        foreach(GameVariable.controlerType cntrlType in cntrlTypes)
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/PlayerHolder")) as GameObject;
            PlayerControl pcs = go.GetComponentInChildren<PlayerControl>();
            float xcord = Random.Range(0.3f, .7f) * GV.World_Size_X;
            float zcord = Random.Range(0.3f, .7f) * GV.World_Size_Z;
            float yCord = WorldGrid.Instance.GetHeightAt(new Vector2(xcord, zcord)) + 1;
            pcs.Initialize(new Vector3(xcord, yCord, zcord));
            go.GetComponentInChildren<InputManager>().controlType = cntrlType;
            go.GetComponentInChildren<InputManager>().Initialize(cntrlType);
        }
        SetupCameras(players);
    }

    public void SetupCameras(List<PlayerControl> _players)
    {
        switch(players.Count)
        {
            case 1:
                break;
            case 2:
                Setup2Player(_players);
                break;
            case 3:
                Setup3Player(_players);
                break;
            case 4:
                Setup4Player(_players);
                break;
        }
    }

    private void Setup2Player(List<PlayerControl> _players)
    {
        for(int i = 0; i < 2; i++)
        {
            Rect playerViewRect = (i == 0) ? new Rect(0, 0, .5f, 1) : new Rect(.5f, .5f, .5f, 1);
            _players[i].gameObject.GetComponentInChildren<Camera>().rect = playerViewRect;
        }
    }

    private void Setup3Player(List<PlayerControl> _players)
    {
        for (int i = 0; i < 2; i++)
        {
            Rect playerViewRect = (i == 0) ? new Rect(0, 0, .5f, .5f) : new Rect(.5f, .5f, .5f, .5f);
            _players[i].gameObject.GetComponentInChildren<Camera>().rect = playerViewRect;
        }
        Rect playerViewRect2 = new Rect(0, .5f, 1f, .5f);
        _players[2].gameObject.GetComponentInChildren<Camera>().rect = playerViewRect2;
    }

    private void Setup4Player(List<PlayerControl> _players)
    {
        _players[0].gameObject.GetComponentInChildren<Camera>().rect = new Rect(0, 0f, .5f, .5f);
        _players[1].gameObject.GetComponentInChildren<Camera>().rect = new Rect(0, .5f, .5f, .5f);
        _players[2].gameObject.GetComponentInChildren<Camera>().rect = new Rect(.5f, 0f, .5f, .5f);
        _players[3].gameObject.GetComponentInChildren<Camera>().rect = new Rect(.5f, .5f, .5f, .5f);
    }





}

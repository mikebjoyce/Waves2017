using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundSetup : MonoBehaviour {

    List<PlayerControl> players;

    public void SetupPlayers(List<GameVariable.controlerType> cntrlTypes)
    {
        players = new List<PlayerControl>();
        foreach(GameVariable.controlerType cntrlType in cntrlTypes)
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/PlayerHolder")) as GameObject;
            PlayerControl pcs = go.GetComponentInChildren<PlayerControl>();
            int xcord =  (int)(Random.Range(0.3f, .7f) * GV.World_Size_X);
            int zcord =  (int)(Random.Range(0.3f, .7f) * GV.World_Size_Z);
            float yCord = WorldGrid.Instance.GetPillarStaticHeight(new GridPos(xcord, zcord)) + 3;
            pcs.Initialize(new Vector3(xcord, yCord, zcord));
            go.GetComponentInChildren<InputManager>().controlType = cntrlType;
            go.GetComponentInChildren<InputManager>().Initialize(cntrlType);
            players.Add(pcs);
        }
        SetupCameras(players);
        DropBook();
    }

    private void DropBook()
    {
        int x = (int)(Random.Range(0.4f, .6f) * GV.World_Size_X);
        int z = (int)(Random.Range(0.5f, .6f) * GV.World_Size_Z);
        float y = WorldGrid.Instance.GetPillarStaticHeight(new GridPos(x, z)) + 1;
        GameObject go = Instantiate(Resources.Load("Prefabs/Book")) as GameObject;
        go.transform.position = new Vector3(x, y, z);
        GV.theOneBook = go.GetComponent<Book>();
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
        _players[0].playerCam.rect = new Rect(0, 0f, .5f, 1f);
        _players[1].playerCam.rect = new Rect(.5f, 0f,  .5f, 1f);
    }

    private void Setup3Player(List<PlayerControl> _players)
    {
        _players[0].playerCam.rect = new Rect(0f,  .5f, .5f, .5f);
        _players[1].playerCam.rect = new Rect(.5f, .5f,.5f, .5f);
        _players[2].playerCam.rect = new Rect(0f,  0f, 1f, .5f);
    }

    private void Setup4Player(List<PlayerControl> _players)
    {
        _players[0].playerCam.rect = new Rect(0, 0f, .5f, .5f);
        _players[1].playerCam.rect = new Rect(0, .5f, .5f, .5f);
        _players[2].playerCam.rect = new Rect(.5f, 0f, .5f, .5f);
        _players[3].playerCam.rect = new Rect(.5f, .5f, .5f, .5f);
    }





}

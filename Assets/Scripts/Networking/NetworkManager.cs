using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;

public class NetworkManager : SingletonPUN<NetworkManager>
{
    // Events:
    public const byte SCORE_UPDATED_EVENT_CODE = 1;

    private const string PLAYER_PREFAB_NAME = "Player";
    [SerializeField]
    private Sprite[] playerIcons;
    [SerializeField]
    private Sprite[] playerBullets;

    public bool IsInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        if (!PhotonNetwork.IsConnected)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            return;
        }

        // Spawn the player
        GameObject player = PhotonNetwork.Instantiate(PLAYER_PREFAB_NAME, Vector3.zero, Quaternion.identity);
        IsInitialized = true;
    }

    public Sprite GetPlayerIcon(int id)
    {
        if (id > -1 && id < playerIcons.Length)
        {
            return playerIcons[id];
        }
        else
        {
            Debug.LogWarning($"Cannot access sprite with id {id}");
        }
        return null;
    }

    public Sprite GetPlayerBullet(int id)
    {
        if (id > -1 && id < playerBullets.Length)
        {
            return playerBullets[id];
        }
        else
        {
            Debug.LogWarning($"Cannot access sprite with id {id}");
        }
        return null;
    }
}

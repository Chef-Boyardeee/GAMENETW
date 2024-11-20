using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExitGames.Client.Photon;
using System.Linq;
using System.Xml.Linq;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] scoreboardRows;
    [SerializeField] private GameObject[] stuffToHide;

    private void OnEnable()
    {
        HideElements();
        InitializeScoreboard();
        InputData();
    }

    private void HideElements()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerController>().CancelInvoke();
            p.SetActive(false);
        }

        foreach (GameObject element in stuffToHide)
        {
            element.SetActive(false);
        }

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            //isInactive = true;
        }
    }

    private void InitializeScoreboard()
    {
        for(int i = 0; i < scoreboardRows.Length; i++)
        {
            scoreboardRows[i].transform.Find("PlayerSprite").gameObject.SetActive(false);
            scoreboardRows[i].transform.Find("PlayerName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            scoreboardRows[i].transform.Find("PlayerScore").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        }
    }
    private void InputData()
    {
        var sortedPlayerList = (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();
        for (int i = 0; i < sortedPlayerList.Count; i++)
        {
            Debug.Log($"{sortedPlayerList[i] != null}");
            scoreboardRows[i].transform.Find("PlayerSprite").gameObject.SetActive(true);
            scoreboardRows[i].transform.Find("PlayerSprite").gameObject.GetComponent<Image>().sprite = NetworkManager.Instance.GetPlayerIcon(sortedPlayerList[i].GetPlayerNumber());
            scoreboardRows[i].transform.Find("PlayerName").gameObject.GetComponent<TextMeshProUGUI>().text = sortedPlayerList[i].NickName;
            scoreboardRows[i].transform.Find("PlayerScore").gameObject.GetComponent<TextMeshProUGUI>().text = sortedPlayerList[i].GetScore().ToString();
        }
    }
}

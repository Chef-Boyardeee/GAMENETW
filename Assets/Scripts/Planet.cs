using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Planet : MonoBehaviourPunCallbacks
{
    private float maxHP = 30f;
    private float currentHP;
    [SerializeField] private TextMeshProUGUI labelHP;
    [SerializeField] private Image fillHP;
    [SerializeField] private GameObject scoreboard;

    private bool isDestroyed = false;

    private void Awake()
    {
        base.OnEnable();
        isDestroyed = false;
        currentHP = maxHP;
        UpdateHealth();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
        {
            photonView.RPC("RPCTakeDamage",RpcTarget.AllBuffered,enemy.Damage);
            enemy.DestroyOverNetwork();
            //Grant score to the player that destroyed the enemy
            if (currentHP <= 0)
            {
                photonView.RPC("RPCDeath", RpcTarget.AllBuffered);
                DestroyOverNetwork();
            }
            //Got hit by bullet
        }
    }

    [PunRPC]
    private void RPCTakeDamage(float Damage)
    {
        Debug.Log("RPC called");
        currentHP -= Damage;
        Debug.Log("Damage has been taken");
        UpdateHealth();
        Debug.Log("HP has been updated. Ending function");
    }

    [PunRPC]
    private void RPCDeath()
    {
        scoreboard.SetActive(true);
    }

    private void UpdateHealth()
    {
        fillHP.fillAmount = currentHP / maxHP;
        labelHP.text = string.Format("{0:00}/{1:00}", currentHP, maxHP); 
    }

    public void DestroyOverNetwork()
    {
        // Since the enemy is Instantiated by the Master Client
        // Only the master client has authority over the object
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
        // If we're not the master client,
        // We simply set the boolean flag
        else
        {
            isDestroyed = true;
        }
    }
}

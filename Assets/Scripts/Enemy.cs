using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.Demo.Asteroids;

public class Enemy : MonoBehaviourPunCallbacks
{
    [SerializeField] private float moveSpeed = 5.0f;
    private float maxHP = 20f;
    private float currentHP;
    private float damage = 1f;
    public float Damage => damage;

    private Transform target = null;
    private bool isDestroyed = false;
    [SerializeField] private Image fillHP;
    [SerializeField] private GameObject barHP;

    public override void OnEnable(){
        base.OnEnable();
        LookAtTarget();
        isDestroyed = false;
        currentHP = maxHP;
        UpdateHealth();
    }

    public void SetTarget(Transform target){
        this.target = target;
    }

    private void Update(){
        if (isDestroyed) return;
        Move();
    }
    
    private void LookAtTarget(){
        Quaternion newRotation;
        Vector3 targetDirection = target == null ? transform.position : transform.position - target.transform.position;
        newRotation = Quaternion.LookRotation(targetDirection, Vector3.forward);
        newRotation.x = 0;
        newRotation.y = 0;
        transform.rotation = newRotation;
    }

    private void Move(){
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        /*if(Camera.main.transform == null)
        {
            Debug.LogError("Main camera was not found");
            return;
        }
        if(hpBar == null)
        {
            Debug.LogError("hpBar was not found");
            return;
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;
        if (collision.gameObject.TryGetComponent<Bullet>(out Bullet bullet))
        {
            photonView.RPC("RPCTakeDamage", RpcTarget.AllBuffered,bullet.Damage,bullet.Owner);
            bullet.DestroyOverNetwork();
            //Got hit by bullet
        }
    }

    

    [PunRPC]
    private void RPCTakeDamage(float Damage, Photon.Realtime.Player player)
    {
        currentHP -= Damage;
        UpdateHealth();

        ScoreManager.Instance.AddScore(10, player);

        if (currentHP <= 0)
        {
            ScoreManager.Instance.AddScore(100, player);
            DestroyOverNetwork();
        }
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

    private void UpdateHealth()
    {
        fillHP.fillAmount = currentHP / maxHP;
    }
}

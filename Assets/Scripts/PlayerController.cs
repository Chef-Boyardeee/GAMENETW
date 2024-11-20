using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using Photon.Realtime;
using System.Collections;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float movementRadius = 1.5f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float currentSpeed;
    private float timeCounter;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PlayerNumbering.OnPlayerNumberingChanged += AssignSprite;
        if (!photonView.IsMine) return;
        InvokeRepeating("ShootBullet", 00.01f, fireRate);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PlayerNumbering.OnPlayerNumberingChanged -= AssignSprite;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Get the player input to determine the diretion of the movement
        float movementInput = Input.GetAxis("Horizontal");
        currentSpeed = movementInput * Time.deltaTime * maxSpeed;
        timeCounter += currentSpeed;

        // Circular motion based on the movement radius
        float x = Mathf.Cos(timeCounter) * movementRadius;
        float y = Mathf.Sin(timeCounter) * movementRadius;

        transform.position = new Vector2(x, y);
    }

    private void HandleRotation()
    {
        // Define a quaternion that will make the player face outwards of the circle
        Quaternion newRotation = Quaternion.LookRotation(-transform.position, Vector3.forward);
        // Disregard rotation in the x and y since we are working on 2D space
        newRotation.x = 0;
        newRotation.y = 0;
        transform.rotation = newRotation;
    }

    private void ShootBullet()
    {
        var bulletObject = PhotonNetwork.Instantiate("Bullet", transform.position, transform.rotation);
        if(bulletObject.TryGetComponent<Bullet>(out Bullet bullet))
        {
            bullet.InitializeValues(baseDamage, photonView.Owner);
        }
    }
    
    public void AssignSprite()
    {
        photonView.RPC("RPCAssignSprite", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPCAssignSprite()
    {
        // GetPlayerNumber is only accessible through Photon.Pun.UtilityScripts
        spriteRenderer.sprite = NetworkManager.Instance.GetPlayerIcon(photonView.Owner.GetPlayerNumber());
    }

    public void DestroyOverNetwork()
    {
        // Only the player that spawned the object can destroy it
        // Because the bullet is spawned by the player
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}

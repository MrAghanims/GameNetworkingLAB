using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using Cinemachine;

public class PlayerNetwork : NetworkBehaviour
{
    [Networked] public int HP { get; set; } = 100;
    [Networked] public string PlayerName { get; set; }
    [Networked] public bool IsReady { get; set; }
    [Networked] public int Score { get; set; }
    private float damageTimer;
    public float damageInterval = 0.25f;

    private bool movementDisabled = false;
    private TextMeshProUGUI deathText;
    private TextMeshProUGUI respawnText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI readyText;

    private CharacterController controller;
    private float verticalVelocity;
    public float gravity = -20f;

    public float speed = 5f;
    private bool isDead;
    private float respawnTimer;
    public float respawnDelay = 10f;

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerName(string newName)
    {
        PlayerName = newName;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (isDead)
        {
            respawnTimer -= Runner.DeltaTime;

            if (Object.HasInputAuthority && respawnText != null)
            {
                respawnText.text =
                    "Respawning in " +
                    Mathf.Ceil(respawnTimer).ToString();
            }

            if (respawnTimer <= 0f)
            {
                Respawn();
            }

            return;
        }
        if (movementDisabled)
            return;
        UpdateUI();

        if (!Object.HasStateAuthority)
            return;

        if (!GetInput(out NetworkInputData data))
            return;

        Vector3 move =
            data.CamForward * data.Move.y +
            data.CamRight * data.Move.x;

        move.y = 0;

        // Rotate toward movement direction
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(move);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    12f * Runner.DeltaTime);
        }

        // Gravity
        if (controller.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Runner.DeltaTime;

        Vector3 finalMove = move * speed;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Runner.DeltaTime);
    }

    void UpdateUI()
    {
        if (nameText != null)
            nameText.text = PlayerName;

        if (readyText != null)
            readyText.text = IsReady ? "READY" : "NOT READY";

        if (hpText != null)
            hpText.text = "HP: " + HP;
    }

    private void OnTriggerEnter(Collider other)
    {
        // READY ZONE
        if (other.CompareTag("ReadyZone"))
        {
            IsReady = true;
            return;
        }

        // DAMAGE ZONE
        if (other.CompareTag("DamageZone"))
        {
            damageTimer = 0f;
            return;
        }

        // COLLECTIBLE
        if (!other.CompareTag("Collectible"))
            return;

        if (!Object.HasStateAuthority)
            return;

        Score++;

        NetworkObject obj = other.GetComponent<NetworkObject>();

        if (obj != null)
        {
            Runner.Despawn(obj);

            if (CollectibleSpawner.Instance != null)
            {
                CollectibleSpawner.Instance.SpawnCollectible();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!Object.HasStateAuthority)
            return;

        if (!other.CompareTag("DamageZone"))
            return;

        damageTimer += Time.deltaTime;

        if (damageTimer >= damageInterval)
        {
            damageTimer = 0f;
            TakeDamage(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReadyZone"))
        {
            IsReady = false;
        }
        if (other.CompareTag("DamageZone"))
        {
            damageTimer = 0f;
        }
    }

    public override void Spawned()
    {
        deathText = UIManager.Instance.deathText;
        respawnText = UIManager.Instance.respawnText;
        Debug.Log(
        $"Spawned: {PlayerName} | InputAuth={Object.HasInputAuthority} | StateAuth={Object.HasStateAuthority}"

    );
        if (Object.HasInputAuthority)
        {
            RPC_SetPlayerName(NetworkManager.LocalPlayerName);

            CinemachineFreeLook freeLook =
                FindObjectOfType<CinemachineFreeLook>();

            freeLook.Follow = transform;
            freeLook.LookAt = transform;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority)
            return;

        if (isDead)
            return;

        HP -= damage;

        if (HP <= 0)
        {
            HP = 0;
            Die();
        }
    }
    void Respawn()
    {
        isDead = false;
        HP = 100;

        controller.enabled = true;

        transform.position = new Vector3(0, 1, 0);

        if (Object.HasInputAuthority)
        {
            if (deathText != null)
                deathText.gameObject.SetActive(false);

            if (respawnText != null)
                respawnText.gameObject.SetActive(false);
        }
    }
    void Die()
    {
        isDead = true;
        respawnTimer = respawnDelay;

        controller.enabled = false;

        if (Object.HasInputAuthority)
        {
            if (deathText != null)
                deathText.gameObject.SetActive(true);

            if (respawnText != null)
                respawnText.gameObject.SetActive(true);
        }
    }
    public void DisablePlayer()
    {
        movementDisabled = true;
    }

}
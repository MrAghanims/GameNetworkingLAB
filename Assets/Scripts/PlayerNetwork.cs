using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [Networked] public string PlayerName { get; set; }
    [Networked] public bool IsReady { get; set; }
    [Networked] public int Score { get; set; }

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI readyText;

    private CharacterController controller;

    public float speed = 5f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 move =
                new Vector3(h, 0, v);

            controller.Move(move * speed * Runner.DeltaTime);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        nameText.text = PlayerName;

        readyText.text =
            IsReady ? "READY" : "NOT READY";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ReadyZone"))
        {
            IsReady = true;
        }

        if (other.CompareTag("Collectible"))
        {
            Score++;
            if (Score >= 5)
            {
                FindObjectOfType<GameManager>()
                    .EndGame(PlayerName);
            }
            Runner.Despawn(
                other.GetComponent<NetworkObject>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReadyZone"))
        {
            IsReady = false;
        }
    }


}
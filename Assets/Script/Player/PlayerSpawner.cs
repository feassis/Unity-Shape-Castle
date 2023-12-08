using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;
    [SerializeField] private Player playerPrefab;

    private void Start()
    {
        var player = Instantiate(playerPrefab);
        player.transform.position = spawnPos.position;
    }
}

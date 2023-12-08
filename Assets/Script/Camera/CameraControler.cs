using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    [SerializeField] private Transform cameraFocalPoint;
    public static CameraControler Instance;

    private CameraState cameraState;
    private Transform player;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        switch (cameraState)
        {
            case CameraState.None:
                break;
            case CameraState.Follow:
                FollowPlayerUpdate();
                break;
        }
    }

    private void FollowPlayerUpdate()
    {
        cameraFocalPoint.transform.position = player.position;
    }

    public void GoToFollowPlayerMode(Player player)
    {
        this.player = player.transform;
        cameraState = CameraState.Follow;
    }
}

public enum CameraState
{
    None = 0,
    Follow = 1,
}

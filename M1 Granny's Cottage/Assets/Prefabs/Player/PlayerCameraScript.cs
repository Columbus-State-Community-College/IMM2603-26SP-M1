using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CinemachinePositionComposer))]
public class PlayerCameraScript : MonoBehaviour
{
    private CinemachineCamera parentObject; 
    public CinemachinePositionComposer cinemachinePositionComposer;
    [SerializeField] private PlayerController playerController;
    
    void Awake()
    {
        parentObject = FindFirstObjectByType<CinemachineCamera>();
        cinemachinePositionComposer = this.GetComponent<CinemachinePositionComposer>();
    }
    void OnEnable()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        playerController._playerCamera = parentObject;
        
    }
    public void EnableJumpCamera(bool jumping)
    {
        if (jumping)
        {
            cinemachinePositionComposer.Composition.DeadZone.Enabled = true;
        }
        else
        {
            cinemachinePositionComposer.Composition.DeadZone.Enabled = false;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;

[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float MaxSpeed = 3.5f;
    public float Acceleration = 15f;

    [Header("Looking Parameters")]
    public Vector2 LookSensitivity = new Vector2(0.1f, 0.1f);
    public float PitchLimit = 85f;
    [SerializeField] float currentPitch = 0f;

    public float CurrentPitch
    {
        get => currentPitch;

        set
        {
            currentPitch = Mathf.Clamp(value, -PitchLimit, PitchLimit); // limits player looking up and down by 85 degrees
        }
    }

    [Header("Camera Parameters")]
    [SerializeField] float CameraNormalFOV = 60f;

    [Header("Physics Parameters")]
    [SerializeField] float GravityScale = 3f;
    public float VerticalVelocity = 0f;
    public Vector3 CurrentVel { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool IsGrounded => characterController.isGrounded;

    [Header("Input")]
    public Vector2 MoveInput;
    public Vector2 LookInput;

    [Header("Components")]
    [SerializeField] CinemachineCamera fpCamera;
    [SerializeField] CharacterController characterController;

    #region Unity Methods

    void OnValidate()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        MoveUpdate();
        LookUpdate();
        CameraUpdate();
    }

    #endregion

    #region Controller Methods

    void MoveUpdate()
    {
        // MoveInput.y WS keys; MoveInput.x AD keys
        Vector3 motion = transform.forward * MoveInput.y + transform.right * MoveInput.x;
        motion.y = 0f; // motion needs to be on flat plane
        motion.Normalize();

        if (motion.sqrMagnitude >= 0.01f)
        {
            // set vel to vector that comes from difference of vel to motion vector with a delta of acceleration
            CurrentVel = Vector3.MoveTowards(CurrentVel, motion * MaxSpeed, Acceleration * Time.deltaTime);
        }
        else
        {
            CurrentVel = Vector3.MoveTowards(CurrentVel, Vector3.zero, Acceleration * Time.deltaTime);
        }

        if (IsGrounded && VerticalVelocity <= 0.01f)
        {
            // keep character stuck to the ground
            VerticalVelocity = -3f;
        }
        else
        {
            VerticalVelocity += Physics.gravity.y * GravityScale * Time.deltaTime;
        }

        
        Vector3 fullVelocity = new Vector3(CurrentVel.x, VerticalVelocity, CurrentVel.z);

        characterController.Move(fullVelocity * Time.deltaTime);

        //update speed
        CurrentSpeed = CurrentVel.magnitude;
    }

    void LookUpdate()
    {
        Vector2 input = new Vector2(LookInput.x * LookSensitivity.x, LookInput.y * LookSensitivity.y);
        CurrentPitch -= input.y;
        // for looking up and down: rotate camera up and down with clamp 
        fpCamera.transform.localRotation = Quaternion.Euler(CurrentPitch, 0f, 0f);

        // for looking left and right: actually rotate character model
        transform.Rotate(Vector3.up * input.x);
    }

    void CameraUpdate()
    {
        fpCamera.Lens.FieldOfView = CameraNormalFOV;
    }

    #endregion
}

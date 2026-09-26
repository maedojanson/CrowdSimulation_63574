using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float walkSpeed = 5.0f;
    public float gravity = -9.81f;

    [Header("Controlo de Rato")]
    public Transform playerCamera;
    public float mouseSensitivity = 0.15f;
    public float maxLookAngle = 80.0f;

    [Header("Balanço da Câmara (Head Bobbing)")]
    public float bobFrequency = 10.0f;
    public float bobAmount = 0.05f;
    public float bobSwayAmount = 0.03f;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private Vector3 defaultCameraPos;
    private float bobTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                playerCamera = cam.transform;
            }
        }

        if (playerCamera != null)
        {
            defaultCameraPos = playerCamera.localPosition;
        }
    }

    void Update()
    {
        HandleMouseLook();
        Vector3 moveInput = HandleMovement();
        HandleHeadBob(moveInput);

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked || Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseDelta.x);

        if (playerCamera != null)
        {
            verticalRotation -= mouseDelta.y;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
            playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }

    Vector3 HandleMovement()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
        }

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move.normalized * walkSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        return new Vector3(horizontal, 0, vertical);
    }

    void HandleHeadBob(Vector3 moveInput)
    {
        if (playerCamera == null) return;

        if (moveInput.sqrMagnitude > 0.01f && controller.isGrounded)
        {
            bobTimer += Time.deltaTime * bobFrequency;

            float newY = defaultCameraPos.y + Mathf.Sin(bobTimer) * bobAmount;
            float newX = defaultCameraPos.x + Mathf.Cos(bobTimer * 0.5f) * bobSwayAmount;

            playerCamera.localPosition = new Vector3(newX, newY, defaultCameraPos.z);
        }
        else
        {
            bobTimer = 0f;
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, defaultCameraPos, Time.deltaTime * 6.0f);
        }
    }
}

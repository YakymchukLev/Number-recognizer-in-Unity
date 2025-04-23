using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("PLAYER VARIABLES")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private CharacterController controller;

    [Header("CAMERA VARIABLES")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float bobbingSpeed = 7f;
    [SerializeField] private float bobbingAmount = 0.05f;
    [SerializeField] private float sensitivity = 2f;

    private float bobbingTimer = 0f;
    private float verticalRot = 0f;
    private float moveForward;
    private float moveRight;
    private Vector3 velocity;
    private Vector3 originalCameraPosition;

    private void Start()
    {
        GameStats.isMove = true;
        GameStats.isCam = true;
        Cursor.lockState = CursorLockMode.Locked;
        originalCameraPosition = cameraTransform.localPosition;
    }

    private void Update()
    {
        //Movement
        if(GameStats.isMove)
        {
            moveForward = Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue();
            moveRight = Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue();
        }
        float currentSpeed = speed;
        if (moveForward < 0) // Якщо йде назад, швидкість зменшується
        {
            currentSpeed *= 0.5f;
        }

        Vector3 move = new Vector3(moveRight, 0, moveForward).normalized;
        controller.Move(transform.TransformDirection(move) * currentSpeed * Time.deltaTime);


        //Gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        //Mouse view
        if(GameStats.isCam)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * sensitivity * Time.deltaTime;
        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;

        verticalRot -= mouseY;
        verticalRot = Mathf.Clamp(verticalRot, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRot, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        //Тряска камери
        if (move.magnitude > 0)
        {
            bobbingTimer += Time.deltaTime * bobbingSpeed;
            float bobbingOffset = Mathf.Sin(bobbingTimer) * bobbingAmount;
            cameraTransform.localPosition = originalCameraPosition + new Vector3(0, bobbingOffset, 0);
        }
        else
        {
            bobbingTimer = 0;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition,
                                                            originalCameraPosition, Time.deltaTime * bobbingSpeed);
        }
        }
    }
}

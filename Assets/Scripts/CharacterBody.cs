using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class CharacterBody : MonoBehaviour
{
    [SerializeField] private Transform _headTransform;
    [SerializeField] private float _climb;
    [SerializeField] private float _damping;
    [SerializeField] private float _speed;
    [SerializeField] private float _runMultiplier;
    [SerializeField] private float _gravity;
    [SerializeField] private float _jumpForce;

    private CharacterController _characterController;
    private Vector3 _acceleration;
    private Vector3 _planarVelocity;
    private float _verticalVelocity;

    public Transform HeadTransform => _headTransform;
    public bool IsRunning { get; set; } = false;
    public bool IsJumping { get; set; } = false;
    public bool IsSitting { get; set; } = false;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _planarVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        _planarVelocity *= _damping;
        _planarVelocity += _acceleration * Time.fixedDeltaTime;

        var maxVelocity = _speed * (IsRunning ? _runMultiplier : 1f);
        if (_planarVelocity.magnitude > maxVelocity)
        {
            _planarVelocity = _planarVelocity.normalized * maxVelocity;
        }

        if (IsGrounded())
        {
            _verticalVelocity = IsJumping ? _jumpForce : 0;
        }
        else
        {
            _verticalVelocity += _gravity * Time.fixedDeltaTime *
                UnityEngine.Physics.gravity.y;
        }

        _characterController.Move(_planarVelocity + _verticalVelocity * Vector3.up);
    }

    public void Move(Movement horizontal, Movement vertical)
    {
        _acceleration = (
            (int)vertical * transform.forward +
            (int)horizontal * transform.right).normalized * _climb;
    }

    public void UpdateView(Vector3 rotationDelta)
    {
        rotationDelta.y = Mathf.Clamp(rotationDelta.y, -90, 90);
        _headTransform.localEulerAngles = new Vector3(-rotationDelta.y, 0);
        transform.eulerAngles = new Vector3(0, rotationDelta.x);
    }

    private bool IsGrounded()
    {
        return _characterController.isGrounded;
    }
}

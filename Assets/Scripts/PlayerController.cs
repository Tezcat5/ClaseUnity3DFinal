using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //--------------Componentes--------------------
    private CharacterController _controller;
    private Transform _camera;
    private Animator _animator;

    //--------------Inputs-------------------------
    private float _horizontal;
    private float _vertical;

    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _pushForce = 10;

    private float _turnSmoothVelocity;

    [SerializeField] private float _turnSmoothTime = 0.5f;

    [SerializeField] private float _jumpHeight = 1;

    //------------Cosas Gravedad-------------------
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private Vector3 _playerGravity;

    //------------Cosas GroundSensor---------------
    [SerializeField] Transform _sensorPosition;
    [SerializeField] float _sensorRadius = 0.5f;
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] private bool _isGrounded;

    private bool _hasJumped = false;

    private Vector3 moveDirection;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main.transform;
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();
        }

        if (Input.GetButton("Fire2"))
        {
            AimMovement();
        }
        else
        {
            Movement();
        }

        Gravity();

        if (Input.GetKeyDown(KeyCode.F))
        {
            RayTest();
        }
    }

    void AimMovement()
    {
        Vector3 direction = new Vector3(_horizontal, 0, _vertical);

        _animator.SetFloat("VelZ", _vertical);
        _animator.SetFloat("VelX", _horizontal);

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _camera.eulerAngles.y, ref _turnSmoothVelocity, _turnSmoothTime);

        transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

        if (direction != Vector3.zero)
        {
            moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _controller.Move(moveDirection * _movementSpeed * Time.deltaTime);
        }
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_horizontal, 0, _vertical);

        _animator.SetFloat("VelZ", direction.magnitude);
        _animator.SetFloat("VelX", 0);

        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;

            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _controller.Move(moveDirection * _movementSpeed * Time.deltaTime);
        }
    }

    void Gravity()
    {
        if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if (IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = -1;

            _animator.SetBool("IsJumping", false);
            _hasJumped = false;
        }

        _controller.Move(_playerGravity * Time.deltaTime);
    }

    void Jump()
    {
        _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);

        if (!_hasJumped)
        {
            _animator.SetBool("IsJumping", true);
            _hasJumped = true;
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(_sensorPosition.position, -transform.up, out hit, 2))
        {
            if (hit.transform.gameObject.layer == 6)
            {
                Debug.DrawRay(_sensorPosition.position, -transform.up * 2, Color.green);
                return true;
            }
            else
            {
                Debug.DrawRay(_sensorPosition.position, -transform.up * 2, Color.red);
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void RayTest()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10))
        {
            if (hit.transform.gameObject.tag == "Enemy")
            {
                Enemy enemyScript = hit.transform.gameObject.GetComponent<Enemy>();
                enemyScript.TakeDamage();
            }
        }
    }

    void OnTriggerEnter(Collider other)
{
    // Comprueba si el objeto tocado tiene el tag "Enemy"
    if (other.CompareTag("Enemy"))
    {
        // Reproduce la animación de muerte
        _animator.SetTrigger("IsDead");

        // Inicia la corrutina para destruir el personaje tras un pequeño retraso
        StartCoroutine(DelayedDestroy());
    }
}

IEnumerator DelayedDestroy()
{
    // Espera unos segundos para que la animación de muerte se complete
    yield return new WaitForSeconds(2); // Ajusta el tiempo según la duración de tu animación

    // Destruye al jugador
    Destroy(gameObject);
}
}
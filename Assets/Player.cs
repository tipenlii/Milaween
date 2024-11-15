using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Animator animator;
    public float moveSpeed = 5f;        
    public float runSpeed = 10f;        
    public float lookSpeed = 2f;        
    public float jumpHeight = 2.5f;     
    public float dashSpeed = 20f;       
    public float dashDuration = 0.2f;   
    public Transform playerCamera;      
    private float xRotation = 0f;       

    private bool isGrounded;            
    private bool isRunning = false;     
    private bool isDashing = false;     
    private float dashTime;             
    private float lastTapTime = 0f;     

    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        LookAround();
        Jump();
    }

    void MovePlayer()
{
    // Cek apakah tombol W ditekan dua kali dalam waktu singkat untuk berlari
    if (Input.GetKeyDown(KeyCode.W))
    {
        if (Time.time - lastTapTime < 0.3f)
        {
            animator.SetBool("isRunning", true);
        }
        lastTapTime = Time.time;
    }

    if (Input.GetKeyUp(KeyCode.W))
    {
        // Only stop running when no movement input is present and the player is not dashing
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && !isDashing)
        {
            animator.SetBool("isRunning", false);
        }
    }

    // Dapatkan kecepatan berdasarkan apakah karakter sedang berlari atau berjalan
    float speed = isRunning ? runSpeed : moveSpeed;
    float moveX = Input.GetAxis("Horizontal");
    float moveZ = Input.GetAxis("Vertical");

    // Hitung arah gerakan
    Vector3 move = transform.right * moveX + transform.forward * moveZ;

    // Jika ada input gerakan, set isWalking ke true, jika tidak, set ke false
    if (move.magnitude > 0 && !isRunning)
    {
        animator.SetBool("isWalking", true);
    }
    else
    {
        animator.SetBool("isWalking", false);
    }

    // Pindahkan karakter jika tidak sedang dash
    if (!isDashing)
    {
        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }

    // Mulai dash jika tombol Left Shift ditekan
    if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
    {
        StartCoroutine(Dash(move.normalized));
    }
}


    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        transform.Rotate(Vector3.up * mouseX);
    }

    void Jump()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
    }

    IEnumerator Dash(Vector3 dashDirection)
    {
        isDashing = true;
        float startTime = Time.time;
        
        while (Time.time < startTime + dashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }
        
        isDashing = false;
    }
}

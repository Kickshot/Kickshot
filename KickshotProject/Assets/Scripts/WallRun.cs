using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{

    [SerializeField] private float m_RayDistance;
    [SerializeField] private GameObject m_RotationObject;

    private bool m_WallRunning;
    private bool m_Right;
    private bool m_WallJumping;

    private Vector3 m_WallNormal;
    private Vector3 m_Direction;
    private float m_CurrentWallJumpTime;
    private float m_Speed;

    private UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController m_FPC;
    private PlayerController m_PlayerController;


    // Use this for initialization
    void Start()
    {
        m_WallJumping = false;
        m_WallRunning = false;
        m_FPC = GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController>();
        m_PlayerController = GetComponent<PlayerController>();
    }

    void SnapToWall(bool right, RaycastHit hitInfo)
    {

        float angle = 90;
        m_Right = right;

        if (!right)
            angle = -90;

        //m_FPS.m_GravityMultiplier = m_Gravity;
        // m_FPS.m_wallRunning = true;
        m_FPC.m_RigidBody.useGravity = false;
        m_FPC.m_RigidBody.velocity = new Vector3(0, 0, 0);
        m_FPC.m_WallRunning = true;
        Vector3 wallNormal = hitInfo.normal;
        m_WallNormal = wallNormal;
        Vector3 rotatedVector = Quaternion.Euler(0, angle, 0) * wallNormal;
        Quaternion newRotaton = Quaternion.LookRotation(rotatedVector);
        m_Direction = rotatedVector;
        m_RotationObject.transform.rotation = newRotaton;
        this.transform.position = rotatedVector * (Time.deltaTime * m_Speed) + this.transform.position;
  
        m_WallRunning = true;
        m_PlayerController.WallRun = true;
    }

    // Update is called once per frame
    void Update()
    {

        m_RotationObject.transform.position = this.transform.position;

        
        RaycastHit rightRayHitInfo;
        RaycastHit leftRayHitInfo;
        RaycastHit frontRayHitInfo;

        if(!m_WallRunning)
        {
            Vector3 playerVel = m_FPC.m_RigidBody.velocity;
            m_Speed = playerVel.magnitude;

        }


        if (!m_PlayerController.freeze)
        {
            if (!m_FPC.Grounded)
            {


                if (Physics.Raycast(m_RotationObject.transform.position, m_RotationObject.transform.right, out rightRayHitInfo, m_RayDistance))
                {
                    if (!Physics.Raycast(m_RotationObject.transform.position, m_RotationObject.transform.forward, out frontRayHitInfo, m_RayDistance))
                    {
                        SnapToWall(true, rightRayHitInfo);
                    }
                    else
                    {
                        SnapToWall(true, frontRayHitInfo);
                    }

                }
                else if (Physics.Raycast(m_RotationObject.transform.position, -m_RotationObject.transform.right, out leftRayHitInfo, m_RayDistance))
                {
                    if (!Physics.Raycast(m_RotationObject.transform.position, m_RotationObject.transform.forward, out frontRayHitInfo, m_RayDistance))
                    {
                        SnapToWall(false, leftRayHitInfo);
                    }
                    else
                    {
                        SnapToWall(false, frontRayHitInfo);
                    }
                }
                else
                {
                    // m_FPS.m_GravityMultiplier = 2;
                    m_RotationObject.transform.rotation = this.transform.rotation;
                    m_FPC.m_RigidBody.useGravity = true;
                    m_WallRunning = false;
                    m_PlayerController.WallRun = false;
                    m_FPC.m_WallRunning = false;
                }
            }
            else
            {
                m_RotationObject.transform.rotation = this.transform.rotation;
                m_WallRunning = false;
                m_PlayerController.WallRun = false;
            }
        }
        

        /******************************* TEAM NEEDS TO TALK ABOUT WHAT KEYS ARE WHAT ****************************/
        // TODO CHANGE INPUT KEY
        if (Input.GetButtonDown("Jump") && m_WallRunning == true && m_WallJumping == false)
        {
            Vector3 direction = m_WallNormal + m_Direction;
            m_FPC.m_RigidBody.AddForce(direction * 2500);
        }

        if (Input.GetButtonUp("Jump") && m_WallRunning == false)
            m_WallJumping = true;

        if (Input.GetButtonUp("Jump"))
            m_WallJumping = false;





    }
}

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
    private bool bDirectionOfWallRunRight;
    private bool bDirectionOfWallRunLeft;

    private UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController m_FPC;
    private PlayerController m_PlayerController;


    // Use this for initialization
    void Start()
    {
        m_WallJumping = false;
        m_WallRunning = false;
        bDirectionOfWallRunRight = false;
        bDirectionOfWallRunLeft = false;
        m_FPC = GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController>();
        m_PlayerController = GetComponent<PlayerController>();
    }

    void SnapToWall(bool right,bool rightDirection, RaycastHit hitInfo)
    {
        SendMessageUpwards("Reload");

        float angle = 90;
        float rotationAngle = 90;

        m_Right = right;

        if (!right)
            angle = -90;

        if (!rightDirection)
            rotationAngle = -90;

        m_FPC.m_RigidBody.useGravity = false;
        m_FPC.m_RigidBody.velocity = new Vector3(0, 0, 0);
        m_FPC.m_WallRunning = true;

        Vector3 wallNormal = hitInfo.normal;
        m_WallNormal = wallNormal;
        Vector3 rotatedVector = Quaternion.Euler(0, angle, 0) * wallNormal;
        Quaternion newRotaton = Quaternion.LookRotation(rotatedVector);
        m_Direction = rotatedVector;

        Vector3 rotatedVectorDirection = Quaternion.Euler(0, rotationAngle, 0) * wallNormal;
        Quaternion newRotatonDirection = Quaternion.LookRotation(rotatedVectorDirection);
        m_RotationObject.transform.rotation = newRotatonDirection;

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

            Vector3 velocity = m_FPC.m_RigidBody.velocity.normalized;
            float momentumAngleRight = Vector3.SignedAngle(this.transform.right, velocity, Vector3.up);
            float momentumAngleLeft = Vector3.SignedAngle(-this.transform.right, velocity, Vector3.up);
            //Debug.Log(momentumAngleLeft);

            bDirectionOfWallRunRight = (momentumAngleRight < 0);
            bDirectionOfWallRunLeft = (momentumAngleLeft < 0);

        }

        if (!m_PlayerController.Freeze)
        {
            if (!m_FPC.Grounded)
            {

                if (Physics.Raycast(m_RotationObject.transform.position, m_RotationObject.transform.right, out rightRayHitInfo, m_RayDistance))
                {
                    //TODO Get rid of this quick fix.
                    if (rightRayHitInfo.collider.gameObject.tag != "finish")
                    {

                        if (!Physics.Raycast(m_RotationObject.transform.position, m_RotationObject.transform.forward, out frontRayHitInfo, m_RayDistance))
                            SnapToWall(bDirectionOfWallRunRight, true, rightRayHitInfo);
                        else
                            SnapToWall(bDirectionOfWallRunRight, true, frontRayHitInfo);
                    }

                }
                else if (Physics.Raycast(m_RotationObject.transform.position, -m_RotationObject.transform.right, out leftRayHitInfo, m_RayDistance))
                {
                    //TODO Get rid of this quick fix.
                    if (rightRayHitInfo.collider.gameObject.tag != "finish")
                    {
                        if (!Physics.Raycast(m_RotationObject.transform.position, m_RotationObject.transform.forward, out frontRayHitInfo, m_RayDistance))
                            SnapToWall(bDirectionOfWallRunLeft, false, leftRayHitInfo);
                        else
                            SnapToWall(bDirectionOfWallRunLeft, false, frontRayHitInfo);
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

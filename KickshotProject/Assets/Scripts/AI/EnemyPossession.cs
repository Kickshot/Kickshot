using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent( typeof(SourcePlayer) )]
[RequireComponent( typeof(MouseLook) )]
public class EnemyPossession : MonoBehaviour {
    private SourcePlayer player;
    private MouseLook look;
    private float updateTimer;
    private Vector3 destination;
    private NavMeshPath path;
    int node = 0;
    void Start () {
        path = new NavMeshPath ();
        player = GetComponent<SourcePlayer> ();
        look = GetComponent<MouseLook> ();
        look.useDeltas = false;
    }
    void LateUpdate () {
        updateTimer += Time.deltaTime;
        if (updateTimer > .3f) {
            updateTimer = 0;

            SourcePlayer p = GameManager.instance.Player;
            if (p == null) {
                return;
            } else {
                destination = p.transform.position - new Vector3 (0, 1, 0);
                NavMesh.CalculatePath (transform.position - new Vector3 (0, 1, 0), p.transform.position - new Vector3 (0, 1, 0), NavMesh.AllAreas, path);
                node = 0;
            }
        }
        if (path == null || path.corners.Length <= 0) {
            player.wishDir = new Vector3 (0, 0, 0);
            return;
        }
        Vector3 targetPos;
        if (node >= path.corners.Length) {
            targetPos = destination;
        } else {
            targetPos = path.corners [node];
        }
        Vector3 pos = transform.position - new Vector3 (0, 1, 0);
        //Helper.DrawLine (pos, targetPos, Color.red, 0.2f);
        Vector3 dir = (targetPos - pos);
        if (dir.magnitude < 0.5f) {
            //transform.position = targetPos + new Vector3 (0, 1, 0);
            node++;
            return;
        }
        dir.y = 0;
        if (player.groundEntity != null && player.frictionStun <= 0f) {
            player.wishDir = new Vector3 (0, 0, 1);//(Quaternion.Inverse(rot)*dir).normalized;
        } else {
            player.wishDir = new Vector3 (0, 0, 0);
        }
        look.wishDir = Quaternion.LookRotation ((targetPos - transform.position).normalized);
        //Vector3 dest = agent.path.corners [0];
        //Vector3 dir = (dest - transform.position).normalized;
        //dir.y = 0;
        //player.wishDir = dir;
    }
}

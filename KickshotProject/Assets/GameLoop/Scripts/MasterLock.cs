using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class MasterLock : MonoBehaviour {
    
    private List<ChildLock> lockedChildren = new List<ChildLock>();
    private Vector3 lastPos;
    private bool locked = true;

    private void Start()
    {
        lastPos = transform.position;
        //Populate children at the start
        Object[] lc = FindObjectsOfType(typeof(ChildLock));
        foreach (Object o in lc) {
            lockedChildren.Add(o as ChildLock);
        }
        LockChildren();
    }

    private void Update()
    {
        if (EditorApplication.isPlaying) return;
        lockedChildren = new List<ChildLock>();
        Object[] lc = FindObjectsOfType(typeof(ChildLock));
        foreach (Object o in lc)
        {
            lockedChildren.Add(o as ChildLock);
        }

        if (lastPos != transform.position && !locked) {
            LockChildren();
            locked = true;
        } else if (locked){
            UnlockChildren();
            locked = false;
        }
        lastPos = transform.position;
    }

    private void LockChildren() {
        Debug.Log("Locking Children");
        foreach (ChildLock l in lockedChildren) {
            l.enabled = true;
        }
    }

    private void UnlockChildren() {
        Debug.Log("Unlocking Children");
        foreach (ChildLock l in lockedChildren)
        {
            l.enabled = false;
        }
    }
}
#endif
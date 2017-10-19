using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class meant to hold basic types to restore parameters to objects. Should be easy to serialize so don't put any bloated unity objects in here!
public class SaveData {
    // Save object's current state, and do nothing else.
    public virtual void Save (GameObject obj) {}
    // Assume destruction of original object (and its children), so create a new prefab from save data.
    public virtual GameObject Load () {
        return null;
    }
}

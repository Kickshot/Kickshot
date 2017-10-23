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
    // Saved Data must be writable to a json file.
    public virtual string Serialize(){
        return JsonUtility.ToJson(this);
    }
    // Save Data must also be readable from a file. MUST BE FILLED OUT BY CHILD CLASSES *ABSTRACT*
    public virtual object Deserialize(string json){
        return null;
    }
}

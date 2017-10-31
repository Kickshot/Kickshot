using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ImpactSoundGenerator : GunBase {
    public List<string> materialTypes;
    public string filepath = "Assets/Resources/test.txt";

    private Dictionary<string,string> memory;
    private int currentType;
    void Start() {
        memory = new Dictionary<string,string> ();
        if (materialTypes.Count < 0) {
            materialTypes.Add ("dirt");
        }
    }
    public override void OnEquip(GameObject Player) {
        base.OnEquip (Player);
        Debug.Log ("Hello! M1 = tag surface, M2 = switch tag, R = write to file.");
    }
    public override void OnGUI () {
        if (!equipped) {
            return;
        }
        base.OnGUI ();
        GUIStyle style = GUIStyle.none;
        style.normal.textColor = Color.black;
        style.fontSize = 24;
        GUI.Label (new Rect (51f, Screen.height-50f, 100, 50), materialTypes[currentType], style);
        style.normal.textColor = Color.red;
        GUI.Label (new Rect (50f, Screen.height-50f, 100, 50), materialTypes[currentType], style);
    }
    public override void Update() {
        base.Update();
        if (!equipped) {
            return;
        }
        transform.rotation = view.rotation;
    }
    public override void OnReload() {
        StreamWriter writer = new StreamWriter (filepath, true);
        writer.WriteLine ( "// Sounds generated for " + SceneManager.GetActiveScene ().name );
        foreach (KeyValuePair<string, string> v in memory) {
            writer.WriteLine ("soundLookup[\"" + v.Key + "\"] = \"" + v.Value + "\";");
        }
        writer.Close ();
        Debug.Log ("Wrote `" + filepath + "` !");
    }
    public override void OnPrimaryFire() {
        RaycastHit hit;
        if (Physics.Raycast (view.position, view.forward, out hit, 1000f, ~((1 << LayerMask.NameToLayer ("Player")) | (1 << LayerMask.NameToLayer ("Ignore Raycast"))), QueryTriggerInteraction.Ignore)) {
            Material mat = Helper.getMaterial (hit);
            if (!mat) {
                Debug.LogError ("Failed to find material! Possible there's an invisible collider that isn't set to ignore raycasts...");
                return;
            }
            if (memory.ContainsKey (mat.name)) {
                Debug.Log ("Re-marked " + mat.name + " as a " + materialTypes [currentType] + " material.");
            } else {
                Debug.Log ("Marked " + mat.name + " as a " + materialTypes [currentType] + " material.");
            }
            memory [mat.name] = materialTypes [currentType];
        }
    }
    public override void OnSecondaryFire() {
        currentType = (currentType + 1) % materialTypes.Count;
    }
}

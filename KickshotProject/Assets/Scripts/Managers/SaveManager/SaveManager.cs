﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveManager
{
    private static List<SaveData> data;
    // Saves the state of everything that matters, and stores it inside data.
    public static void Save() {
        data = new List<SaveData> ();
        foreach( SourcePlayer player in UnityEngine.Object.FindObjectsOfType<SourcePlayer>() ) {
            data.Add (new SourcePlayerSaveData (player.gameObject));
        }
        foreach( GunBase gun in UnityEngine.Object.FindObjectsOfType<GunBase>() )
        {
            if (gun.player == null) { // We don't want to save guns that a player is carrying, they're already saved by SourcePlayerSaveData
                data.Add (new GunSaveData (gun.gameObject));
            }
        }
    }

    // Destroys everything in the world (that has save data), and recreates them with saved parameters.
    public static void Load()
    {
        foreach( SourcePlayer player in UnityEngine.Object.FindObjectsOfType<SourcePlayer>() ) {
            GameObject.Destroy (player.gameObject);
        }
        foreach( GunBase gun in UnityEngine.Object.FindObjectsOfType<GunBase>() ) {
            GameObject.Destroy (gun.gameObject);
        }
        foreach (SaveData sd in data) {
            sd.Load ();
        }
    }

    public static void WriteState(string name = null, string path = null)
    {
        path = string.IsNullOrEmpty(path) ? @"\Assets\" : path;
        name = string.IsNullOrEmpty(name) ? "Save1.json" : name;

        if (data.Count == 0)
        {
            Save();
        }

        Dictionary<string,string> jsonData = new Dictionary<string, string>();
        foreach (SaveData sd in data)
        {
            if(sd is GunSaveData)
            {
                jsonData.Add( ((GunSaveData)sd).Serialize(), typeof(GunSaveData).ToString());
            }
            if (sd is SourcePlayerSaveData)
            {
                jsonData.Add(((SourcePlayerSaveData)sd).Serialize(), typeof(SourcePlayerSaveData).ToString());
            }
        }

        path = System.Environment.CurrentDirectory + path;

        File.Delete(path + name);

        using (StreamWriter writer = new StreamWriter(File.Open(path + name, FileMode.CreateNew)))
        {
            foreach (KeyValuePair<string,string> position in jsonData)
            {
                writer.WriteLine(position.Value);
                writer.WriteLine(position.Key);
            }
        }

        MonoBehaviour.print(name + " Written!");
        return;
    }
}
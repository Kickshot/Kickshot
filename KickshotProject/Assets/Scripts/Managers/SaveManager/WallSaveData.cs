using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallSaveData : SaveData
{
    public GameObject ShatteredWallObj;
    public float CenterToObjVelScale;
    public float ForwardVelScale;
    public Vector3 location;
    public Quaternion Rotation;

    public WallSaveData(GameObject obj)
    {
        Save(obj);
    }

    public override void Save(GameObject obj)
    {
        location = obj.transform.position;
        Rotation = obj.transform.rotation;

        ShatterWall wall = obj.GetComponent<ShatterWall>();

        ShatteredWallObj = wall.ShatteredWallObj;
        CenterToObjVelScale = wall.CenterToObjVelScale;
        ForwardVelScale = wall.ForwardVelScale;
        
    }

    public override GameObject Load()
    {
        GameObject obj = GameObject.Instantiate(ResourceManager.GetResource<GameObject>("ShatterWall"));
        obj.transform.position = location;
        obj.transform.rotation = Rotation;

        ShatterWall g = obj.GetComponent<ShatterWall>();
        g.ShatteredWallObj = ShatteredWallObj;
        g.CenterToObjVelScale = CenterToObjVelScale;
        g.ForwardVelScale = ForwardVelScale;

        return obj;

    }

    public override object Deserialize(string json)
    {
        return JsonUtility.FromJson<WallSaveData>(json);
    }

    public override string Serialize()
    {
        return base.Serialize();
    }
}

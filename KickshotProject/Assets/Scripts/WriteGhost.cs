using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WriteGhost : MonoBehaviour
{
    public KeyCode WriteKey = KeyCode.Backspace;
    public KeyCode CutKey = KeyCode.Backslash;
    public string ReplayName = "TestGhost";
    public string ReplayPath = @"\Assets\";

    List<Vector3> m_data;
    bool m_recording;
     

	// Use this for initialization
	void Start ()
    {
        m_data = new List<Vector3>();

        m_recording = true;
	}
	
	void FixedUpdate ()
    {
        if(m_recording)
        {
            m_data.Add(transform.position);
        }
	}

    void Update()
    {
        if(Input.GetKeyDown(WriteKey))
        {
            WriteReplay(ReplayName, ReplayPath);
        }
        if (Input.GetKeyDown(CutKey))
        {
            m_recording = false;
        }
    }

    void WriteReplay(string name, string path)
    {
        path = System.Environment.CurrentDirectory + path;

        File.Delete(path + name);

        using (StreamWriter writer = new StreamWriter(File.Open(path + name, FileMode.CreateNew)))
        {
            foreach (Vector3 position in m_data)
            {
                writer.WriteLine(position.x + " " + position.y + " " + position.z);
            }
        }

        print(name + " Written!");
        return;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadGhost : MonoBehaviour
{
    public KeyCode StartKey = KeyCode.UpArrow;
    public KeyCode StopKey = KeyCode.DownArrow;
    public KeyCode PreviousFrame = KeyCode.LeftArrow;
    public KeyCode AdvanceFrame = KeyCode.RightArrow;

    public string ReplayName = "TestGhost";
    public string ReplayPath = @"\Assets\";

    public bool   LoopReplay;

    List<Vector3> m_data;
    bool m_playing;
    int currentKeyFrame;

    // Use this for initialization
    void Start ()
    {
        m_data = ReadReplay(ReplayName, ReplayPath);

        currentKeyFrame = 0;
        m_playing = true;
	}
	
	// Update is called once per frame
	void FixedUpdate()
    {
        if(currentKeyFrame > m_data.Count - 1)
        {
            if(LoopReplay)
            {
                currentKeyFrame = 0;
            }
        }
        else
        {
            if(m_playing)
            {
                gameObject.transform.position = m_data[currentKeyFrame++];
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(StartKey))
        {
            m_playing = true;
        }
        else if (Input.GetKeyDown(StopKey))
        {
            m_playing = false;
        }
        else if (Input.GetKey(PreviousFrame))
        {
            m_playing = false;
            gameObject.transform.position = m_data[--currentKeyFrame];
        }
        else if (Input.GetKey(AdvanceFrame))
        {
            m_playing = false;
            gameObject.transform.position = m_data[++currentKeyFrame];
        }
    }

    static List<Vector3> ReadReplay(string name, string path)
    {
        path = System.Environment.CurrentDirectory + path;

        List<Vector3> replay = new List<Vector3>(); 
        string line = string.Empty;

        using (StreamReader reader = new StreamReader(File.Open(path + name, FileMode.Open)))
        {
            // Read BPM
            while ((line = reader.ReadLine()) != null)
            {
                string[] position = line.Split(' ');

                replay.Add(new Vector3(float.Parse(position[0]), float.Parse(position[1]), float.Parse(position[2]) ) );
            }
        }

        return replay;
    }
}

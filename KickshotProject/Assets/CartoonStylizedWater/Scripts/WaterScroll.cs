using UnityEngine;
using System.Collections;

public class WaterScroll : MonoBehaviour {

    public float scrollSpeed = 0.5F;
    public float scrollSpeedY = 0.5F;
    public float scrollSpeed2 = 0.5F;
    public float scrollSpeedY2 = 0.5F;
    public bool Tiling = false;

    public float amplitudeY = 5.0f;


     float index=0;


    public float amplitudeY2 = 5.0f;
    public float max = 0.5f;



    public Renderer rend;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        float offset = Time.time * scrollSpeed;
        float offset2 = Time.time * scrollSpeedY;
        float offset3 = Time.time * scrollSpeed2;
        float offset4 = Time.time * scrollSpeedY2;
        if(rend.material.HasProperty("_MainTex"))
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, offset2));
        if(rend.material.HasProperty("_MainTex2"))
            rend.material.SetTextureOffset("_MainTex2", new Vector2(offset3, offset4));


        if(Tiling)
        {
        index += Time.deltaTime;
  
            float y = 0.47f+max + (Mathf.Abs( Mathf.Sin (index*amplitudeY))/1)*(0.47f-max);
        rend.material.SetTextureScale("_MainTex", new Vector2(2, y));

  
            y = 0.47f+max + (Mathf.Abs( Mathf.Sin (index*amplitudeY2))/1)*(0.47f-max);
        rend.material.SetTextureScale("_MainTex2", new Vector2(2, y));
        }
	}
}

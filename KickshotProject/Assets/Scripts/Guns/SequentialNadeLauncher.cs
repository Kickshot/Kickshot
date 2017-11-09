using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialNadeLauncher : GunBase
{
    public int MaxNadesOut = 5;
    public bool ReverseOrder = true;
    public float Knockback = 1f;
    public float Radius = 5f;
    public GameObject Explosion;

    List<GameObject> nades;

	// Use this for initialization
	void Start ()
    {
        nades = new List<GameObject>(MaxNadesOut);
	}

    public override void OnPrimaryFire()
    {
        RaycastHit hit;
        if (Physics.Raycast(view.position, view.forward, out hit, 1000f, Helper.GetHitScanLayerMask()))
        {
           GameObject newNade = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), hit.point, Quaternion.identity);

           nades.Insert(0, newNade);

           
           if(nades.Count >= MaxNadesOut)
           {
                if (ReverseOrder)
                {
                    Destroy(nades[nades.Count - 1]);
                    nades.RemoveAt(nades.Count - 1);
                }
                else
                {
                    Destroy(nades[0]);
                    nades.RemoveAt(0);
                }
           }
        }
    }

    public override void OnSecondaryFire()
    {
        ammo++;
        if (nades.Count > 0)
        {
            if (ReverseOrder)
            {
                GameObject exp = Instantiate(Explosion, nades[0].transform.position, Quaternion.identity);
                Destroy(nades[0]);
                GameRules.RadiusDamage(100f, Knockback, nades[0].transform.position, Radius, true);

                nades.RemoveAt(0);
            }
            else
            {
                GameObject exp = Instantiate(Explosion, nades[nades.Count - 1].transform.position, Quaternion.identity);
                Destroy(nades[nades.Count - 1]);
                GameRules.RadiusDamage(100f, Knockback, nades[nades.Count - 1].transform.position, Radius, true);

                nades.RemoveAt(nades.Count - 1);
            }
        }
    }
}

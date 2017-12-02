using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialNadeLauncher : GunBase
{
	public GameObject mine;
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
           GameObject newNade = Instantiate(mine, hit.point, Quaternion.identity);

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
			float minDist = float.MaxValue;
			GameObject closestNade = null;

			foreach (var nade in nades)
			{
				if(Vector3.Distance(player.transform.position, nade.transform.position) < minDist)
				{
					minDist = Vector3.Distance(player.transform.position, nade.transform.position);
					closestNade = nade;
				}
			}

			GameObject exp = Instantiate(Explosion, closestNade.transform.position, Quaternion.identity);

			GameRules.RadiusDamage(100f, Knockback, closestNade.transform.position, Radius, true ,player.gameObject);
			nades.Remove(closestNade);

			foreach (Collider col in Physics.OverlapSphere(closestNade.transform.position, Radius, ~0 , QueryTriggerInteraction.Collide)) 
			{
				print (col.gameObject.name);
				if (col.gameObject.tag == "Combustible")
				{
					exp = Instantiate(Explosion, col.gameObject.transform.position, Quaternion.identity);
					Destroy(col.gameObject);
					GameRules.RadiusDamage(100f, Knockback, col.gameObject.transform.position, Radius, true ,player.gameObject);
					nades.Remove(col.gameObject);
				}
			}
			Destroy(closestNade);


//            if (ReverseOrder)
//            {
//                GameObject exp = Instantiate(Explosion, nades[0].transform.position, Quaternion.identity);
//                Destroy(nades[0]);
//				  GameRules.RadiusDamage(100f, Knockback, nades[0].transform.position, Radius, true,player.gameObject);
//
//                nades.RemoveAt(0);
//            }
//            else
//            {
//                GameObject exp = Instantiate(Explosion, nades[nades.Count - 1].transform.position, Quaternion.identity);
//                Destroy(nades[nades.Count - 1]);
//				print (player.gameObject.name);
//				GameRules.RadiusDamage(100f, Knockback, nades[nades.Count - 1].transform.position, Radius, true, player.gameObject);
//
//                nades.RemoveAt(nades.Count - 1);
//            }
        }
    }
}

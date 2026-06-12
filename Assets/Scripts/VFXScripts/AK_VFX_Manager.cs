using UnityEngine;

public class AK_VFX_Manager : MonoBehaviour
{
    public GameObject SlashVFX;

    //public GameObject Half_L;
    //public GameObject Half_R;

    //[Header("Slash Half VFX parameter")]
    //public float angularForce;
    //public float blowForceX;
    //public float blowForceY;
    //public float flyDirection;


    public void SpawnSlashVFX(Vector2 position, float direction)
    {
        Instantiate(SlashVFX, position, Quaternion.Euler(0, 0, direction));
    }
    public void SpawnSlashVFX(Vector2 position)
    {
        Instantiate(SlashVFX, position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
    }

    /*
    public void SpawnSlashHalfVFX(GameObject slashObj, Vector2 slashPosition,  float slashDirection)
    {
        GameObject objectL = Instantiate(Half_L, slashPosition, Quaternion.Euler(0,0, slashDirection));
        GameObject belongObject_L = Instantiate(slashObj, Vector2.zero, Quaternion.Euler(0, 0, -slashDirection));
        belongObject_L.transform.SetParent(objectL.transform);
        objectL.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-flyDirection, flyDirection));
        objectL.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-blowForceX, blowForceX /8), Random.Range(blowForceY / 4, blowForceY)));


        GameObject objectR = Instantiate(Half_R, slashPosition, Quaternion.Euler(0, 0, slashDirection));
        GameObject belongObject_R = Instantiate(slashObj, Vector2.zero, Quaternion.Euler(0, 0, -slashDirection));
        belongObject_R.transform.SetParent(objectR.transform);
        objectR.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-flyDirection, flyDirection));
        objectR.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-blowForceX, blowForceX / 8), Random.Range(blowForceY / 4, blowForceY)));
    }
    */



    public void FixedUpdate()
    {
        //SpawnSlashVFX(new Vector2(0, 0), Random.Range(0, 360));
    }
}

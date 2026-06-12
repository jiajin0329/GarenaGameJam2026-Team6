using UnityEngine;
using UnityEngine.UI;

public class Kill_Direction : MonoBehaviour
{
    public SpriteRenderer myRGB_VFX_Image;
    public SpriteRenderer mainSlashEffect;
    public SpriteRenderer slashEffectLine;

    public float hueSpeed = 0.7f;

    public float h = 0;
    public float s = 1;
    public float v = 1;

    public float direction;

    public bool isSmallMovement = true;
    public float smallMovementSpeed = 0.05f;

    public bool isBlack = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        h = Random.Range(0f,1f);

        //Color.RGBToHSV(myRGB_VFX_Image.color, out h, out s, out v);
    }

    // Update is called once per frame
    void Update()
    {
        h += hueSpeed * Time.deltaTime;

        h = h > 1f ? 0f : h;

        myRGB_VFX_Image.color = Color.HSVToRGB(h, s, v);
    
        if (isSmallMovement)
        {
            transform.position += new Vector3(1 * Mathf.Deg2Rad * Mathf.Cos(direction) , 1 * Mathf.Deg2Rad * Mathf.Sin(direction), 0) * Time.deltaTime * smallMovementSpeed;
        }

        if (isBlack)
        {
            mainSlashEffect.color = Color.black;
            slashEffectLine.color = Color.black;
        }
        else
        {
            mainSlashEffect.color = Color.white;
            slashEffectLine.color = Color.white;
        }
    }

}

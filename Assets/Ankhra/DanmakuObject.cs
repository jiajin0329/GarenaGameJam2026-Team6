using UnityEngine;

public class DanmakuObject : MonoBehaviour
{
    public Rigidbody2D myRb2d;

    public float speed = 6f;
    public void DanmakuMove()
    {
        myRb2d.linearVelocity = new Vector2(-1 * speed, 0 );
    }

    private void FixedUpdate()
    {
        DanmakuMove();
    }

}

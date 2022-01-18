using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public GameObject Following;

    public float cameraSpeed = 1;

    private Vector3 startheight;
    // Start is called before the first frame update
    [SerializeField] private float zOffset = -13.46f;
    void Start()
    {
        startheight = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Following != null)
        {
            Vector3 followingPos = Following.transform.position;
            followingPos.y = startheight.y;
            followingPos.z += zOffset;
            transform.position = followingPos;
        }
        if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            gameObject.transform.Translate(
                new Vector3(Input.GetAxisRaw("Horizontal")* cameraSpeed, 0, Input.GetAxisRaw("Vertical")* cameraSpeed ), Space.World
            );
            
            Following = null;
        }
        
        
    }
}

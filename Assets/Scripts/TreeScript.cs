using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : ResourceScript
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public override void OnCollected()
    {
        Debug.Log("Treeshouldbe gon");
        Destroy(this.gameObject);
    }
}

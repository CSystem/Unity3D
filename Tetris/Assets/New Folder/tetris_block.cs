using UnityEngine;
using System.Collections;

public class tetris_block : MonoBehaviour {

    public static GameObject block;
    


	// Use this for initialization
	void Start () {

        GameObject.Find("Wall/floor").renderer.material.color = Color.white;
        GameObject.Find("Wall/rwall").renderer.material.color = Color.white;
        GameObject.Find("Wall/lwall").renderer.material.color = Color.white;
        Object[] block_obj = Resources.LoadAll("prefab/");


        // block = (GameObject)block_obj;
        block = (GameObject)GameObject.Instantiate(Resources.Load("prefab/block5"));

         
        block.transform.position = new Vector3(0,2,3);


    
	}
	
	// Update is called once per frame
	void Update () {
	



	}
}

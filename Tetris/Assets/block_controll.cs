using UnityEngine;
using System.Collections;

public class block_controll : MonoBehaviour {

    public string[] block;


    private bool[,] blockMatrix;
    private float fallSpeed;
    private float xPosition;
    private float yPosition;
    private int size;
    private int halfSize;
    private float halfSizeFloat;
    private bool dropped = false;



	// Use this for initialization
	void Start () {
	
        //Sanity checking
        size = block.Length;

        var width = block[0].Length;




        halfSize = size / 2;
        halfSizeFloat = (float)(size * .5);

        blockMatrix = new bool[size, size];

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                if (block[y][x] == "1"[0]) {
                    blockMatrix[x, y] = true;
                    //var block = Instantiate();

                
                }
                
            
            }
        
        
        }

        
        

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

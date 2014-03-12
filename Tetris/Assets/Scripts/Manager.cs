using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {
	
	public GameObject[] blocks;
	public Transform cube;
	public Transform leftWall;
	public Transform rightWall;
	public int maxBlockSize = 4;
	public int _fieldWidth = 10;
	public int _fieldHeight = 13;
	public float blockNormalFallSpeed = 2f;
	public float blockDropSpeed = 30f;
	public Texture2D cubeTexture;
	
	private int fieldWidth;
	private int fieldHeight;
	private bool[,] fields;
	private int[] cubeYposition;
	private Transform[] cubeTransforms;
	private int clearTimes;
	private float addSpeed = .3f;
	private int TimeToAddSpeed = 10;
	
	private int Score = 0;
	private int Highest = 0;
	private int blockRandom;
	private GameObject nextBlock;
	private Block nextB;
	private int nextSize;
	private string[] nextblock;
	
	public static Manager manager;

	// Use this for initialization
	void Start () {
	
		if (manager == null){
			manager = this;
		}
		
		if (PlayerPrefs.HasKey("Highest")){
			Highest = PlayerPrefs.GetInt("Highest");
		}
		else{
			PlayerPrefs.SetInt("Highest", 0);
		}

        //change the floor color
        GameObject.Find("floor").renderer.material.color = Color.grey;

		blockRandom = Random.Range(0, blocks.Length);
		
        //2D view use this so We don't need  _fieldWidth + maxBlockSize 
		fieldWidth = _fieldWidth + maxBlockSize * 2;//fieldWitdh = 22
		fieldHeight = _fieldHeight + maxBlockSize; //fieldHeight = 34

        //fieldWidth = _fieldWidth;
        //fieldHeight = _fieldHeight;

		fields = new bool[fieldWidth, fieldHeight];
		cubeYposition = new int[fieldHeight * fieldWidth];
		cubeTransforms = new Transform[fieldHeight * fieldWidth];
		
		for (int i = 0;i < fieldHeight;i++){
			
			for (int j = 0 ;j < maxBlockSize;j++){
				
				fields[j, i] = true;
				fields[fieldWidth -1 - j, i] = true;
				
			}
			
		}
		
		for (int i = 0;i < fieldWidth;i++){
			fields[i, 0] = true;
		}
		
		//leftWall.position = new Vector3(maxBlockSize - .5f, leftWall.position.y, leftWall.position.z);
		//rightWall.position = new Vector3(fieldWidth - maxBlockSize + .5f, rightWall.position.y, rightWall.position.z);
		//Camera.main.transform.position = new Vector3(fieldWidth/2, fieldHeight/2, -16.0f);
		
		CreateBlock(blockRandom);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void CreateBlock(int random){
		Instantiate(blocks[random]);
		blockRandom = Random.Range(0, blocks.Length);
		nextBlock = blocks[blockRandom];
		nextB = (Block)nextBlock.GetComponent("Block");
		nextSize = nextB.block.GetLength(0);
		nextblock = new string[nextSize];
		nextblock = nextB.block;
	}
	
	public int GetFieldWidth(){
		return fieldWidth;
	}
	
	public int GetFieldHeight(){
		return fieldHeight;
	}
	
	public int GetBlockRandom(){
		return blockRandom;
	}
	
	public bool CheckBlock(bool [,] blockMatrix, int xPos, int yPos){

		var size = blockMatrix.GetLength(0);
		/*
		for (int y=size-1;y>=0;y--){
			for (int x=0;x<size;x++){
				//print(xPos + " " + yPos + " " + blockMatrix[x, y] + " " + fields[xPos + x, yPos - y]);
				if (blockMatrix[x, y] && fields[xPos + x, yPos - y]){
					return true;
				}
			}
		}*/
		for (int y = 0;y < size;y++){
			for (int x = 0;x < size;x++){
				if (blockMatrix[y, x] && fields[xPos + x, yPos - y]){
					return true;
				}
			}
		}
		
		return false;
	}
	
	public void SetBlock(bool[,] blockMatrix, int xPos, int yPos){
		
		int size = blockMatrix.GetLength(0);
		for (int y = 0;y < size;y++){
			for (int x = 0;x < size;x++){
				if (blockMatrix[y, x]){
					Instantiate(cube, new Vector3(xPos + x, yPos - y, 7.0f), Quaternion.identity);
					fields[xPos + x, yPos - y] = true;
				}
			}
		}

        for (int i = 0; i < fieldWidth; i++) {
            for (int j = 0; j < fieldHeight; j++) {

                if (fields[i, j]) {
                    Debug.Log("x = " + i + "y = " + j );
                
                }
            
            }
        
        }
            

            StartCoroutine(CheckRows(yPos - size, size));
		
	}
	
	IEnumerator CheckRows(int yStart, int size){
		yield return null;
		if (yStart < 1)yStart = 1;
		int count = 1;
		for (int y = yStart;y < yStart + size;y++){
			int x;
			for (x = maxBlockSize;x < fieldWidth - maxBlockSize;x++){
				if (!fields[x, y]){
					break;
				}
			}
			if (x == fieldWidth - maxBlockSize){
				yield return StartCoroutine(SetRows(y));
				Score += 10 * count;
				y--;
				count++;
			}
		}
		CreateBlock(blockRandom);
	}
	
	IEnumerator SetRows(int yStart){
		for (int y = yStart;y < fieldHeight - 1;y++){
			for (int x = maxBlockSize;x < fieldWidth - maxBlockSize;x++){
				fields[x, y] = fields[x, y + 1];
			}
		}
		
		for (int x = maxBlockSize;x < fieldWidth - maxBlockSize;x++){
			fields[x, fieldHeight - 1] = false;
		}
		
		var cubes = GameObject.FindGameObjectsWithTag("Cube");
		int cubeToMove = 0;
		for (int i = 0;i < cubes.Length;i++){
			GameObject cube = cubes[i];
			if (cube.transform.position.y > yStart){
				cubeYposition[cubeToMove] = (int)(cube.transform.position.y);
				cubeTransforms[cubeToMove++] = cube.transform;
			}
			else if (cube.transform.position.y == yStart){
				Destroy(cube);
			}
		}
		
		float t = 0;
		while (t <= 1f){
			t += Time.deltaTime * 5f;
			for(int i = 0;i < cubeToMove;i++){
				cubeTransforms[i].position = new Vector3(cubeTransforms[i].position.x, Mathf.Lerp(cubeYposition[i], cubeYposition[i] - 1, t),
					cubeTransforms[i].position.z);
			}
		    yield return null;
		}
		
		if (++clearTimes == TimeToAddSpeed){
			blockNormalFallSpeed += addSpeed;
			clearTimes = 0;
		}
		
	}
	
	public void GameOver(){
		if (Score > PlayerPrefs.GetInt("Highest")){
			PlayerPrefs.SetInt("Highest", Score);
		}
		print("Game Over!!!");
	}
	
	void OnGUI(){
        //draw the score use GUI
		GUI.Label(new Rect(20, 30, 80, 40),"Score:");
		GUI.Label(new Rect(80, 30, 100, 40),Score.ToString());
		GUI.Label(new Rect(20, 50, 80, 40),"Highest:");
		GUI.Label(new Rect(80, 50, 80, 40),Highest.ToString());
		
        
        //draw the nextBlock use the GUI.Button
		for (int y = 0;y < nextSize;y++){
			for (int x = 0;x < nextSize;x++){
				if (nextblock[y][x] == '1'){
					GUI.Button(new Rect(20 + 30 * x, 100 + 30 * y, 30, 30), cubeTexture);
				}
			}
		}
	
        
          
         }
	
}

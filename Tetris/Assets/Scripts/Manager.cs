using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {
	
	public GameObject[] blocks;                         //save the block
	public Transform cube;                              //��ɷ����Сcube
	public Transform leftWall;                          //��ߵ�ǽ
	public Transform rightWall;                         //�ұߵ�ǽ
	public int maxBlockSize = 4;                        //�������ĳߴ�   �������
	public int _fieldWidth = 10;                        //ʵ�ʿճ����Ŀ��
    public int _fieldHeight = 13;                       //ʵ�ʿճ����ĸ߶�  
	public float blockNormalFallSpeed = 2f;             //��������������ٶ�
	public float blockDropSpeed = 30f;                  //�����·�����󷽿�������ٶ�
	public Texture cubeTexture;                         //cube����ͼ
	
	private int fieldWidth;                             //ǽ�Ӻ�֮��ĳ������
    private int fieldHeight;                            //ǽ�Ӻ�֮��ĳ����߶�
	private bool[,] fields;                             //�洢ǽ�Ӻ�֮����������������
	private int[] cubeYposition;                        //�洢����������Y����
	private Transform[] cubeTransforms;                 //�洢��������������
	private int clearTimes;                             //���ʱ��
	private float addSpeed = .3f;                       //���������ٶȵ�����
	private int TimeToAddSpeed = 10;                    //�������ʱ��ͻ����ӷ���������ٶ�
	
	private int Score = 0;                              //��ǰ����
	private int Highest = 0;                            //��߷���
	private int blockRandom;                            //�����������������
	private GameObject nextBlock;                       //��һ���������
	private Block nextB;                                //��һ������
	private int nextSize;                               //��һ������ĳߴ�
	private string[] nextblock;                         //�洢��һ���������״���ַ�������
	
	public static Manager manager;                      //ʵ��һ��Manager�࣬��ʵ�ֵ���ģʽ

	// Use this for initialization
	void Start () {
	
		if (manager == null){
            //���managerΪ�վ�ʵ������ǰ����
			manager = this;
		}
		
		if (PlayerPrefs.HasKey("Highest")){
			Highest = PlayerPrefs.GetInt("Highest");
		}
		else{
			PlayerPrefs.SetInt("Highest", 0);
		}

        //�ı�ذ����ɫ
        GameObject.Find("floor").renderer.material.color = Color.grey;

        //�������һ������������
		blockRandom = Random.Range(0, blocks.Length);
		
        //���ֱ��ʹ����ʵ��ȣ�����ᴩǽ�����԰�ǽ������Ҳ����Ϊ������
        //2D view use this so We don't need  _fieldWidth + maxBlockSize 
		fieldWidth = _fieldWidth + maxBlockSize * 2;//fieldWitdh = 18
		fieldHeight = _fieldHeight + maxBlockSize; //fieldHeight = 17

        //fieldWidth = _fieldWidth;
        //fieldHeight = _fieldHeight;

        //������ǽ�Ӻ�֮��������浽fields������ռ����block = true�����Ϊ���� = false
		fields = new bool[fieldWidth, fieldHeight];

        //�����е�������Y������ȥ
		cubeYposition = new int[fieldHeight * fieldWidth];

        //�洢����������ϵ�����
		cubeTransforms = new Transform[fieldHeight * fieldWidth];
		
		for (int i = 0;i < fieldHeight;i++){
			
			for (int j = 0 ;j < maxBlockSize;j++){
				
                //����ߵ�ǽ�Ӻ񷽿����ߴ�Ĵ�С
				fields[j, i] = true;

                //���ұߵ�ǽ��Ӻ�
				fields[fieldWidth - j - 1, i] = true;
				
			}
			
		}
		

        //�õ������������㶼��Ϊ������
		for (int i = 0;i < fieldWidth;i++){
			fields[i, 0] = true;
		}
		
        //�趨ǽ���λ�ã���Unity�����ò������ǲ���
		leftWall.position = new Vector3(maxBlockSize - .5f, leftWall.position.y, leftWall.position.z);
		rightWall.position = new Vector3(fieldWidth - maxBlockSize + .5f, rightWall.position.y, rightWall.position.z);
        //������������м�λ��
		Camera.main.transform.position = new Vector3(fieldWidth/2, fieldHeight/2, -16.0f);
		
        //���ò�������ĺ���
		CreateBlock(blockRandom);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
    /*
     * ��������
       ÿһ�����鶼����һ��block�ű�
       ʵ�������Զ����ýű����г�ʼ��
     */
	void CreateBlock(int random){

        //���ʵ����һ������
		Instantiate(blocks[random]);
        //����һ�������
		blockRandom = Random.Range(0, blocks.Length);
        //������һ�������String
		nextBlock = blocks[blockRandom];
		nextB = (Block)nextBlock.GetComponent("Block");
		nextSize = nextB.block.GetLength(0);
		nextblock = new string[nextSize];

        //���ýű���nextblockʵ����
		nextblock = nextB.block;
	}
	
	public int GetFieldWidth(){

        //����ǽ�Ӻ�֮��ĺ���
		return fieldWidth;
	}
	
	public int GetFieldHeight(){

        //��������Ӻ�֮��ĳ����߶�
		return fieldHeight;
	}
	
	public int GetBlockRandom(){

        //����������������
		return blockRandom;
	}
	

    /*
        ��鷽���Ƿ�����ǽ�ڻ�����������
     
     */
	public bool CheckBlock(bool [,] blockMatrix, int xPos, int yPos){

		var size = blockMatrix.GetLength(0);
		
		for (int y = 0;y < size;y++){
			for (int x = 0;x < size;x++){
                //����������볡����������ͬ������㲢�Ҹ�����㶼��block�����򷵻�true���ж�������ײ
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
					Instantiate(cube, new Vector3(xPos + x, yPos - y, 5.0f), Quaternion.identity);
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
        //load the GameOver scene
        Application.LoadLevel("GameOver");
        
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

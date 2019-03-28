using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ConnectFour
{
	public class GameController : MonoBehaviour 
	{
        [SerializeField]
        public GameObject fieldPrefab;

        [SerializeField]
        private GameObject redChipPrefab;

        [SerializeField]
        public GameObject yellowChipPrefab;

        [SerializeField]
		private int numRows;

		[SerializeField]
		private int numColumns;

		[SerializeField]
		private int numPiecesToWin;

        [SerializeField]
        private float dropTime;

        [SerializeField]
		private bool allowDiagonally;

        private GameObject gameObjectField;
        private GameObject gameObjectTurn;
        private Flags flags;
        private CanvasMessageManager messageManager;
        private Camera mainCamera;

		private int[,] field;

        private void Awake()
        {
            flags = FindObjectOfType<Flags>();
            messageManager = FindObjectOfType<CanvasMessageManager>();
            mainCamera = Camera.main;
        }

        void Start () 
		{
			int max = Mathf.Max (numRows, numColumns);

			if(numPiecesToWin > max)
            {
                numPiecesToWin = max;
            }				

			CreateBoard();
            flags.ConvertIsPlayersTurnToBoolean();
		}

        private void Update()
        {
            if(flags.GetIsLoading())
            {
                return;
            }               

            if (flags.GetIsCheckingForWinner())
            {
                return;
            }

            if(flags.GetGameOver())
            {
                return;
            }

            if(flags.GetIsPlayersTurn())
            {
                if (gameObjectTurn == null)
                {
                    gameObjectTurn = SpawnPiece();
                }
                else
                {
                    // update the objects position
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    gameObjectTurn.transform.position = new Vector3(
                        Mathf.Clamp(pos.x, 0, numColumns - 1),
                        gameObjectField.transform.position.y + 1, 0);

                    // click the left mouse button to drop the piece into the selected column
                    if(Input.GetMouseButtonDown(0) && !flags.GetMouseButtonPressed() && !flags.GetIsDropping())
                    {
                        flags.SetIsMouseButtonPressedTrue();
                        StartCoroutine(DropPiece(gameObjectTurn));
                    }
                    else
                    {
                        flags.SetIsMouseButtonPressedFalse();
                    }
                }
            }
            else
            {
                if (gameObjectTurn == null)
                {
                    gameObjectTurn = SpawnPiece();
                }
                else
                {
                    if(!flags.GetIsDropping())
                    {
                        StartCoroutine(DropPiece(gameObjectTurn));
                    }                        
                }
            }
        }

        private void CreateBoard()
		{
            flags.SetIsLoadingTrue();

			gameObjectField = GameObject.Find ("Field");
			if(gameObjectField != null)
			{
				DestroyImmediate(gameObjectField);
			}
			gameObjectField = new GameObject("Field");

			// create an empty field and instantiate the cells
			field = new int[numColumns, numRows];
			for(int x = 0; x < numColumns; x++)
			{
				for(int y = 0; y < numRows; y++)
				{
					field[x, y] = (int)Piece.Empty;
					GameObject g = Instantiate(fieldPrefab, new Vector3(x, y * -1, -1), Quaternion.identity) as GameObject;
					g.transform.parent = gameObjectField.transform;
				}
			}

            flags.SetIsLoadingFalse();
            flags.SetGameOverFalse();

            CenterCamera();		
		}

        private void CenterCamera()
        {
            mainCamera.transform.position = new Vector3((numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f), Camera.main.transform.position.z);
        }

        private GameObject SpawnPiece()
		{
			Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(!flags.GetIsPlayersTurn())
			{
				List<int> moves = GetPossibleMoves();

				if(moves.Count > 0)
				{
					int column = moves[UnityEngine.Random.Range (0, moves.Count)];
					spawnPos = new Vector3(column, 0, 0);
				}
			}

			GameObject g = Instantiate(
                    flags.GetIsPlayersTurn() ? yellowChipPrefab : redChipPrefab, // is players turn = spawn blue, else spawn red
					new Vector3(
					Mathf.Clamp(spawnPos.x, 0, numColumns-1), 
					gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
					Quaternion.identity) as GameObject;

			return g;
		}

		public List<int> GetPossibleMoves()
		{
			List<int> possibleMoves = new List<int>();
			for (int x = 0; x < numColumns; x++)
			{
				for(int y = numRows - 1; y >= 0; y--)
				{
					if(field[x, y] == (int)Piece.Empty)
					{
						possibleMoves.Add(x);
						break;
					}
				}
			}
			return possibleMoves;
		}

		private IEnumerator DropPiece(GameObject gObject)
		{
            flags.SetIsDroppingTrue();

			Vector3 startPosition = gObject.transform.position;
			Vector3 endPosition = new Vector3();

			// round to a grid cell
			int x = Mathf.RoundToInt(startPosition.x);
			startPosition = new Vector3(x, startPosition.y, startPosition.z);

			// is there a free cell in the selected column?
			bool foundFreeCell = false;
			for(int i = numRows-1; i >= 0; i--)
			{
				if(field[x, i] == 0)
				{
					foundFreeCell = true;
					//field[x, i] = isPlayersTurn ? (int)Piece.Blue : (int)Piece.Red;
                    field[x, i] = flags.GetIsPlayersTurn() ? (int)Piece.Blue : (int)Piece.Red;
                    endPosition = new Vector3(x, i * -1, startPosition.z);

					break;
				}
			}

			if(foundFreeCell)
			{
				// Instantiate a new Piece, disable the temporary
				GameObject g = Instantiate (gObject) as GameObject;
				gameObjectTurn.GetComponent<Renderer>().enabled = false;

				float distance = Vector3.Distance(startPosition, endPosition);

				float t = 0;
				while(t < 1)
				{
					t += Time.deltaTime * dropTime * ((numRows - distance) + 1);

					g.transform.position = Vector3.Lerp (startPosition, endPosition, t);
					yield return null;
				}

				g.transform.parent = gameObjectField.transform;

				// remove the temporary gameobject
				DestroyImmediate(gameObjectTurn);

				// run coroutine to check if someone has won
				StartCoroutine(Won());

				// wait until winning check is done
                while(flags.GetIsCheckingForWinner())
                {
                    yield return null;
                }					

                flags.InvertIsPlayersTurn();
			}

            flags.SetIsDroppingFalse();

			yield return 0;
		}

		private IEnumerator Won()
		{
            flags.SetIsCheckingForWinnerTrue();

			for(int x = 0; x < numColumns; x++)
			{
				for(int y = 0; y < numRows; y++)
				{
					// Get the Laymask to Raycast against, if its Players turn only include
					// Layermask Blue otherwise Layermask Red

					//int layermask = isPlayersTurn ? (1 << 8) : (1 << 9);
                    int layermask = flags.GetIsPlayersTurn() ? (1 << 8) : (1 << 9);

                    // If its Players turn ignore red as Starting piece and wise versa
                    //if (field[x, y] != (isPlayersTurn ? (int)Piece.Blue : (int)Piece.Red))
                    if (field[x, y] != (flags.GetIsPlayersTurn() ? (int)Piece.Blue : (int)Piece.Red))
					{
						continue;
					}

					// shoot a ray of length 'numPiecesToWin - 1' to the right to test horizontally
					RaycastHit[] hitsHorz = Physics.RaycastAll(
						new Vector3(x, y * -1, 0), 
						Vector3.right, 
						numPiecesToWin - 1, 
						layermask);

					// return true (won) if enough hits
					if(hitsHorz.Length == numPiecesToWin - 1)
					{
                        flags.SetGameOverTrue();
						break;
					}

					// shoot a ray up to test vertically
					RaycastHit[] hitsVert = Physics.RaycastAll(
						new Vector3(x, y * -1, 0), 
						Vector3.up, 
						numPiecesToWin - 1, 
						layermask);
					
					if(hitsVert.Length == numPiecesToWin - 1)
					{
                        flags.SetGameOverTrue();
						break;
					}

					// test diagonally
					if(allowDiagonally)
					{
						// calculate the length of the ray to shoot diagonally
						float length = Vector2.Distance(new Vector2(0, 0), new Vector2(numPiecesToWin - 1, numPiecesToWin - 1));

						RaycastHit[] hitsDiaLeft = Physics.RaycastAll(
							new Vector3(x, y * -1, 0), 
							new Vector3(-1 , 1), 
							length, 
							layermask);
						
						if(hitsDiaLeft.Length == numPiecesToWin - 1)
						{
                            flags.SetGameOverTrue();
							break;
						}

						RaycastHit[] hitsDiaRight = Physics.RaycastAll(
							new Vector3(x, y * -1, 0), 
							new Vector3(1 , 1), 
							length, 
							layermask);
						
						if(hitsDiaRight.Length == numPiecesToWin - 1)
						{
							//gameOver = true;
                            flags.SetGameOverTrue();
							break;
						}
					}

					yield return null;
				}

				yield return null;
			}

            if(flags.GetGameOver() == true)
			{
                if(flags.GetIsPlayersTurn())
                {
                    messageManager.ShowPlayerWonMessage();
                }

                else if(!flags.GetIsPlayersTurn())
                {
                    messageManager.ShowComputerWonMessage();
                }
            }
			else 
			{
				// check if there are any empty cells left, if not set game over and update text to show a draw
				if(!FieldContainsEmptyCell())
				{
                    flags.SetGameOverTrue();
                    messageManager.ShowDrawMessage();
				}
			}

            flags.SetIsCheckingForWinnerFalse();

			yield return 0;
		}

		private bool FieldContainsEmptyCell()
		{
			for(int x = 0; x < numColumns; x++)
			{
				for(int y = 0; y < numRows; y++)
				{
					if(field[x, y] == (int)Piece.Empty)
						return true;
				}
			}
			return false;
		}

        public int GetNumberOfRows()
        {
            return numRows;
        }
	}
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviour {
    [SerializeField]
    private GameObject backgroundCellPrefab;

    [SerializeField]
    private Transform cellsBackgroundTransform;

    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private Transform cellsParentTransform;

    [SerializeField]
    private TextMeshProUGUI currentScoreText;

    [SerializeField]
    private FloatingText currentScoreFloating;

    [SerializeField]
    private TextMeshProUGUI highScoreText;

    [SerializeField]
    private FloatingText highScoreFloating;

    private KeyboardController keyboardController;
    private GameOver gameOver;

    private BoardCell[,] board = new BoardCell[4, 4];
    private System.Random rand = new System.Random();

    private int _score = 0;
    private int score {
        get {
            return _score;
        }
        set {
            if (value > _score) {
                currentScoreFloating.startAnimation(value.ToString());
            }
            _score = value;
            if (_score > highScore) {
                highScore = _score;
                PlayerPrefs.SetInt("HighScore", _score);
                highScoreFloating.startAnimation(value.ToString());
               // PlayerPrefs.Save();
            }
        }
    }
    private int highScore = 0;
    
	void Start () {
        keyboardController = GetComponent<KeyboardController>();
        gameOver = GetComponent<GameOver>();

        generateBackgroundCells();
        loadPlayerData();
        resetGame();
    }

    // Background cells are just for visuals, never move or get otherwise updated
    private void generateBackgroundCells() {
        for (int y = 0; y < 4; y++) {
            for (int x = 0; x < 4; x++) {
                GameObject newCell = Instantiate(backgroundCellPrefab);
                newCell.transform.parent = cellsBackgroundTransform;
                newCell.transform.position = new Vector2(x, y);
            }
        }
    }

    private void loadPlayerData() {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void resetGame() {
        for (int y = 0; y < 4; y++) {
            for (int x = 0; x < 4; x++) {
                if (board[x, y] != null) {
                    Destroy(board[x, y].gameObject);
                }
                board[x, y] = null;
            }
        }

        score = 0;
        displayScore();

        addRandomCell();
        addRandomCell();

        keyboardController.enabled = true;
        gameOver.reset();
    }

    public void quitGame() {
        Application.Quit();
    }

    private void displayScore() {
        currentScoreText.text = score.ToString();
        highScoreText.text = highScore.ToString();
    }

    private List<Position> getEmptyCells() {
        List<Position> emptyCells = new List<Position>();

        for (int y = 0; y < 4; y++) {
            for (int x = 0; x < 4; x++) {
                if (board[x, y] == null) {
                    emptyCells.Add(new Position(x, y));
                }
            }
        }

        return emptyCells;
    }

    private void addRandomCell() {
        List<Position> emptyCells = getEmptyCells();
        if (emptyCells.Count == 0) {
            GameOver(false);
            return;
        }
        
        // Pick up a random empty cell
        Position cellPos = emptyCells[rand.Next(emptyCells.Count)];

        GameObject newCell = Instantiate(cellPrefab);
        newCell.transform.parent = cellsParentTransform;
        newCell.transform.position = new Vector2(cellPos.X, cellPos.Y);

        board[cellPos.X, cellPos.Y] = newCell.GetComponent<BoardCell>();
        board[cellPos.X, cellPos.Y].Value = 2;
    }

    private void GameOver(bool won) {
        keyboardController.enabled = false;
        gameOver.gameOver(won);
    }

    public void Move(MoveDirection direction) {
        bool doneAction = false;
        bool won = false;

        // Important: 0,0 coordinate is bottom left, not the more typical top left (!), hence Up and Down may seem reversed

        switch (direction) {
            case MoveDirection.Up:
                // x 0 -> 3, then y 3 -> 0
                for (int x = 0; x < 4; x++) {
                    BoardCell cellBefore = null;
                    int spacesBefore = 0;
                    bool canMerge = true;

                    for (int y = 3; y >= 0; y--) {
                        if (board[x, y] != null) {
                            if (cellBefore != null && cellBefore.Value == board[x, y].Value && canMerge) {
                                // We can merge with previous cell

                                // Update score
                                score = score + cellBefore.Value * 2;

                                // We released a new spot by destroying this cell
                                spacesBefore++;
                                // Move and merge with previous cell
                                board[x, y].MoveAndDestroy(MoveDirection.Up, spacesBefore, cellBefore);
                                board[x, y] = null;
                                canMerge = false;

                                if (cellBefore.Value * 2 == 2048) {
                                    won = true;
                                }

                                doneAction = true;
                            } else {
                                // We can't merge, but we can move

                                cellBefore = board[x, y];
                                canMerge = true;
                                if (spacesBefore > 0) {
                                    board[x, y].Move(MoveDirection.Up, spacesBefore);
                                    board[x, y + spacesBefore] = board[x, y];
                                    board[x, y] = null;

                                    doneAction = true;
                                }
                            }
                        } else {
                            spacesBefore++;
                        }
                    }
                }
                break;

            case MoveDirection.Down:
                // x 0 -> 3, then y 0 -> 3
                for (int x = 0; x < 4; x++) {
                    BoardCell cellBefore = null;
                    int spacesBefore = 0;
                    bool canMerge = true;

                    for (int y = 0; y < 4; y++) {
                        if (board[x, y] != null) {
                            if (cellBefore != null && cellBefore.Value == board[x, y].Value && canMerge) {
                                // We can merge with previous cell

                                // Update score
                                score = score + cellBefore.Value * 2;

                                // We released a new spot by destroying this cell
                                spacesBefore++;
                                // Move and merge with previous cell
                                board[x, y].MoveAndDestroy(MoveDirection.Down, spacesBefore, cellBefore);
                                board[x, y] = null;
                                canMerge = false;

                                doneAction = true;
                            } else {
                                // We can't merge, but we can move

                                cellBefore = board[x, y];
                                canMerge = true;
                                if (spacesBefore > 0) {
                                    board[x, y].Move(MoveDirection.Down, spacesBefore);
                                    board[x, y - spacesBefore] = board[x, y];
                                    board[x, y] = null;

                                    doneAction = true;
                                }
                            }
                        } else {
                            spacesBefore++;
                        }
                    }
                }
                break;

            case MoveDirection.Left:
                // y 3 -> 0, then x 0 -> 3
                for (int y = 3; y >= 0; y--) {
                    BoardCell cellBefore = null;
                    int spacesBefore = 0;
                    bool canMerge = true;

                    for (int x = 0; x < 4; x++) {
                        if (board[x, y] != null) {
                            if (cellBefore != null && cellBefore.Value == board[x, y].Value && canMerge) {
                                // We can merge with previous cell

                                // Update score
                                score = score + cellBefore.Value * 2;

                                // We released a new spot by destroying this cell
                                spacesBefore++;
                                // Move and merge with previous cell
                                board[x, y].MoveAndDestroy(MoveDirection.Left, spacesBefore, cellBefore);
                                board[x, y] = null;
                                canMerge = false;

                                doneAction = true;
                            } else {
                                // We can't merge, but we can move

                                cellBefore = board[x, y];
                                canMerge = true;
                                if (spacesBefore > 0) {
                                    board[x, y].Move(MoveDirection.Left, spacesBefore);
                                    board[x - spacesBefore, y] = board[x, y];
                                    board[x, y] = null;

                                    doneAction = true;
                                }
                            }
                        } else {
                            spacesBefore++;
                        }
                    }
                }
                break;

            case MoveDirection.Right:
                // y 3 -> 0, then x 3 -> 0
                for (int y = 3; y >= 0; y--) {
                    BoardCell cellBefore = null;
                    int spacesBefore = 0;
                    bool canMerge = true;

                    for (int x = 3; x >= 0; x--) {
                        if (board[x, y] != null) {
                            if (cellBefore != null && cellBefore.Value == board[x, y].Value && canMerge) {
                                // We can merge with previous cell

                                // Update score
                                score = score + cellBefore.Value * 2;

                                // We released a new spot by destroying this cell
                                spacesBefore++;
                                // Move and merge with previous cell
                                board[x, y].MoveAndDestroy(MoveDirection.Right, spacesBefore, cellBefore);
                                board[x, y] = null;
                                canMerge = false;

                                doneAction = true;
                            } else {
                                // We can't merge, but we can move

                                cellBefore = board[x, y];
                                canMerge = true;
                                if (spacesBefore > 0) {
                                    board[x, y].Move(MoveDirection.Right, spacesBefore);
                                    board[x + spacesBefore, y] = board[x, y];
                                    board[x, y] = null;

                                    doneAction = true;
                                }
                            }
                        } else {
                            spacesBefore++;
                        }
                    }
                }
                break;
        }

        if (doneAction) {
            displayScore();

            if (won) {
                GameOver(true);
            } else {
                addRandomCell();
            }
        } else {
            // Table full, no moves were able to be made. It's game over!
            if (getEmptyCells().Count == 0) {
                GameOver(false);
            }
        }
    }
}

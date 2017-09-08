using UnityEngine;

public class KeyboardController : MonoBehaviour {
    public float inputRepeatDelay = 1f;
    
    private float timePassed = 0f;

    [SerializeField]
    private GameLogic gameLogic;

    public bool _enabled = true;
    new public bool enabled {
        get {
            return _enabled;
        }

        set {
            _enabled = value;
        }
    }
    
    void Update () {
        timePassed += Time.deltaTime;

        if (timePassed < inputRepeatDelay) {
            return;
        }

        if (!enabled) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            timePassed = 0.0f;
            gameLogic.Move(MoveDirection.Up);
        } else
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            timePassed = 0.0f;
            gameLogic.Move(MoveDirection.Down);
        } else
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            timePassed = 0.0f;
            gameLogic.Move(MoveDirection.Left);
        } else
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            timePassed = 0.0f;
            gameLogic.Move(MoveDirection.Right);
        }
	}
}

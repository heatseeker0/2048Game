using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    [SerializeField]
    private Text winText;

    [SerializeField]
    private Text loseText;

    private void Start() {
        reset();
    }

    public void reset() {
        winText.enabled = false;
        loseText.enabled = false;
    }

    public void gameOver(bool won) {
        if (won) {
            winText.enabled = true;
        } else {
            loseText.enabled = true;
        }
    }
}

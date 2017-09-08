using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {
    [SerializeField]
    private Transform originalPosition;

    [SerializeField]
    private Text text;

    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private float timeBeforeFade = 0.1f;
    
    [SerializeField]
    private float timeFade = 0.1f;

    [SerializeField]
    private int maxFontSize = 48;

    private float _timeBeforeFade;
    private float _timeFade;
    private bool doAnimation = false;

    private Color originalColor;
    private Color originalTransparent;

    private int originalFontSize;

    private void Start() {
        originalColor = text.color;
        originalTransparent = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        originalFontSize = text.fontSize;
        text.enabled = false;
    }

    public void startAnimation(string message) {
        text.transform.position = originalPosition.position;

        text.text = message;
        text.color = originalColor;
        text.enabled = true;
        text.fontSize = originalFontSize;

        _timeFade = timeFade;
        _timeBeforeFade = timeBeforeFade;

        doAnimation = true;
    }
    
    void Update() {
        if (!doAnimation) {
            return;
        }

        text.transform.Translate(0, moveSpeed, 0);

        text.fontSize = (int) Mathf.Lerp(maxFontSize, originalFontSize, _timeBeforeFade / timeBeforeFade);

        if (_timeBeforeFade > 0) {
            _timeBeforeFade -= Time.deltaTime;
        } else {
            _timeFade -= Time.deltaTime;
            text.color = Color.Lerp(originalTransparent, originalColor, _timeFade / timeFade);
        }

        if (_timeFade <= 0) {
            text.enabled = false;
            doAnimation = false;
        }
    }
}

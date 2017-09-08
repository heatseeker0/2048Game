using System.Collections;
using TMPro;
using UnityEngine;

public class BoardCell : MonoBehaviour {
    public float showTime = .1f;

    public float bumpTime = .1f;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private TextMeshPro text;

    private CellMove cellMove;

    private int _Value = 0;
    public int Value {
        get {
            return _Value;
        }
        set {
            if (_Value != value) {
                _Value = value;
                needUpdate = true;
            }
        }
    }

    private bool needUpdate = true;
    private bool needFadeIn = true;

    public void MoveAndDestroy(MoveDirection direction, int tileCount, BoardCell cellToUpdate) {
        cellMove.MoveAndDestroy(direction, tileCount, cellToUpdate);
    }

    public void Move(MoveDirection direction, int tileCount) {
        cellMove.Move(direction, tileCount);
    }

    private void Start() {
        cellMove = GetComponent<CellMove>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        text.alpha = 0f;
    }

    private void Update() {
        if (!needUpdate) {
            return;
        }

        needUpdate = false;

        text.text = Value.ToString();

        if (Value <= 0) {
            spriteRenderer.color = CellColors.Instance.CellBg0;
        } else
        if (Value <= 2) {
            spriteRenderer.color = CellColors.Instance.CellBg2;
        } else
        if (Value <= 4) {
            spriteRenderer.color = CellColors.Instance.CellBg4;
        } else
        if (Value <= 8) {
            spriteRenderer.color = CellColors.Instance.CellBg8;
        } else
        if (Value <= 16) {
            spriteRenderer.color = CellColors.Instance.CellBg16;
        } else
        if (Value <= 32) {
            spriteRenderer.color = CellColors.Instance.CellBg32;
        } else
        if (Value <= 64) {
            spriteRenderer.color = CellColors.Instance.CellBg64;
        } else
        if (Value <= 128) {
            spriteRenderer.color = CellColors.Instance.CellBg128;
        } else
        if (Value <= 256) {
            spriteRenderer.color = CellColors.Instance.CellBg256;
        } else
        if (Value <= 512) {
            spriteRenderer.color = CellColors.Instance.CellBg512;
        } else
        if (Value <= 1024) {
            spriteRenderer.color = CellColors.Instance.CellBg1024;
        } else {
            spriteRenderer.color = CellColors.Instance.CellBg2048;
        }

        if (needFadeIn) {
            needFadeIn = false;
            StartCoroutine(SmoothFadeIn());
        } else {
            StartCoroutine(Bump());
        }
    }

    protected IEnumerator SmoothFadeIn() {
        float currentFadeVelocity = 0f;
        float currentScaleVelocity = 0f;

        float fade = 0f;
        Color color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, fade);

        Vector3 originalScaleVector = spriteRenderer.transform.localScale;
        float originalScale = originalScaleVector.x;
        float scale = 0.1f;

        while (fade < 0.9f) {
            fade = Mathf.SmoothDamp(fade, 1f, ref currentFadeVelocity, showTime);
            color.a = fade;
            spriteRenderer.color = color;
            text.alpha = fade;

            scale = Mathf.SmoothDamp(scale, originalScale, ref currentScaleVelocity, showTime);
            spriteRenderer.transform.localScale = new Vector3(scale, scale, 1);

            yield return null;
        }

        color.a = 1;
        spriteRenderer.color = color;
        text.alpha = 1;
        spriteRenderer.transform.localScale = originalScaleVector;
    }

    protected IEnumerator Bump() {
        float currentVelocity = 0f;
        float halfBumpTime = bumpTime / 2;

        Vector3 originalScaleVector = spriteRenderer.transform.localScale;
        float originalScale = originalScaleVector.x;
        float scale = originalScale;

        while (scale < 1f) {
            scale = Mathf.SmoothDamp(scale, 1.1f, ref currentVelocity, halfBumpTime);
            spriteRenderer.transform.localScale = new Vector3(scale, scale, 1);

            yield return null;
        }

        currentVelocity = 0f;
        while (scale > originalScale) {
            scale = Mathf.SmoothDamp(scale, originalScale - .1f, ref currentVelocity, halfBumpTime);
            spriteRenderer.transform.localScale = new Vector3(scale, scale, 1);

            yield return null;
        }

        spriteRenderer.transform.localScale = originalScaleVector;
    }
}

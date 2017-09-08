using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CellMove : MonoBehaviour {

    public float moveTime = .1f;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private TextMeshPro textMesh;

    private Rigidbody2D rigidBody;
    private float inverseMoveTime;

    void Start () {
        rigidBody = GetComponent<Rigidbody2D>();

        inverseMoveTime = 1f / moveTime;
    }

    // Moves a cell, destroyes it at the end of the move and updates (doubles) cellToUpdate value
    public void MoveAndDestroy(MoveDirection direction, int tileCount, BoardCell cellToUpdate) {

        StartCoroutine(SmoothFadeOut());
        Move(direction, tileCount, new Action(delegate() {
            cellToUpdate.Value *= 2;
            Destroy(gameObject);
        }));
    }

    // Moves a cell without performing any special action at the end
    public void Move(MoveDirection direction, int tileCount) {
        Move(direction, tileCount, null);
    }

    private void Move(MoveDirection direction, int tileCount, Action action) {
        Vector2 start = transform.position;
        Vector2 end = start;

        switch (direction) {
            case MoveDirection.Up:
                end += new Vector2(0, tileCount);
                break;
            case MoveDirection.Down:
                end += new Vector2(0, -tileCount);
                break;
            case MoveDirection.Left:
                end += new Vector2(-tileCount, 0);
                break;
            case MoveDirection.Right:
                end += new Vector2(tileCount, 0);
                break;
        }

        StartCoroutine(SmoothMovement(end, action));
    }

    protected IEnumerator SmoothMovement(Vector3 end, Action method) {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(rigidBody.position, end, inverseMoveTime * Time.deltaTime);
            rigidBody.MovePosition(newPosition);

            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }
        rigidBody.MovePosition(end);
        if (method != null) {
            method();
        }
    }

    protected IEnumerator SmoothFadeOut() {
        float currentVelocity = 0f;

        float fade = 1f;
        Color color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, fade);

        while (fade > 0.1f) {
            fade = Mathf.SmoothDamp(fade, 0f, ref currentVelocity, moveTime);
            color.a = fade;
            spriteRenderer.color = color;
            textMesh.alpha = fade;
            yield return null;
        }
    }
}

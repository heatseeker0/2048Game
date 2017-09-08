using UnityEngine;

public class CellColors : MonoBehaviour {
    public static CellColors Instance { get; private set; }

    public Color CellBg0;
    public Color CellBg2;
    public Color CellBg4;
    public Color CellBg8;
    public Color CellBg16;
    public Color CellBg32;
    public Color CellBg64;
    public Color CellBg128;
    public Color CellBg256;
    public Color CellBg512;
    public Color CellBg1024;
    public Color CellBg2048;

    private void Start() {
        Instance = this;
    }
}

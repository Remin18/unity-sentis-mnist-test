using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

[RequireComponent(typeof(MeshCollider))]
public class DrawingSystem : MonoBehaviour
{
    public float brushSize = 0.5f;
    public Color brushColor = Color.white;
    public Texture2D drawTexture { get; private set; }
    public bool isDrawing { get; private set; } = false;

    private Vector2 previousDrawPosition;

    void Start()
    {
        drawTexture = new Texture2D(28, 28);

        drawTexture.filterMode = FilterMode.Trilinear;

        for (int y = 0; y < drawTexture.height; y++)
        {
            for (int x = 0; x < drawTexture.width; x++)
            {
                drawTexture.SetPixel(x, y, Color.black);
            }
        }
        drawTexture.Apply();

        GetComponent<Renderer>().material.mainTexture = drawTexture;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                Vector2 drawPosition = new Vector2(
                    hit.textureCoord.x * drawTexture.width,
                    hit.textureCoord.y * drawTexture.height
                );

                if (isDrawing)
                {
                    DrawLine(previousDrawPosition, drawPosition);
                }
                else
                {
                    DrawPoint(drawPosition);
                    isDrawing = true;
                }

                previousDrawPosition = drawPosition;
                drawTexture.Apply();
            }
        }
        else
        {
            isDrawing = false;
        }
    }

    void DrawPoint(Vector2 position)
    {
        float radiusSquared = brushSize * brushSize;

        int minX = Mathf.FloorToInt(position.x - brushSize);
        int maxX = Mathf.CeilToInt(position.x + brushSize);
        int minY = Mathf.FloorToInt(position.y - brushSize);
        int maxY = Mathf.CeilToInt(position.y + brushSize);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (x >= 0 && x < drawTexture.width && y >= 0 && y < drawTexture.height)
                {
                    float distSquared = (x - position.x) * (x - position.x) +
                                      (y - position.y) * (y - position.y);

                    if (distSquared <= radiusSquared)
                    {
                        drawTexture.SetPixel(x, y, brushColor);
                    }
                }
            }
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);

        // 線を滑らかにするために、距離に応じて補間ポイントを増やす
        int steps = Mathf.Max(1, Mathf.CeilToInt(distance * 2));

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(start, end, t);
            DrawPoint(point);
        }
    }

    public void ClearTexture()
    {
        for (int y = 0; y < drawTexture.height; y++)
        {
            for (int x = 0; x < drawTexture.width; x++)
            {
                drawTexture.SetPixel(x, y, Color.black);
            }
        }
        drawTexture.Apply();
    }
}
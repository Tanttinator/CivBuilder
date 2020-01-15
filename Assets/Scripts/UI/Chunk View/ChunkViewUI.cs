using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChunkViewUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public RectTransform chunkParent;
    public GameObject chunkImage;
    public Slider zoom;
    Dictionary<Coord, GameObject> chunkImages = new Dictionary<Coord, GameObject>();
    float speed = 500f;

    bool mouseOver = false;
    int zoomLevel = 4;

    public void PreStart()
    {
        foreach (GameObject GO in chunkImages.Values)
            Destroy(GO);
        chunkImages.Clear();
        zoom.onValueChanged.AddListener(delegate { Zoom((int)zoom.value); });
    }

    public void RegisterCallbacks()
    {
        World.RegisterOnChunkGenerated(CreateChunkImage);
        GetComponent<WindowUI>().RegisterOnShow(Show);
    }

    private void OnDisable()
    {
        World.UnregisterOnChunkGenerated(CreateChunkImage);
    }

    private void Update()
    {
        if (mouseOver)
        {
            if(Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                ZoomIn();
            }else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                ZoomOut();
            }
        }
    }

    public void Show()
    {
        Center();
    }

    public void Move(Vector2 dir)
    {
        chunkParent.anchoredPosition += dir * speed * Time.deltaTime;
    }

    public void ZoomIn()
    {
        if (zoomLevel > 1)
            Zoom(zoomLevel - 1);
    }

    public void ZoomOut()
    {
        if (zoomLevel < 8)
            Zoom(zoomLevel + 1);
    }

    void Zoom(int level)
    {
        zoomLevel = level;
        chunkParent.transform.localScale = new Vector3(1, 1, 1) * (4f / level);
        if(zoom.value != level)
            zoom.value = level;
    }

    public void Center()
    {
        chunkParent.anchoredPosition = new Vector2(0, 0);
        Zoom(4);
    }

    public void CreateChunkImage(Coord chunk)
    {
        if (chunkImages.ContainsKey(chunk))
            return;
        if (!World.chunks.ContainsKey(chunk))
        {
            World.instance.GenerateChunk(new Coord(chunk.x, chunk.y));
        }
        GameObject chunkImageGO = Instantiate(chunkImage, chunkParent);
        chunkImageGO.GetComponent<ChunkImageUI>().Setup(chunk);
        chunkImages.Add(chunk, chunkImageGO);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }
}

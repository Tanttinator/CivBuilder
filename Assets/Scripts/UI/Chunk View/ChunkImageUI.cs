using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChunkImageUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    World world;
    World.TerrainChunk chunk;
    Image image;
    Coord coord;

    public void Setup(Coord chunkCoord)
    {
        coord = chunkCoord;
        image = GetComponent<Image>();
        world = FindObjectOfType<World>();

        chunk = World.chunks[chunkCoord];
        Texture2D texture = World.GetChunkTexture(chunkCoord);
        image.material = new Material(Shader.Find("Unlit/ChunkImage"));
        image.material.mainTexture = texture;

        GetComponent<RectTransform>().anchoredPosition = chunkCoord * (int)GetComponent<RectTransform>().rect.height;
    }

    public void ClaimChunk()
    {
        if (chunk.enemies.Count > 0)
        {
            FindObjectOfType<BattleViewUI>().GetComponent<WindowUI>().Show();
            FindObjectOfType<BattleViewUI>().Setup(chunk);
            return;
        }
        if (!world.TryBuyChunk(coord))
            return;
        World.instance.GenerateChunk(new Coord(coord.x + 1, coord.y));
        World.instance.GenerateChunk(new Coord(coord.x - 1, coord.y));
        World.instance.GenerateChunk(new Coord(coord.x, coord.y + 1));
        World.instance.GenerateChunk(new Coord(coord.x, coord.y - 1));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClaimChunk();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string tooltip = coord.ToString();
        if (chunk.claimed)
            tooltip += "\nClaimed";
        else
        {
            tooltip += "\nCost: " + World.claimedChunks * 100 + " influence.";
            if (chunk.enemies.Count > 0)
                tooltip += "\nEnemy forces detected.";
        }
        UIHandler.DisplayTooltip(tooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIHandler.HideTooltip();
    }
}

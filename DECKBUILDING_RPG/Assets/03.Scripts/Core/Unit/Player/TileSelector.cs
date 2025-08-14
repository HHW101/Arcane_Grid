using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    public HexCoord HexCoord;
    public LayerMask tileLayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, tileLayer);
            if (hit.collider != null && hit.collider.GetComponentInParent<ATile>() is ATile tile)
            {
                Debug.Log("타일 클릭");
                HexCoord = tile.tileData.hexCoord;
                Debug.Log(HexCoord);
              
            }
        }

    }
}

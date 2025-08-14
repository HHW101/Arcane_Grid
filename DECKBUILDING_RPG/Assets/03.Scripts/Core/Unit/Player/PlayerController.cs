using System.Collections;
using UnityEngine;
/*
public class PlayerController : APlayer
{
    public LayerMask tileLayer;
    public float moveSpeed = 3f;
    public int dmg = 10;
    public HexCoord currentCoord; // 현재 플레이어 위치 HexCoord
    public float tileRadius = 1.5f;
    private bool IsMoveMode = false;
    private Vector3 targetPosition;
    private void Awake()
    {
       // playerMovement = GetComponent<PlayerMovementInTile>();
        //if (!playerMovement)
        //{
            Debug.LogError("PlayerMovementInTile 컴포넌트를 찾을 수 없습니다.");
        //}
    }
    private void Start()
    {
        // 초기 위치 정렬 (필요시)
        currentCoord = WorldToHex(transform.position);
        transform.position = ConvertHexToWorld(currentCoord);
    }
    private void Update()
    {
        TileMove();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsMoveMode = true;
        }
    }
    private void TileMove()
    {
        if (Input.GetMouseButtonDown(0) && IsMoveMode)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, tileLayer);
            if (hit.collider != null && hit.collider.GetComponentInParent<ATile>() is ATile tile)
            {
                Debug.Log("타일 클릭");
                HexCoord targetCoord = tile.tileData.hexCoord;
                targetPosition = ConvertHexToWorld(targetCoord);
                StopAllCoroutines();
                StartCoroutine(MovetoPosition(targetCoord));
                IsMoveMode = false;
            }
        }
    }
    private IEnumerator MovetoPosition(HexCoord targetCoord)
    {
        Vector3 targetPos = ConvertHexToWorld(targetCoord);
        while (Vector3.Distance(transform.position, targetPos) >= 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        // 정중앙 위치로 스냅
        transform.position = targetPos;
        // HexCoord 정확히 일치시키기
        currentCoord = targetCoord;
        Debug.Log($"이동 완료 → q:{currentCoord.q}, r:{currentCoord.r}");
    }
    // UI 버튼 연결용
    public void MoveButton()
    {
        IsMoveMode = true;
    }
    // HexCoord → World
    public Vector3 ConvertHexToWorld(HexCoord coord)
    {
        float x = tileRadius * 1.5f * coord.q;
        float y = tileRadius * Mathf.Sqrt(3f) * (coord.r + coord.q * 0.5f);
        return new Vector3(x, y, 0f);
    }
    // World → HexCoord
    public HexCoord WorldToHex(Vector3 pos)
    {
        float qf = pos.x / (tileRadius * 1.5f);
        float rf = (pos.y / (tileRadius * Mathf.Sqrt(3f))) - (qf * 0.5f);
        int q = Mathf.RoundToInt(qf);
        int r = Mathf.RoundToInt(rf);
        return new HexCoord(q, r);
    }
    
    public void BasicAttack() { }
    public bool IsAttackAnimationDone() => true;
}
*/  
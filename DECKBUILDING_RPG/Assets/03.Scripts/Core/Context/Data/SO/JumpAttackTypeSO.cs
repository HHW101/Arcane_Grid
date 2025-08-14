using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attack/Special/JumpAttack")]
public class JumpAttackTypeSO : NPCAttackTypeSO
{
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;

    public override bool UseDefaultEffect => false;

    public override IEnumerator ExecuteAttackCoroutine(ANPC npc, Unit target)
    {
        var stageManager = npc.StageManager;
        if (!stageManager)
        {
            yield break;
        }

        HexCoord npcCoord = npc.npcData.hexCoord;
        HexCoord targetCoord = (target is APlayer p) ? p.playerStateInStage.hexCoord : ((ANPC)target).npcData.hexCoord;

        int distToTarget = npcCoord.Distance(targetCoord);
        if (distToTarget > Mathf.RoundToInt(range))
        {
            yield break;
        }

        if (distToTarget == 1)
        {
            Vector3 pos = npc.transform.position;
            float elapsed = 0f;
            while (elapsed < jumpDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / jumpDuration);
                float height = 4 * jumpHeight * t * (1 - t);
                npc.transform.position = pos + Vector3.up * height;
                yield return null;
            }

            npc.transform.position = pos;

            yield return base.ExecuteAttackCoroutine(npc, target);
            yield break;
        }

        List<HexCoord> neighborCoords = targetCoord.GetNeighbors();
        HexCoord? jumpDest = null;
        float minDist = float.MaxValue;
        foreach (var coord in neighborCoords)
        {
            if (!stageManager.IsTileExist(coord)) continue;
            if (stageManager.IsUnitOnTile(coord)) continue;
            float d = npcCoord.Distance(coord);
            if (!(d < minDist))
            {
                continue;
            }

            minDist = d;
            jumpDest = coord;
        }

        if (jumpDest == null)
        {
            yield break;
        }

        stageManager.FindTileByCoord(npc.npcData.hexCoord).ReactBeforeUnitExitThisTile(npc);

        Vector3 startWorld = npc.transform.position;
        Vector3 endWorld = stageManager.ConvertHexToWorld(jumpDest.Value);
        ATile tile = stageManager.FindTileByCoord(jumpDest.Value);
        endWorld.y += tile?.GetObjectOnTileWorldPositionOffset() ?? 0f;
        npc.npcData.hexCoord = jumpDest.Value;

        float elapsedJump = 0f;
        while (elapsedJump < jumpDuration)
        {
            elapsedJump += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedJump / jumpDuration);
            float height = 4 * jumpHeight * t * (1 - t);
            npc.transform.position = Vector3.Lerp(startWorld, endWorld, t) + Vector3.up * height;
            yield return null;
        }

        npc.transform.position = endWorld;
        tile.ReactAfterUnitEnterThisTile(npc);

        yield return base.ExecuteAttackCoroutine(npc, target);
    }
}
using System.Collections;
using NPCEnum;

public interface INPCState
{
    void Enter(NpcController npcController);
    IEnumerator Execute(NpcController npcController, System.Action<NPCStateResult> onStateSignal);
    void Exit(NpcController npcController);
}

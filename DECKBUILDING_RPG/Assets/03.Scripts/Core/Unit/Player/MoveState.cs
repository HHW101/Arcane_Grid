// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class MoveState : IPlayerState
// {
//     private PlayerController player;
//
//     public MoveState(PlayerController player)
//     {
//         this.player = player;
//     }
//
//     public void Enter() { }
//
//     public void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             player.ChangeState(new AttackState(player));
//         }
//     }
//
//     public void Exit() { }
// }

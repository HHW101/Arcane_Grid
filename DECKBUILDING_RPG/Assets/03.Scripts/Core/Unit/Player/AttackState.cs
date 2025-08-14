// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class AttackState : IPlayerState
// {
//     private PlayerController player;
//
//     public AttackState(PlayerController player) 
//     {
//         this.player = player;
//     }
//
//     public void Enter() 
//     {
//         player.BasicAttack();
//     }
//
//     public void Update() 
//     {
//         if (player.IsAttackAnimationDone())
//         {
//             player.ChangeState(new IdleState(player));
//         }
//     }
//
//     public void Exit() { }
// }

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Scripting.APIUpdating;
//
// public class IdleState : IPlayerState
// {
//     private PlayerController player;
//
//     public IdleState(PlayerController player) 
//     {
//         this.player = player;
//     }
//
//     public void Enter() { }
//
//     public void Update() 
//     {
//         if (Input.GetMouseButtonDown(1)) 
//         {
//             Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//             
//             player.ChangeState(new MoveState(player)); 
//         }
//
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             player.ChangeState(new AttackState(player));
//         }
//     }
//     
//     public void Exit() { }
//
//
// }
//
//

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class ParticleTest : MonoBehaviour
// {
//     public Transform Target; //Ÿ�� ��ġ�� �����Ҽ�����
//     // Start is called before the first frame update
//     void Start()
//     {
//         
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             ParticleManager.Instance.Play(ParticleType.Fireball,Target.position, Quaternion.identity);// �̺κ� �����ذ��� ���ø� �˴ϴ�. ParticleType�ʿ� ��ƼŬ �Ŵ����� enum
//             // ĭ�� �ִ� �͵��� �����ٰ� ���ñ⸸ �Ͻø� �˴ϴ�.   �տ��� ���ϴ� ��ƼŬ����,�ڿ��� ��ƼŬ�� �����Ǵ� ��ġ, ��ƼŬ�� �����Ǵ� ȸ����
//         }
//
//         if (Input.GetKeyDown(KeyCode.A ))
//         {
//            
//             ParticleManager.Instance.Play(ParticleType.Hit, this.transform.position, Quaternion.identity);// ���� ��ġ�� ���� �ʿ䰡 ���� �ϸ� �̰� ������ ����˴ϴ�.
//           
//         }
//     }
// }

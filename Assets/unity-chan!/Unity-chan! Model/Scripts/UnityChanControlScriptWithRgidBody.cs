//
// Mecanim의 애니매이션 데이터 원점으로 이동하지 않은 경우 Rigidbody포함 컨트롤러
// 샘플
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

namespace UnityChan
{
    // 필요한 구성 요소 열기
    [RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]

	public class UnityChanControlScriptWithRgidBody : MonoBehaviour
	{

		public float animSpeed = 1.5f;              // 애니메이션 재생 속도 설정
        public float lookSmoother = 3.0f;			// a smoothing setting for camera motion
		public bool useCurves = true;               // Mecanim에서 커브 조정을 사용하거나 설정

        // 이 스위치가 켜져 있지 않으면 커브가 사용되지 않습니다.
        public float useCurvesHeight = 0.5f;        // 곡선 보정의 유효 높이（지면을 빠져나가기 쉬운 때에는 크게 한다）

        // 이하 캐릭터 컨트롤러용 파라미터
        public float forwardSpeed = 7.0f;   // 전진 속도
        public float backwardSpeed = 2.0f;  // 후퇴 속도
        public float rotateSpeed = 2.0f;    // 선회 속도
        public float jumpPower = 3.0f;      // 점프 위력
        
        private CapsuleCollider col;    // 캐릭터 컨트롤러（캡슐 콜라이더）참조
        private Rigidbody rb; 
        private Vector3 velocity;       // 캐릭터 컨트롤러（캡슐 콜라이더）이동량

        // CapsuleCollider에 설정된 콜라이더의 Height、Center의 초기값을 포함하는 변수
        private float orgColHight;
		private Vector3 orgVectColCenter;
		private Animator anim;                          // 캐릭터에 첨부되는 애니메이터에의 참조
        private AnimatorStateInfo currentBaseState;     // base layer에서 사용되는 애니메이터의 현재 상태 참조

        private GameObject cameraObject;    // 메인 카메라에 대한 참조

        // 애니메이터 각 상태에 대한 참조
        static int idleState = Animator.StringToHash("Base Layer.Idle");
		static int locoState = Animator.StringToHash("Base Layer.Locomotion");
		static int jumpState = Animator.StringToHash("Base Layer.Jump");
		static int restState = Animator.StringToHash("Base Layer.Rest");

        // 초기화
        void Start ()
		{
            anim = GetComponent<Animator>();                      // Animator 구성 요소 얻기
            col = GetComponent<CapsuleCollider>();                // CapsuleCollider구성 요소 얻기（캡슐형 콜리전）
            rb = GetComponent<Rigidbody>();
            cameraObject = GameObject.FindWithTag("MainCamera"); // 메인 카메라 얻기
            // CapsuleCollider 구성 요소 Height、Center의 초기값 저장
            orgColHight = col.height;
			orgVectColCenter = col.center;
		}


        // 이하, 메인 처리.리지드 바디와 얽히기 때문에. FixedUpdate내에서 처리.
        void FixedUpdate ()
		{
			float h = Input.GetAxis("Horizontal");                     // 입력 장치의 수평축을 h로 정의
            float v = Input.GetAxis("Vertical");                       // 입력 장치의 수직축을 v로 정의
            anim.SetFloat("Speed", v);                                 // Animator측면에서 설정 "Speed" 매개 변수에 v를 전달
            anim.SetFloat("Direction", h);                             // Animator측면에서 설정 "Direction" 매개 변수에 h를 전달
            anim.speed = animSpeed;                                     // Animator모션 재생 속도 animSpeed 설정
            currentBaseState = anim.GetCurrentAnimatorStateInfo (0);    // 참조용 상태 변수에 Base Layer (0)의 현재 상태 설정

            rb.useGravity = true;   // 점프 중에 중력을 끊기 때문에 그 이외는 중력의 영향을 받도록 한다.

            // 이하, 캐릭터 이동 처리
            velocity = new Vector3 (0, 0, v);                       // 상하의 키 입력으로부터 Z축 방향의 이동량을 취득
            velocity = transform.TransformDirection (velocity);     // 캐릭터의 로컬 공간에서 방향으로 전환

             // 다음 v의 임계값은 Mecanim 측의 전환과 함께 조정된다.
            if (v > 0.1)
				velocity *= forwardSpeed;       // 이동 속도를 곱
            else if (v < -0.1)
				velocity *= backwardSpeed;      // 이동 속도를 곱

            // 스페이스 키를 입력하면
            if (Input.GetButtonDown("Jump"))
            {
                // 애니메이션의 상태 Locomotion 중간에 점프할 수 있다.
                if (currentBaseState.nameHash == locoState)
                {
					// 상태 전이 중이 아니면 점프할 수 있다.
					if (!anim.IsInTransition (0)) {
						rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
						anim.SetBool("Jump", true);		// Animator로 점프로 전환하는 플래그 보내기
					}
				}
			}

			transform.localPosition += velocity * Time.fixedDeltaTime;  // 위아래 키 입력으로 캐릭터 이동
            transform.Rotate(0, h * rotateSpeed, 0);                   // 좌우의 키 입력으로 캐릭터를 Y축으로 선회

            // 이하, Animator 각 상태에서 처리
            // Locomotion 중간
            // 현재 기본 레이어가 locoState 때
            if (currentBaseState.nameHash == locoState)
            {
				// 커브로 콜라이더 조정을 할때는, 만약을 위해서 리셋한다.
				if (useCurves) resetCollider();
			}
            else if (currentBaseState.nameHash == jumpState)
            {
                // JUMP 내부처리 -> 현재 기본 레이어가jumpState 때

                cameraObject.SendMessage("setCameraPositionJumpView");  // 점프 중인 카메라로 변경
                // 상태가 전환중이 아닌 경우
                if (!anim.IsInTransition(0))
                {
                    // 이하, 커브 조정을 하는 경우의 처리
                    if (useCurves)
                    {
                        // 아래JUMP00 애니매이션에 대한 커브 JumpHeight 그리고 GravityControl
                        // JumpHeight:JUMP00에서 점프 높이（0〜1）
                        // GravityControl: 1 => 점프 중(중력 무효), 0 => 중력 유효
                        float jumpHeight = anim.GetFloat ("JumpHeight");
						float gravityControl = anim.GetFloat ("GravityControl"); 
						if (gravityControl > 0) rb.useGravity = false;  // 점프 중 중력의 영향을 끊다

                        // 레이캐스트를 캐릭터 센터에서 떨어뜨린다.
                        Ray ray = new Ray (transform.position + Vector3.up, -Vector3.up);
						RaycastHit hitInfo = new RaycastHit ();
						// 높이가 useCurvesHeight 이상일때만, 콜라이더의 높이와 중심을 JUMP00 애니매이션에 대한 커브로 조정
						if (Physics.Raycast (ray, out hitInfo))
                        {
							if (hitInfo.distance > useCurvesHeight)
                            {
								col.height = orgColHight - jumpHeight;			    // 조정된 콜라이더의 높이
								float adjCenterY = orgVectColCenter.y + jumpHeight;
								col.center = new Vector3 (0, adjCenterY, 0);        // 조정된 콜라이더의 센터
                            }
                            else
                            {
								resetCollider ();   // 임계값보다 낮을때는 초기값으로 되돌린다.（만약에）					
                            }
						}
					}
									
					anim.SetBool ("Jump", false); // Jump bool값 재설정(반복하지 않도록)
                }
			}
		    else if (currentBaseState.nameHash == idleState)
            {
                // IDLE 내부처리
                // 현재 기본 레이어가 idleState 때
                if (useCurves) resetCollider();     // 커브로 콜라이더 조정을 할때는, 만약을 위해서 리셋한다.
                if (Input.GetButtonDown("Jump")) anim.SetBool("Rest", true);    // 스페이스 키를 입력하면Rest상태가 됨
            }
            else if (currentBaseState.nameHash == restState)
            {
                // REST 내부처리
                // 현재 기본 레이어가 restState 때

                // cameraObject.SendMessage("setCameraPositionFrontView");		// 카메라를 정면으로 전환
                // 상태가 전환중이 아닌 경우 Rest bool값 재설정（반복하지 않도록）
                if (!anim.IsInTransition(0)) anim.SetBool("Rest", false);
			}
		}

		void OnGUI ()
		{
			GUI.Box(new Rect(Screen.width - 260, 10, 250, 150), "Interaction");
			GUI.Label(new Rect(Screen.width - 245, 30, 250, 30), "Up/Down Arrow : Go Forwald/Go Back");
			GUI.Label(new Rect(Screen.width - 245, 50, 250, 30), "Left/Right Arrow : Turn Left/Turn Right");
			GUI.Label(new Rect(Screen.width - 245, 70, 250, 30), "Hit Space key while Running : Jump");
			GUI.Label(new Rect(Screen.width - 245, 90, 250, 30), "Hit Spase key while Stopping : Rest");
			GUI.Label(new Rect(Screen.width - 245, 110, 250, 30), "Left Control : Front Camera");
			GUI.Label(new Rect(Screen.width - 245, 130, 250, 30), "Alt : LookAt Camera");
		}
        
		// 캐릭터의 콜라이더 사이즈 리셋 함수
		void resetCollider ()
		{
			// 컴포넌트의 Height, Center의 초기값을 반환한다.
			col.height = orgColHight;
			col.center = orgVectColCenter;
		}
	}
}
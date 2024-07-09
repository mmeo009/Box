using UnityEngine;
using CatBoxUtils;
public class Timer
{
    private float duration;                 // 동작 간격
    private float remainingTime;            // 남은 시간
    private bool isRunning;                 // 동작 중인지 판단

    public Timer(float duration)            // 클래스 생성자 [클래스가 만들어질 때 초기화]
    {
        this.duration = duration;
        this.remainingTime = duration;
        this.isRunning = false;
    }

    public void Start()                     // 스타트 생명주기에서 사용할 때 동작 시작 해주는 함수
    {
        this.remainingTime = duration;      // 혹시 모르니 한번 더 초기화
        this.isRunning = true;
    }

    public void Update(float deltaTime, Enums.GameSpeed speed)     // Update 함수에서 DeltaTime을 받아온다.
    {
        if (isRunning)                       // 동작 중 일때
        {

            if (speed == Enums.GameSpeed.Default) 
            {
                remainingTime -= deltaTime;
            }
            else if(speed == Enums.GameSpeed.Fast)
            {
                remainingTime -= deltaTime * 2;
            }
            else if (speed == Enums.GameSpeed.Slow)
            {
                remainingTime -= deltaTime / 2;
            }

            if (remainingTime <= 0)          // 시간이 다 소모 되면
            {
                isRunning = false;          // 동작 중지
                remainingTime = 0;          // 남은 시간 0
            }
        }
    }

    public bool IsRunning()                 // 동작 중 확인 함수
    {
        return isRunning;                   // 동작 상태 반환
    }

    public float GetRemainingTime()         // 남아있는 시간 확인 함수
    {
        return remainingTime;               // 시간 상태 반환
    }

    public void Reset()                     // 초기화 시켜주는 함수
    {
        this.remainingTime = duration;
        this.isRunning = false;
    }
}
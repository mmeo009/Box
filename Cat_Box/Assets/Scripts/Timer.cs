using UnityEngine;
using CatBoxUtils;
public class Timer
{
    private float duration;                 // ���� ����
    private float remainingTime;            // ���� �ð�
    private bool isRunning;                 // ���� ������ �Ǵ�

    public Timer(float duration)            // Ŭ���� ������ [Ŭ������ ������� �� �ʱ�ȭ]
    {
        this.duration = duration;
        this.remainingTime = duration;
        this.isRunning = false;
    }

    public void Start()                     // ��ŸƮ �����ֱ⿡�� ����� �� ���� ���� ���ִ� �Լ�
    {
        this.remainingTime = duration;      // Ȥ�� �𸣴� �ѹ� �� �ʱ�ȭ
        this.isRunning = true;
    }

    public void Update(float deltaTime, Enums.GameSpeed speed)     // Update �Լ����� DeltaTime�� �޾ƿ´�.
    {
        if (isRunning)                       // ���� �� �϶�
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

            if (remainingTime <= 0)          // �ð��� �� �Ҹ� �Ǹ�
            {
                isRunning = false;          // ���� ����
                remainingTime = 0;          // ���� �ð� 0
            }
        }
    }

    public bool IsRunning()                 // ���� �� Ȯ�� �Լ�
    {
        return isRunning;                   // ���� ���� ��ȯ
    }

    public float GetRemainingTime()         // �����ִ� �ð� Ȯ�� �Լ�
    {
        return remainingTime;               // �ð� ���� ��ȯ
    }

    public void Reset()                     // �ʱ�ȭ �����ִ� �Լ�
    {
        this.remainingTime = duration;
        this.isRunning = false;
    }
}
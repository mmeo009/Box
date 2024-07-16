using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;
using DG.Tweening;

public class TowerController : MonoBehaviour
{
    public TowerObject towerObject;           // 타워의 기본적인 속성을 가져옴
    public GameObject myTowerGameObject;    // 타워의 외형

    public int towerLevel = 0;          // 타워의 레벨을 가져옴

    public Timer attackRateTimer;       // 타워의 공격 속도를 계산할 타이머

    public List<Transform> enemies;     // 공격할 적들의 리스트 (사정거리 이내의 적들)
    public EnemyController targetEnemy; // 공격할 적

    public bool isDragging = false;     // 드래그 중인지

    public Vector3 origin;              // 원래 포지션 (드래그 실패시 돌아가야 하니까)
    public Vector3 offset;

    private Tweener rotateTweener;
    private void Start()
    {
        OnCreated(towerObject);
    }
    void Update()
    {
        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause) // 게임이 일시정지 상태가 아닐경우
        {
            if(!isDragging)                                         // 드래그 중이 아닐때
            {
                if (rotateTweener != null)
                    rotateTweener.Play();

                float deltaTime = Time.deltaTime;                   // 시간을 가져옴

                if (attackRateTimer != null)                        // 타이머가 존재한다면
                {
                    attackRateTimer.Update(deltaTime, GameManager.instance.gameSpeed);      // 타이머에 시간과 현재 게임메니저의 시간상태를 가져옴

                    if (!attackRateTimer.IsRunning())                   // 타이머가 실행중이지 않을 때
                    {
                        Attack();                                       // 공격을 함
                    }
                }

                if(enemies.Count > 0 && towerObject.towerType != Enums.TowerType.SLOWDOWN)
                {
                    RotateTower(targetEnemy.transform);
                }
            }
            else
            {
                rotateTweener.Pause();
            }
        }
    }
    private void OnMouseDown()
    {
        origin = transform.position;                                // 기존 위치 저장
        offset = gameObject.transform.position - GetWorldPositon(); // offset 위치 가져옴
        isDragging = true;          // 드래그 중 활성화
    }
    private void OnMouseDrag()
    {
        if (isDragging)             // 드래그 중이라면
        {
            transform.position = GetWorldPositon() + offset;            // 마우스 따라다니기
        }
    }
    private void OnMouseUp()
    {
        isDragging = false;                     // 드래그 종료
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.8f);         // 근방의 오브젝트들을 가져옴

        foreach (var coll in colliders)
        {
            if (coll.tag == "Tower" && coll.enabled)     // 타워 테그를 가지고 있을 경우
            {
                var towerController = coll.GetComponent<TowerController>();     // 타워 컨트롤러를 가져옴

                if (towerController != this)                // 나 자신은 아니고
                {
                    if (CheckMerge(towerController))        // 머지 가능한지 확인하고 가능할 경우
                    {
                        towerController.towerLevel++;       // 그 타워의 레벨을 올림
                        towerLevel = 0;                     // 나의 레벨은 초기화
                        PoolManager.Instance.ReturnToPool(this.gameObject); // 이 오브젝트를 제거
                        return;     // 반복문 종료
                    }
                }
            }
        }

        transform.position = origin;       // 머지가 불가능 할경우 기존 위치로 옮김
        origin = Vector3.zero;             // 기존 위치 관련 정보 초기화
    }

    private void OnDrawGizmosSelected()             // 타워 오브젝트를 선택했을 시 나타나는 기즈모
    {
        if (towerObject == null)
        {
            return;
        }

        Gizmos.color = Color.blue;                  // 기즈모를 그릴 색상 파랑
        Gizmos.DrawWireSphere(transform.position, towerObject.tower[towerLevel].baseRange);          // 사정거리를 표시함

        if (enemies.Count > 0)
        {
            if (towerObject.towerType == Enums.TowerType.BASIC)            // 타워의 공격 방식이 기본일 경우
            {
                Gizmos.color = Color.red;           // 빨간색으로
                Transform proximateEnemy = targetEnemy.transform;
                if (proximateEnemy != null)
                    Gizmos.DrawLine(transform.position, proximateEnemy.position);        // 타워로부터 적까지 선을 그림
                Gizmos.DrawSphere(new Vector3(proximateEnemy.position.x, proximateEnemy.position.y + 1, proximateEnemy.position.z), 0.3f);        // 공격 범위 내의 적에게 구를 그림
            }
            else if (towerObject.towerType == Enums.TowerType.SLOWDOWN)   // 타워의 공격 방식이 슬로우일 경우
            {

                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].GetComponent<EnemyController>().moveState == Enums.MoveState.SLOWDOWN)
                    {
                        Gizmos.color = Color.yellow;       // 슬로우 상태일 경우 노란색으로
                    }
                    else
                    {
                        Gizmos.color = Color.cyan;         // 아닐경우 하늘색으로
                    }
                    Gizmos.DrawSphere(new Vector3(enemies[i].transform.position.x, enemies[i].transform.position.y + 1, enemies[i].transform.position.z), 0.3f);        // 공격 범위 내의 적들에게 구를 그림
                }
            }
        }

    }
    public void OnCreated(TowerObject towerObject)
    {
        this.towerObject = towerObject;

        var temp = GameObject.Instantiate(this.towerObject.towerObject);
        temp.transform.parent = this.transform;
        temp.transform.localPosition = new Vector3(0, -0.5f, 0);
        temp.transform.localRotation = Quaternion.identity;

        myTowerGameObject = temp;
        towerLevel = 1;
        enemies.Clear();

        attackRateTimer = new Timer(this.towerObject.tower[towerLevel - 1].baseAttackRate);          // 타워오브젝트의 공격 속도를 가져옴
        attackRateTimer.Start();                                                          // 타이머를 작동시킴
    }
    private void Attack()
    {
        if (towerObject.towerType == Enums.TowerType.BASIC)   // 총알을 발사하는 기본적인 타워일 경우
        {
            if (targetEnemy == null)            // 타겟이 없을 경우
            {
                GetEnemiesTransformInRange();
            }
            else if (!EnemyManager.Instance.IsEnemyActive(targetEnemy) || Vector3.Distance(transform.position, targetEnemy.transform.position) > towerObject.tower[towerLevel - 1].baseRange)
            {                                   // 타겟이 사정거리 밖에 있거나 타겟이 죽었을 경우
                GetEnemiesTransformInRange();
            }

            if(targetEnemy != null)
                FireProjectile(targetEnemy.transform);
        }
        else if (towerObject.towerType == Enums.TowerType.SLOWDOWN)
        {
            GetEnemiesTransformInRange();

            foreach (var enemy in enemies)
            {
                EnemyManager.Instance.GetActiveEnemyControllerByTransfrom(enemy).SlowDown(towerObject.tower[towerLevel - 1].baseDamage, towerObject.tower[towerLevel - 1].baseAttackRate);
            }

            attackRateTimer.Start();                        // 타이머를 초기화 함
        }
    }

    private void GetEnemiesTransformInRange()
    {
        var colliders = Physics.OverlapSphere(transform.position, towerObject.tower[towerLevel - 1].baseRange);         // 사정거리 이내의 모든 오브젝트를 가져옴

        enemies.Clear();                         // 적 리스트 초기화

        foreach (var enemy in colliders)
        {
            if (enemy.tag == "Enemy")            // 테그가 적일 경우
            {
                enemies.Add(enemy.transform);    // 적 리스트에 추가
            }
        }

        targetEnemy = EnemyManager.Instance.GetActiveEnemyControllerByTransfrom(GetProximateEnemy());
    }
    private Transform GetProximateEnemy()
    {
        if (enemies.Count == 0) return null;

        var proximateEnemy = EnemyManager.Instance.GetActiveEnemyControllerByTransfrom(enemies[0]);
        float minDistance = towerObject.tower[towerLevel - 1].baseRange + 1f;

        if(enemies.Count == 1)
        {
            return proximateEnemy.transform;
        }
        
        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);        // 적과 타워의 거리를 계산
            var _enemyController = EnemyManager.Instance.GetActiveEnemyControllerByTransfrom(enemy);

            if(_enemyController != null)
            {
                if (distance < minDistance)             // 적과의 거리를 가장 가까운 거리와 비교하였을때
                {
                    if (_enemyController.nowWayIndex > proximateEnemy.nowWayIndex)                       // 만약 가까운 적이 더 많이 웨이포인트를 지난 친구일 경우
                    {
                        minDistance = distance;                 // 더 적다면 가장 가까운 거리를 지금 적과의 거리로 변경
                        proximateEnemy = _enemyController;      // 지금 적을 가장 가까운 적으로 갱신
                    }
                    else if (_enemyController.nowWayIndex == proximateEnemy.nowWayIndex && _enemyController.ToNextWay() > proximateEnemy.ToNextWay())   // 인덱스가 같고 더 다음 웨이포인트 까지 가까울 경우
                    {
                        minDistance = distance;                 // 더 적다면 가장 가까운 거리를 지금 적과의 거리로 변경
                        proximateEnemy = _enemyController;      // 지금 적을 가장 가까운 적으로 갱신
                    }
                }
            }
        }
        return proximateEnemy.transform;
    }

    private void RotateTower(Transform target)
    {
        if (rotateTweener != null && rotateTweener.IsActive())
            rotateTweener.Kill();
        rotateTweener = transform.DORotateQuaternion(Quaternion.LookRotation(target.position - transform.position), 0.3f);
    }

    private void FireProjectile(Transform target)                                   // 총알 발사
    {
        if(towerObject.towerType == Enums.TowerType.BASIC)                          // 기본 총알의 경우
        {
            if(!PoolManager.Instance.HasThisPool("Bullet"))                         // 만약 풀이 없으면 생성
            {
                PoolManager.Instance.AddNewPool("Bullet", towerObject.bulletObject, 10);
            }
            var bullet = PoolManager.Instance.SpawnFromPool("Bullet", transform.position, transform.rotation);      // 풀에서 총알 생성

            BulletController bulletController;

            if (!bullet.TryGetComponent<BulletController>(out bulletController))            // 총알에 총알 컨트롤러가 있으면 가져옴 없으면 추가
            {
                bulletController = bullet.AddComponent<BulletController>();
            }

            bulletController.myTower = this;                // 총알에 주인 타워를 넣어줌
            bulletController.target = target.position;      // 총알의 타겟을 정해줌
            bullet.transform.LookAt(bulletController.target);   // 총알이 타겟을 바라보게 바꿈

            attackRateTimer.Start();                        // 쿨타임 타이머를 작동시킴
        }
    }

    private bool CheckMerge(TowerController hit)
    {
        if(hit.towerLevel == towerLevel && hit.towerObject == towerObject)      // 나의 타워 레벨과 같고 타워 속성이 같을 경우
        {
            return true;    // 머지 가능함
        }

        return false;       // 머지 불가능함
    }
    private Vector3 GetWorldPositon()
    {
        Vector3 mousePoint = Input.mousePosition;                   // 마우스 위치를 가져옴
        Camera cam = Camera.main;                                   // 카메라는 메인 카메라
        mousePoint.z = cam.WorldToScreenPoint(gameObject.transform.position).z;
        return cam.ScreenToWorldPoint(mousePoint);
    }
    
}

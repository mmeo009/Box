using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;
using DG.Tweening;

public class TowerController : MonoBehaviour
{
    public TowerObject towerObject;           // Ÿ���� �⺻���� �Ӽ��� ������
    public MergeTower merge;                // ���� ���� �Ӽ�
    public GameObject myTowerGameObject;    // Ÿ���� ����

    public int towerLevel = 0;          // Ÿ���� ������ ������

    public Timer attackRateTimer;       // Ÿ���� ���� �ӵ��� ����� Ÿ�̸�

    public List<Transform> enemies;     // ������ ������ ����Ʈ (�����Ÿ� �̳��� ����)
    public EnemyController targetEnemy; // ������ ��

    private Tweener rotateTweener;
    private void Start()
    {
        if(merge == null)
        {
            if(!TryGetComponent<MergeTower>(out merge))
            {
                merge = gameObject.AddComponent<MergeTower>();
            }
        }
        merge.tower = this;
    }
    void Update()
    {
        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause) // ������ �Ͻ����� ���°� �ƴҰ��
        {
            if(!merge.isDragging)                                         // �巡�� ���� �ƴҶ�
            {
                if (rotateTweener != null)
                    rotateTweener.Play();

                float deltaTime = Time.deltaTime;                   // �ð��� ������

                if (attackRateTimer != null)                        // Ÿ�̸Ӱ� �����Ѵٸ�
                {
                    attackRateTimer.Update(deltaTime, GameManager.instance.gameSpeed);      // Ÿ�̸ӿ� �ð��� ���� ���Ӹ޴����� �ð����¸� ������

                    if (!attackRateTimer.IsRunning())                   // Ÿ�̸Ӱ� ���������� ���� ��
                    {
                        Attack();                                       // ������ ��
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

    private void OnDrawGizmosSelected()             // Ÿ�� ������Ʈ�� �������� �� ��Ÿ���� �����
    {
        if (towerObject == null)
        {
            return;
        }

        Gizmos.color = Color.blue;                  // ����� �׸� ���� �Ķ�
        Gizmos.DrawWireSphere(transform.position, towerObject.tower[towerLevel].baseRange);          // �����Ÿ��� ǥ����

        if (enemies.Count > 0)
        {
            if (towerObject.towerType == Enums.TowerType.BASIC)            // Ÿ���� ���� ����� �⺻�� ���
            {
                Gizmos.color = Color.red;           // ����������
                Transform proximateEnemy = targetEnemy.transform;

                if (proximateEnemy != null)
                {
                    Gizmos.DrawLine(transform.position, proximateEnemy.position);        // Ÿ���κ��� ������ ���� �׸�
                    Gizmos.DrawSphere(new Vector3(proximateEnemy.position.x, proximateEnemy.position.y + 1, proximateEnemy.position.z), 0.3f);        // ���� ���� ���� ������ ���� �׸�
                }
            }
            else if (towerObject.towerType == Enums.TowerType.SLOWDOWN)   // Ÿ���� ���� ����� ���ο��� ���
            {

                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].GetComponent<EnemyController>().moveState == Enums.MoveState.SLOWDOWN)
                    {
                        Gizmos.color = Color.yellow;       // ���ο� ������ ��� ���������
                    }
                    else
                    {
                        Gizmos.color = Color.cyan;         // �ƴҰ�� �ϴû�����
                    }
                    Gizmos.DrawSphere(new Vector3(enemies[i].transform.position.x, enemies[i].transform.position.y + 1, enemies[i].transform.position.z), 0.3f);        // ���� ���� ���� ���鿡�� ���� �׸�
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

        attackRateTimer = new Timer(this.towerObject.tower[towerLevel - 1].baseAttackRate);          // Ÿ��������Ʈ�� ���� �ӵ��� ������
        attackRateTimer.Start();                                                          // Ÿ�̸Ӹ� �۵���Ŵ
    }
    private void Attack()
    {
        if (towerObject.towerType == Enums.TowerType.BASIC)   // �Ѿ��� �߻��ϴ� �⺻���� Ÿ���� ���
        {
            if (targetEnemy == null)            // Ÿ���� ���� ���
            {
                GetEnemiesTransformInRange();
            }
            else if (!EnemyManager.Instance.IsEnemyActive(targetEnemy) || Vector3.Distance(transform.position, targetEnemy.transform.position) > towerObject.tower[towerLevel - 1].baseRange)
            {                                   // Ÿ���� �����Ÿ� �ۿ� �ְų� Ÿ���� �׾��� ���
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

            attackRateTimer.Start();                        // Ÿ�̸Ӹ� �ʱ�ȭ ��
        }
    }

    private void GetEnemiesTransformInRange()
    {
        var colliders = Physics.OverlapSphere(transform.position, towerObject.tower[towerLevel - 1].baseRange);         // �����Ÿ� �̳��� ��� ������Ʈ�� ������

        enemies.Clear();                         // �� ����Ʈ �ʱ�ȭ

        foreach (var enemy in colliders)
        {
            if (enemy.tag == "Enemy")            // �ױװ� ���� ���
            {
                enemies.Add(enemy.transform);    // �� ����Ʈ�� �߰�
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
            float distance = Vector3.Distance(transform.position, enemy.transform.position);        // ���� Ÿ���� �Ÿ��� ���
            var _enemyController = EnemyManager.Instance.GetActiveEnemyControllerByTransfrom(enemy);

            if(_enemyController != null)
            {
                if (distance < minDistance)             // ������ �Ÿ��� ���� ����� �Ÿ��� ���Ͽ�����
                {
                    if (_enemyController.nowWayIndex > proximateEnemy.nowWayIndex)                       // ���� ����� ���� �� ���� ��������Ʈ�� ���� ģ���� ���
                    {
                        minDistance = distance;                 // �� ���ٸ� ���� ����� �Ÿ��� ���� ������ �Ÿ��� ����
                        proximateEnemy = _enemyController;      // ���� ���� ���� ����� ������ ����
                    }
                    else if (_enemyController.nowWayIndex == proximateEnemy.nowWayIndex && _enemyController.ToNextWay() > proximateEnemy.ToNextWay())   // �ε����� ���� �� ���� ��������Ʈ ���� ����� ���
                    {
                        minDistance = distance;                 // �� ���ٸ� ���� ����� �Ÿ��� ���� ������ �Ÿ��� ����
                        proximateEnemy = _enemyController;      // ���� ���� ���� ����� ������ ����
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
        rotateTweener = transform.DORotateQuaternion(Quaternion.LookRotation(new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position), 0.3f);
    }

    private void FireProjectile(Transform target)                                   // �Ѿ� �߻�
    {
        if(towerObject.towerType == Enums.TowerType.BASIC)                          // �⺻ �Ѿ��� ���
        {
            if(!PoolManager.Instance.HasThisPool(towerObject.bulletObject.name))                         // ���� Ǯ�� ������ ����
            {
                PoolManager.Instance.AddNewPool(towerObject.bulletObject.name, towerObject.bulletObject, 10);
            }
            var bullet = PoolManager.Instance.SpawnFromPool(towerObject.bulletObject.name, transform.position, transform.rotation);      // Ǯ���� �Ѿ� ����

            BulletController bulletController;

            if (!bullet.TryGetComponent<BulletController>(out bulletController))            // �Ѿ˿� �Ѿ� ��Ʈ�ѷ��� ������ ������ ������ �߰�
            {
                bulletController = bullet.AddComponent<BulletController>();
            }

            bulletController.myTower = this;                // �Ѿ˿� ���� Ÿ���� �־���
            bulletController.target = target.position;      // �Ѿ��� Ÿ���� ������
            bullet.transform.LookAt(bulletController.target);   // �Ѿ��� Ÿ���� �ٶ󺸰� �ٲ�

            attackRateTimer.Start();                        // ��Ÿ�� Ÿ�̸Ӹ� �۵���Ŵ
        }
    }
    
}

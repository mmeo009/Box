using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;
using DG.Tweening;

public class TowerController : MonoBehaviour
{
    public TowerObject towerObject;           // Ÿ���� �⺻���� �Ӽ��� ������
    public GameObject myTowerGameObject;    // Ÿ���� ����

    public int towerLevel = 0;          // Ÿ���� ������ ������

    public Timer attackRateTimer;       // Ÿ���� ���� �ӵ��� ����� Ÿ�̸�

    public List<Transform> enemies;      // ������ ������ ����Ʈ (�����Ÿ� �̳��� ����)

    public bool isDragging = false;     // �巡�� ������

    public Vector3 origin;              // ���� ������ (�巡�� ���н� ���ư��� �ϴϱ�)
    public Vector3 offset;

    private Tweener rotateTweener;
    private void Start()
    {
        OnCreated(towerObject);
    }
    void Update()
    {
        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause) // ������ �Ͻ����� ���°� �ƴҰ��
        {
            if(!isDragging)                                         // �巡�� ���� �ƴҶ�
            {
                GetEnemiesTransformInRange();

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
                    RotateTower(GetProximateEnemy());
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
        origin = transform.position;                                // ���� ��ġ ����
        offset = gameObject.transform.position - GetWorldPositon(); // offset ��ġ ������
        isDragging = true;          // �巡�� �� Ȱ��ȭ
    }
    private void OnMouseDrag()
    {
        if (isDragging)             // �巡�� ���̶��
        {
            transform.position = GetWorldPositon() + offset;            // ���콺 ����ٴϱ�
        }
    }
    private void OnMouseUp()
    {
        isDragging = false;                     // �巡�� ����
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.8f);         // �ٹ��� ������Ʈ���� ������

        foreach (var coll in colliders)
        {
            if (coll.tag == "Tower" && coll.enabled)     // Ÿ�� �ױ׸� ������ ���� ���
            {
                var towerController = coll.GetComponent<TowerController>();     // Ÿ�� ��Ʈ�ѷ��� ������

                if (towerController != this)                // �� �ڽ��� �ƴϰ�
                {
                    if (CheckMerge(towerController))        // ���� �������� Ȯ���ϰ� ������ ���
                    {
                        towerController.towerLevel++;       // �� Ÿ���� ������ �ø�
                        towerLevel = 0;                     // ���� ������ �ʱ�ȭ
                        PoolManager.Instance.ReturnToPool(this.gameObject); // �� ������Ʈ�� ����
                        return;     // �ݺ��� ����
                    }
                }
            }
        }

        transform.position = origin;       // ������ �Ұ��� �Ұ�� ���� ��ġ�� �ű�
        origin = Vector3.zero;             // ���� ��ġ ���� ���� �ʱ�ȭ
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
        if(enemies.Count > 0)                                // ����Ʈ�� ���� �Ѱ� �̻��� ���
        {
            if (towerObject.towerType == Enums.TowerType.BASIC)   // �Ѿ��� �߻��ϴ� �⺻���� Ÿ���� ���
            {
                FireProjectile(GetProximateEnemy());
            }
            else if(towerObject.towerType == Enums.TowerType.SLOWDOWN)
            {
                foreach(var enemy in enemies)
                {
                    enemy.GetComponent<EnemyController>().SlowDown(towerObject.tower[towerLevel].baseDamage, towerObject.tower[towerLevel].baseAttackRate);
                }
            }
        }
        else
        {
            return;
        }

        attackRateTimer.Start();                        // Ÿ�̸Ӹ� �ʱ�ȭ ��
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
    }
    private Transform GetProximateEnemy()
    {
        if (enemies.Count == 0) return null;

        var proximateEnemy = enemies[0].GetComponent<EnemyController>();
        float minDistance = towerObject.tower[towerLevel - 1].baseRange + 1f;

        if(enemies.Count == 1)
        {
            return proximateEnemy.transform;
        }
        
        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);        // ���� Ÿ���� �Ÿ��� ���
            var _enemyController = enemy.GetComponent<EnemyController>();

            if (distance < minDistance)             // ������ �Ÿ��� ���� ����� �Ÿ��� ���Ͽ�����
            {
                if(_enemyController.nowWayIndex > proximateEnemy.nowWayIndex)                       // ���� ����� ���� �� ���� ��������Ʈ�� ���� ģ���� ���
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
        return proximateEnemy.transform;
    }

    private void RotateTower(Transform target)
    {
        if (rotateTweener != null && rotateTweener.IsActive())
            rotateTweener.Kill();
        rotateTweener = transform.DORotateQuaternion(Quaternion.LookRotation(target.position - transform.position), 0.3f);
    }

    private void FireProjectile(Transform target)
    {
        if(towerObject.towerType == Enums.TowerType.BASIC)
        {
            if(!PoolManager.Instance.HasThisPool("Bullet"))
            {
                PoolManager.Instance.AddNewPool("Bullet", towerObject.bulletObject, 10);
            }
            var bullet = PoolManager.Instance.SpawnFromPool("Bullet", transform.position, transform.rotation);

            BulletController bulletController;

            if (!bullet.TryGetComponent<BulletController>(out bulletController))
            {
                bulletController = bullet.AddComponent<BulletController>();
            }

            bulletController.myTower = this;
            bulletController.target = target.position;
            bullet.transform.LookAt(bulletController.target);

            attackRateTimer.Start();
        }
    }

    private bool CheckMerge(TowerController hit)
    {
        if(hit.towerLevel == towerLevel && hit.towerObject == towerObject)      // ���� Ÿ�� ������ ���� Ÿ�� �Ӽ��� ���� ���
        {
            return true;    // ���� ������
        }

        return false;       // ���� �Ұ�����
    }
    private Vector3 GetWorldPositon()
    {
        Vector3 mousePoint = Input.mousePosition;                   // ���콺 ��ġ�� ������
        Camera cam = Camera.main;                                   // ī�޶�� ���� ī�޶�
        mousePoint.z = cam.WorldToScreenPoint(gameObject.transform.position).z;
        return cam.ScreenToWorldPoint(mousePoint);
    }
    private void OnDrawGizmosSelected()             // Ÿ�� ������Ʈ�� �������� �� ��Ÿ���� �����
    {
        if(towerObject == null)
        {
            return;
        }

        Gizmos.color = Color.blue;                  // ����� �׸� ���� �Ķ�
        Gizmos.DrawWireSphere(transform.position, towerObject.tower[towerLevel].baseRange);          // �����Ÿ��� ǥ����

        if (enemies.Count > 0)
        {
            if(towerObject.towerType == Enums.TowerType.BASIC)            // Ÿ���� ���� ����� �⺻�� ���
            {
                Gizmos.color = Color.red;           // ����������
                Transform proximateEnemy = GetProximateEnemy();
                if(proximateEnemy != null)
                Gizmos.DrawLine(transform.position, proximateEnemy.position);        // Ÿ���κ��� ������ ���� �׸�
                Gizmos.DrawSphere(new Vector3(proximateEnemy.position.x, proximateEnemy.position.y + 1, proximateEnemy.position.z), 0.3f);        // ���� ���� ���� ������ ���� �׸�
            }
            else if(towerObject.towerType == Enums.TowerType.SLOWDOWN)   // Ÿ���� ���� ����� ���ο��� ���
            {

                for(int i = 0; i < enemies.Count; i ++)
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
}

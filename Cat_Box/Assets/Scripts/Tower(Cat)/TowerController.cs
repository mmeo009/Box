using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class TowerController : MonoBehaviour
{
    public TowerObject tower;           // Ÿ���� �⺻���� �Ӽ��� ������
    public GameObject myTowerObject;    // Ÿ���� ����

    public int towerLevel = 0;          // Ÿ���� ������ ������

    public Timer attackRateTimer;       // Ÿ���� ���� �ӵ��� ����� Ÿ�̸�

    public List<Transform> enemys;      // ������ ������ ����Ʈ (�����Ÿ� �̳��� ����)

    public bool isDragging = false;     // �巡�� ������

    public Vector3 origin;              // ���� ������ (�巡�� ���н� ���ư��� �ϴϱ�)

    public Vector3 offset;
    private void Start()
    {
        OnCreated(tower);
    }
    void Update()
    {
        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause) // ������ �Ͻ����� ���°� �ƴҰ��
        {
            if(!isDragging)                                         // �巡�� ���� �ƴҶ�
            {
                float deltaTime = Time.deltaTime;                   // �ð��� ������

                if (attackRateTimer != null)                        // Ÿ�̸Ӱ� �����Ѵٸ�
                {

                    attackRateTimer.Update(deltaTime, GameManager.instance.gameSpeed);      // Ÿ�̸ӿ� �ð��� ���� ���Ӹ޴����� �ð����¸� ������

                    if (!attackRateTimer.IsRunning())                   // Ÿ�̸Ӱ� ���������� ���� ��
                    {
                        Attack();                                       // ������ ��
                    }
                }
            }
        }
    }
    public void OnCreated(TowerObject towerObject)
    {
        tower = towerObject;

        var temp = GameObject.Instantiate(tower.towerObject);
        temp.transform.parent = this.transform;
        temp.transform.localPosition = Vector3.zero;
        temp.transform.localRotation = Quaternion.identity;

        myTowerObject = temp;
        towerLevel = 1;
        enemys.Clear();

        attackRateTimer = new Timer(tower.tower[towerLevel - 1].baseAttackRate);          // Ÿ��������Ʈ�� ���� �ӵ��� ������
        attackRateTimer.Start();                                                          // Ÿ�̸Ӹ� �۵���Ŵ
    }
    private void Attack()
    {
        var colliders = Physics.OverlapSphere(transform.position, tower.tower[towerLevel - 1].baseRange);         // �����Ÿ� �̳��� ��� ������Ʈ�� ������

        enemys.Clear();                         // �� ����Ʈ �ʱ�ȭ

        foreach(var enemy in colliders)
        {
            if(enemy.tag == "Enemy")            // �ױװ� ���� ���
            {
                enemys.Add(enemy.transform);    // �� ����Ʈ�� �߰�
            }
        }

        if(enemys.Count > 0)                                // ����Ʈ�� ���� �Ѱ� �̻��� ���
        {
            if (tower.towerType == Enums.TowerType.BASIC)   // �Ѿ��� �߻��ϴ� �⺻���� Ÿ���� ���
            {
                FireProjectile(GetProximateEnemy());
            }
            else if(tower.towerType == Enums.TowerType.SLOWDOWN)
            {
                foreach(var enemy in enemys)
                {
                    enemy.GetComponent<EnemyController>().SlowDown(tower.tower[towerLevel].baseDamage, tower.tower[towerLevel].baseAttackRate);
                }
            }
        }
        else
        {
            return;
        }

        attackRateTimer.Start();                        // Ÿ�̸Ӹ� �ʱ�ȭ ��
    }
    private Transform GetProximateEnemy()
    {
        if (enemys.Count == 0) return null;

        Transform proximateEnemy = enemys[0].transform;
        float minDistance = tower.tower[towerLevel - 1].baseRange + 1f;

        if(enemys.Count == 1)
        {
            return proximateEnemy;
        }
        
        foreach (var enemy in enemys)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);        // ���� Ÿ���� �Ÿ��� ���

            if (distance < minDistance)             // ������ �Ÿ��� ���� ����� �Ÿ��� ���Ͽ�����
            {
                minDistance = distance;             // �� ���ٸ� ���� ����� �Ÿ��� ���� ������ �Ÿ��� ����
                proximateEnemy = enemy.transform;      // ���� ���� ���� ����� ������ ����
            }
        }
        return proximateEnemy;
    }

    private void FireProjectile(Transform target)
    {
        if(tower.towerType == Enums.TowerType.BASIC)
        {
            transform.LookAt(target);

            var bullet = PoolManager.Instance.SpawnFromPool("Bullet", transform.position, transform.rotation);
            bullet.GetComponent<BulletController>().myTower = this;
            bullet.GetComponent<BulletController>().target = target.position;
            bullet.transform.LookAt(bullet.GetComponent<BulletController>().target);

            attackRateTimer.Start();
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

        foreach(var coll in colliders)
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
    private bool CheckMerge(TowerController hit)
    {
        if(hit.towerLevel == towerLevel && hit.tower == tower)      // ���� Ÿ�� ������ ���� Ÿ�� �Ӽ��� ���� ���
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
        if(tower == null)
        {
            return;
        }

        Gizmos.color = Color.blue;                  // ����� �׸� ���� �Ķ�
        Gizmos.DrawWireSphere(transform.position, tower.tower[towerLevel].baseRange);          // �����Ÿ��� ǥ����

        if (enemys.Count > 0)
        {
            if(tower.towerType == Enums.TowerType.BASIC)            // Ÿ���� ���� ����� �⺻�� ���
            {
                Gizmos.color = Color.red;           // ����������
                Transform proximateEnemy = GetProximateEnemy();
                if(proximateEnemy != null)
                Gizmos.DrawLine(transform.position, proximateEnemy.position);        // Ÿ���κ��� ������ ���� �׸�
            }
            else if(tower.towerType == Enums.TowerType.SLOWDOWN)   // Ÿ���� ���� ����� ���ο��� ���
            {

                for(int i = 0; i < enemys.Count; i ++)
                {
                    if (enemys[i].GetComponent<EnemyController>().moveState == Enums.MoveState.SLOWDOWN)
                    {
                        Gizmos.color = Color.yellow;       // ���ο� ������ ��� ���������
                    }
                    else
                    {
                        Gizmos.color = Color.cyan;         // �ƴҰ�� �ϴû�����
                    }
                    Gizmos.DrawSphere(enemys[i].transform.position, 1f);        // ���� ���� ���� ���鿡�� ���� �׸�
                }
            }
        }

    }
}

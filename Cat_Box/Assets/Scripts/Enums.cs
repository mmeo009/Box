namespace CatBoxUtils
{
    public class Enums
    {
        public enum GameState       // ������ ����
        {
            MAIN,
            SHOP,
            GAMEPLAY,
            GAMESTOP,
            GAMEEXIT
        }

        public enum GameSpeed       // ������ �ӵ�
        {
            Pause = 0,
            Default,                // 1���
            Slow,                   // 0.5���
            Fast                    // 2���
        }

        public enum TowerType
        {
            BASIC,
            SNIPER,
            SLOWDOWN,
            BOMB
        }

        public enum EnemyType
        {
            BASIC,
            ENCOURAGING
        }

        public enum MoveState
        {
            DEFAULT,
            MOVE,
            STUN,
            SLOWDOWN,
            ARRIVE
        }
        public enum ButtonType
        {
            TOWER_STORE,
            TOWER_INGAME,
            SETTING,
            INFO,
            BACK,
            RESTART
        }
        public enum TowerStatType
        {
            DAMAGE,
            RANGE,
            ATTACKRATE,
            BULLETSPEED
        }
        public enum MoneyType
        {
            INGAME,
            INSTORE
        }
    }
}


namespace CatBoxUtils
{
    public class Enums
    {
        public enum GameState       // ������ ����
        {
            MAINORSHOP,
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
        public enum TowerButtonType
        {
            Store,
            InGame
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


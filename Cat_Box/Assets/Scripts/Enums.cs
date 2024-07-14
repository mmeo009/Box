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
    }
}


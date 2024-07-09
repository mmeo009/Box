namespace CatBoxUtils
{
    public class Enums
    {
        public enum GameState       // ������ ����
        {
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
    }
}


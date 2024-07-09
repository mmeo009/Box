namespace CatBoxUtils
{
    public class Enums
    {
        public enum GameState       // 게임의 상태
        {
            GAMEPLAY,
            GAMESTOP,
            GAMEEXIT
        }

        public enum GameSpeed       // 게임의 속도
        {
            Pause = 0,
            Default,                // 1배속
            Slow,                   // 0.5배속
            Fast                    // 2배속
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


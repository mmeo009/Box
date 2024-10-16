namespace CatBoxUtils
{
    public class Enums
    {
        public enum GameState       // 게임의 상태
        {
            MAIN,
            SHOP,
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


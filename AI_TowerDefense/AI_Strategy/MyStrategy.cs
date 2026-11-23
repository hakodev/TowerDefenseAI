using System.Collections.Generic;
using GameFramework;

namespace AI_Strategy {
    public class MyStrategy : AbstractStrategy {
        private int turn = 0;
        private bool isAttacking = false;

        public MyStrategy(Player player) : base(player) {}

        public override void DeployTowers() {
            turn++;

            if(turn == 1) {
                //Two towers cover the entire width of the lane [(2, 3), (4, 3)] for 5 gold
                player.TryBuyTower<MyTower>(2, 3);
                player.TryBuyTower<MyTower>(4, 3);
                return;
            }
        }

        public override void DeploySoldiers() {}

        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList) {
            return unsortedList;
        }

        public override List<Tower> SortedTowerArray(List<Tower> unsortedList) {
            return unsortedList;
        }
    }
}

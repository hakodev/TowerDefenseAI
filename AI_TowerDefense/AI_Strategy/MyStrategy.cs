using System.Collections.Generic;
using GameFramework;

namespace AI_Strategy {
    public class MyStrategy : AbstractStrategy {
        private int turn = 0;
        private bool isAttacking = false;

        public MyStrategy(Player player) : base(player) {}

        public override void DeployTowers() {}

        public override void DeploySoldiers() {}

        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList) {
            return unsortedList;
        }

        public override List<Tower> SortedTowerArray(List<Tower> unsortedList) {
            return unsortedList;
        }
    }
}

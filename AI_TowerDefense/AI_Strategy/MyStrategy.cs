using System.Collections.Generic;
using GameFramework;

namespace AI_Strategy {
    public class MyStrategy : AbstractStrategy {
        private enum StrategyState {
            Opening,
            Saving,
            Attacking
        }

        private int turn = 0;
        private StrategyState state = StrategyState.Opening;

        public MyStrategy(Player player) : base(player) {}

        public override void DeployTowers() {
            turn++;

            if(state == StrategyState.Opening) {
                //Two towers cover the entire width of the lane [(2, 3), (4, 3)] for 5 gold
                player.TryBuyTower<MyTower>(2, 3);
                player.TryBuyTower<MyTower>(4, 3);
                state = StrategyState.Saving;
                return;
            }

            //Because a game ends after 1000 turns and a lane is 20 rows tall,
            //stop wasting gold on soldiers after turn 980 if no enemies are on our lane
            if(turn > 980 && player.HomeLane.SoldierCount() == 0) return;

            //Build a tower if under attack with too few towers, or if we have plenty of spare gold
            bool underPressure = player.HomeLane.SoldierCount() > player.HomeLane.TowerCount();
            bool canFortify = player.HomeLane.TowerCount() < 5 && player.Gold >= Tower.GetNextTowerCosts(player.HomeLane);

            if(!underPressure && !canFortify) return;
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

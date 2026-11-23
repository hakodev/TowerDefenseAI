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

        private const int MaxTurns = 1000;

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
            if(turn > MaxTurns - 20 && player.HomeLane.SoldierCount() == 0) return;

            //Cut idea!: Build a tower if under attack with too few towers, or if we have plenty of spare gold
            //bool underPressure = player.HomeLane.SoldierCount() > player.HomeLane.TowerCount();
            //bool canFortify = player.HomeLane.TowerCount() < 5 && player.Gold >= Tower.GetNextTowerCosts(player.HomeLane);
            //if(!underPressure && !canFortify) return;

            //Build up to 2 towers per turn if a heavy rush hits us
            for(int i = 0; i < 2; i++) {
                int nextTowerCost = Tower.GetNextTowerCosts(player.HomeLane);
                if(player.Gold < nextTowerCost) break;

                if(!ShouldBuildTower(nextTowerCost)) break;
            }
        }

        public override void DeploySoldiers() {}

        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList) {
            return unsortedList;
        }

        public override List<Tower> SortedTowerArray(List<Tower> unsortedList) {
            return unsortedList;
        }

        private bool ShouldBuildTower(int nextTowerCost) {
            //If we have enough gold to build a tower, and we have less than 5 towers, build a tower
            //return player.Gold >= nextTowerCost && player.HomeLane.TowerCount() < 5;

            int towerCount = player.HomeLane.TowerCount();
            int enemyCount = player.HomeLane.SoldierCount();

            if(towerCount < 2) return true; //Always keep at least 2 towers standing

            if(enemyCount >= towerCount) return true; //Build more towers if we are under pressure

            if(towerCount < 5 && player.Gold >= nextTowerCost * 2) return true; //Build more towers if we have less than 5 towers and plenty of gold

            return false;
        }

        private bool TryFindBestTowerSpot(out int bestX, out int bestY) {
            bestX = -1;
            bestY = -1;
            int bestScore = int.MinValue;

            for(int x = 0; x < PlayerLane.WIDTH; x++) {
                for(int y = PlayerLane.HEIGHT_OF_SAFETY_ZONE; y < PlayerLane.HEIGHT; y++) {
                    if(player.HomeLane.GetCellAt(x, y)?.Unit != null) continue;

                    //Rules forbid placing a tower directly next to another tower horizontally/vertically
                    if(player.CheckAdjacentCellsAny(player.HomeLane, x, y, cell => cell?.Unit is Tower)) continue;
                }
            }

            return false;
        }
    }
}

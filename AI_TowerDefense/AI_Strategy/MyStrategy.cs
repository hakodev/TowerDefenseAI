using System.Collections.Generic;
using GameFramework;

namespace AI_Strategy {
    public class MyStrategy : AbstractStrategy {
        private enum StrategyState {
            Opening,
            Saving,
            Attacking
        }

        private StrategyState state = StrategyState.Opening;

        public MyStrategy(Player player) : base(player) {}

        public override void DeployTowers() {
            if(state == StrategyState.Opening) {
                //Two towers cover the entire width of the lane [(2, 3), (4, 3)] for 5 gold
                player.TryBuyTower<MyTower>(2, 3);
                player.TryBuyTower<MyTower>(4, 3);
                state = StrategyState.Saving;
                return;
            }

            int nextTowerCost = Tower.GetNextTowerCosts(player.HomeLane);

            if(player.Gold < nextTowerCost) return;
            if(!ShouldBuildTower(nextTowerCost)) return;

            if(TryFindBestTowerSpot(out int bestX, out int bestY)) {
                player.TryBuyTower<MyTower>(bestX, bestY);
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
            //Scan row by row from front to back in a checkerboard across the inner columns
            for(int y = PlayerLane.HEIGHT_OF_SAFETY_ZONE; y < PlayerLane.HEIGHT; y++) {
                for(int x = 1; x < PlayerLane.WIDTH - 1; x++) {
                    if((x + y) % 2 == 1 && CanPlaceTowerAt(x, y)) {
                        bestX = x;
                        bestY = y;
                        return true;
                    }
                }
            }

            bestX = -1;
            bestY = -1;
            return false;
        }

        private bool CanPlaceTowerAt(int x, int y) {
            Cell cell = player.HomeLane.GetCellAt(x, y);
            if(cell == null || cell.Unit != null) return false;

            //Rules forbid placing a tower directly next to another tower horizontally/vertically
            return !player.CheckAdjacentCellsAny(player.HomeLane, x, y, neighborCell => neighborCell?.Unit is Tower);
        }
    }
}

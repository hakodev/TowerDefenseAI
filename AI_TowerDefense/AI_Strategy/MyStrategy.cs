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

        private const int MinTowerCount = 2;
        private const int TargetTowerCount = 5;
        private const int BaselineSoldierCount = 6;
        private const int SoldierCost = 2;

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

        public override void DeploySoldiers() {
            int goldGoal = (BaselineSoldierCount + player.EnemyLane.TowerCount()) * SoldierCost;

            if(state == StrategyState.Saving && player.Gold >= goldGoal) {
                state = StrategyState.Attacking;
            }

            if(state == StrategyState.Attacking) {
                while(player.Gold >= SoldierCost) {
                    int bestX = FindBestSoldierX();
                    if(bestX < 0) break;

                    if(player.TryBuySoldier<MySoldier>(bestX) != Player.SoldierPlacementResult.Success) break;
                }

                //Switch back to Saving once we can't afford another soldier
                if(player.Gold < SoldierCost) {
                    state = StrategyState.Saving;
                }
            }
        }

        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList) {
            List<Soldier> sortedList = new(unsortedList);
            sortedList.Sort((a, b) => b.PosY.CompareTo(a.PosY));
            return sortedList;
        }

        public override List<Tower> SortedTowerArray(List<Tower> unsortedList) {
            List<Tower> sortedList = new(unsortedList);
            sortedList.Sort((a, b) => b.PosY.CompareTo(a.PosY));
            return sortedList;
        }

        private bool ShouldBuildTower(int nextTowerCost) {
            int towerCount = player.HomeLane.TowerCount();
            int enemyCount = player.HomeLane.SoldierCount();

            if(towerCount < MinTowerCount) return true; //Always keep at least 2 towers standing
            if(enemyCount >= towerCount) return true; //Build more towers if we are under pressure
            if(towerCount < TargetTowerCount && player.Gold >= nextTowerCost * 2) return true; //Build more towers if we have less than 5 towers and plenty of gold

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

        private int FindBestSoldierX() {
            //Find the column with the fewest enemy soldiers
            int bestX = -1;
            int fewestTowers = int.MaxValue;

            for(int x = 0; x < PlayerLane.WIDTH; x++) {
                if(player.EnemyLane.GetCellAt(x, 0)?.Unit != null) continue;

                int towersInColumnCount = 0;
                for(int y = 0; y < PlayerLane.HEIGHT; y++) {
                    if(player.EnemyLane.GetCellAt(x, y)?.Unit is Tower) towersInColumnCount++;
                }

                if(towersInColumnCount < fewestTowers) {
                    fewestTowers = towersInColumnCount;
                    bestX = x;
                }
            }

            return bestX;
        }
    }
}

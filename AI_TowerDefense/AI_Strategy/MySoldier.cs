using System.Collections.Generic;
using GameFramework;

namespace AI_Strategy {
    public class MySoldier : Soldier
    {
        private const int TowerRange = 2;
        private const int ForwardWeight = 10;
        private const int ThreatPenalty = 6;

        protected override List<Unit> SortTargetsInRange(List<Unit> targets) {
            targets.Sort((a, b) => a.Health.CompareTo(b.Health));
            return targets;
        }

        public override void Move() {
            if(speed <= 0 || posY >= PlayerLane.HEIGHT) return;

            if(posY == PlayerLane.HEIGHT - 1) {
                //Step off the last row to score immediately
                MoveTo(posX, PlayerLane.HEIGHT);
                return;
            }

            int bestX = -1;
            int bestY = -1;
            int bestScore = int.MinValue;

            //Evaluate all eight neighboring cells
            //Prefer moving forward + fewer towers in range
            for(int dx = -1; dx <= 1; dx++) {
                for(int dy = 0; dy <= 1; dy++) {
                    if(dx == 0 && dy == 0) continue;

                    int newX = posX + dx;
                    int newY = posY + dy;

                    if(newX < 0 || newX >= PlayerLane.WIDTH || newY < 0 || newY >= PlayerLane.HEIGHT) continue;

                    //Check if the cell is empty
                    Cell cell = lane.GetCellAt(newX, newY);
                    if(cell?.Unit != null) continue;

                    //Towers have range 2, so check a 5x5 area around for threats
                    int threatLevel = 0;
                    for(int tx = newX - TowerRange; tx <= newX + TowerRange; tx++) {
                        for(int ty = newY - TowerRange; ty <= newY + TowerRange; ty++) {
                            Cell threatCell = lane.GetCellAt(tx, ty);

                            if(threatCell?.Unit is Tower)
                                threatLevel++;
                        }
                    }

                    int score = dy * ForwardWeight - threatLevel * ThreatPenalty; //Forward is good, towers in range are bad

                    if(score > bestScore) {
                        bestScore = score;
                        bestX = newX;
                        bestY = newY;
                    }
                }
            }

            if(bestX >= 0) MoveTo(bestX, bestY);
        }
    }
}

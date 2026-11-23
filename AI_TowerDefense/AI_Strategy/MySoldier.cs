using GameFramework;
using System.Collections.Generic;

namespace AI_Strategy
{
    /*
     * This class derives from Soldier and provides a new move method. Your assignment should
     * do the same - but with your own movement strategy.
     */
    public class MySoldier : Soldier
    {
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
                for(int dy = -1; dy <= 1; dy++) {
                    if(dx == 0 && dy == 0) continue;

                    int newX = posX + dx;
                    int newY = posY + dy;

                    if(newX < 0 || newX >= PlayerLane.WIDTH || newY < 0 || newY > PlayerLane.HEIGHT) continue;

                    //Check if the cell is empty
                    Cell cell = lane.GetCellAt(newX, newY);
                    if(cell?.Unit != null) continue;

                    //Towers have range 2, so check a 5x5 area around for threats
                    int threatLevel = 0;
                    for(int tx = newX - 2; tx <= newX + 2; tx++) {
                        for(int ty = newY - 2; ty <= newY + 2; ty++) {
                            Cell threatCell = lane.GetCellAt(tx, ty);

                            if(threatCell?.Unit is Tower)
                                threatLevel++;
                        }
                    }

                    int score = dy * 10 - threatLevel * 6; //Forward is good, towers in range are bad

                    if(score > bestScore) {
                        bestScore = score;
                        bestX = newX;
                        bestY = newY;
                    }
                }
            }

            MoveTo(bestX, bestY);

            //int x = posX;
            //int y = posY;
            //for(int i = speed; i > 0; i--) {
            //    if(MoveTo(x, y + i)) return;
            //    if(MoveTo(x + i, y + i)) return;
            //    if(MoveTo(x - i, y + i)) return;
            //    if(MoveTo(x + i, y)) return;
            //    if(MoveTo(x - i, y)) return;
            //    if(MoveTo(x, y - i)) return;
            //    if(MoveTo(x - i, y - i)) return;
            //    if(MoveTo(x + i, y - i)) return;
            //}
        }
    }
}

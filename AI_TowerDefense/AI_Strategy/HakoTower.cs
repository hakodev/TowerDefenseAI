using System.Collections.Generic;
using GameFramework;

namespace AI_Strategy {
    public class HakoTower : Tower {
        protected override List<Unit> SortTargetsInRange(List<Unit> targets) {
            targets.Sort((a, b) => {
                //Finish off any soldier that are close to dying
                bool aCloseToDying = a.Health <= damageCaused;
                bool bCloseToDying = b.Health <= damageCaused;
                if(aCloseToDying && !bCloseToDying) return -1;
                if(!aCloseToDying && bCloseToDying) return 1;

                //Otherwise, shoot whoever is furthest down the lane
                int yCompare = b.PosY.CompareTo(a.PosY);
                if(yCompare != 0) return yCompare;

                //If tied, shoot the soldier with the lowest health
                return a.Health.CompareTo(b.Health);
            });

            return targets;
        }
    }
}

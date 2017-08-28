using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS13_Chemistry {
    class Reagent {
        public String id = "";
        public String name = "";
        public String description = "";

        public override string ToString() {
            return $"{id}: {name} - {description}";
        }
    }
}

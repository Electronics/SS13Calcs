using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS13_Chemistry {
    class Recipe {
        public String name = "";
        public String id = "";
        public int tempNeeded;
        public Dictionary<String , int> ingredients = new Dictionary<string, int>();
        public Dictionary<String , int> results = new Dictionary<string, int>(); // result is a list in the code, presumably a few reagents can create multiple results?

        public override string ToString() {
            String builder = $"{id} - {name}, Results: ";
            foreach(KeyValuePair<String, int> x in results) {
                builder += $"{x.Key} - {x.Value}, ";
            }
            builder += $"; Ingredients: ";
            foreach(KeyValuePair<String, int> x in ingredients) {
                builder += $"{x.Key} - {x.Value}, ";
            }
            builder += $"; required temp: {tempNeeded}";
            return builder;
        }
    }
}

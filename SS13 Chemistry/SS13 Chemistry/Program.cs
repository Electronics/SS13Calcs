using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SS13_Chemistry {
    class Program {
        static List<Reagent> reagentList = new List<Reagent>();
        static List<Recipe> recipeList = new List<Recipe>();
        static List<Reagent> chemDispenser = new List<Reagent>();

        static List<System.ConsoleColor> colorTree = new List<System.ConsoleColor> { ConsoleColor.White, ConsoleColor.Green , ConsoleColor.Magenta , ConsoleColor.Yellow , ConsoleColor.Cyan , ConsoleColor.Blue , ConsoleColor.Red};

        static void Main(string[] args) {
            Console.SetWindowSize(Console.WindowWidth + 50 , Console.WindowHeight+15);

            Extractors.initSSL();

            reagentList.AddRange(Extractors.Reagents(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/reagents/alcohol_reagents.dm"));
            reagentList.AddRange(Extractors.Reagents(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/reagents/cat2_medicine_reagents.dm"));
            reagentList.AddRange(Extractors.Reagents(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/reagents/drink_reagents.dm"));
            reagentList.AddRange(Extractors.Reagents(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/reagents/drug_reagents.dm"));
            reagentList.AddRange(Extractors.Reagents(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/reagents/food_reagents.dm"));
            reagentList.AddRange(Extractors.Reagents(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/reagents/medicine_reagents.dm"));
            reagentList.AddRange(Extractors.Reagents(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/reagents/other_reagents.dm"));
            reagentList.AddRange(Extractors.Reagents(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/reagents/pyrotechnic_reagents.dm"));
            reagentList.AddRange(Extractors.Reagents(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/reagents/toxin_reagents.dm"));

            recipeList.AddRange(Extractors.Recipes(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/recipes/cat2_medicines.dm"));
            recipeList.AddRange(Extractors.Recipes(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/recipes/drugs.dm"));
            recipeList.AddRange(Extractors.Recipes(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/recipes/medicine.dm"));
            recipeList.AddRange(Extractors.Recipes(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/recipes/others.dm"));
            recipeList.AddRange(Extractors.Recipes(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/recipes/pyrotechnics.dm"));
            recipeList.AddRange(Extractors.Recipes(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/recipes/slime_extracts.dm"));
            recipeList.AddRange(Extractors.Recipes(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/recipes/toxins.dm"));
            recipeList.AddRange(Extractors.Recipes(@"https://github.com/tgstation/tgstation/raw/master/code/modules/food_and_drinks/recipes/drinks_recipes.dm"));

            chemDispenser.AddRange(Extractors.ChemDispenser(@"https://github.com/tgstation/tgstation/raw/master/code/modules/reagents/chemistry/machinery/chem_dispenser.dm"));

            /*foreach(Reagent r in reagentList) {
                Debug.WriteLine($"{r}");
            }
            foreach(Recipe r in recipeList) {
                Debug.WriteLine($"{r}");
            }*/

            Console.WriteLine($"Loaded {reagentList.Count} reagents, {recipeList.Count} recipes and {chemDispenser.Count} reagents in Chemistry Dispenser");

            while (true) {
                mainMenu();
            }
        }

        static void mainMenu() {
            Console.WriteLine("Search for('?[term]' for reagents, '[Tree depth]![term]' for more defined depth), '%' for searching descriptions: ");
            var result = Console.ReadLine();
            if (result.Contains("?")) {
                Console.WriteLine($"Reagent results: \r\n");
                searchReagents(result.Split('?')[1].Trim(), false);
            } else if (result.Contains("!")) {
                String depthString = result.Split('!')[0].Trim();
                if (String.IsNullOrWhiteSpace(depthString)) Console.WriteLine("Please enter a number before the '!'");
                else {
                    search(result.Split('!')[1].Trim() , "" , 0 , int.Parse(depthString), false);
                }
            } else if (result.Contains("%")) {
                result = result.Replace('%', ' ').Trim();
                searchDescriptions(result);
            }
            else {
                Console.WriteLine($"Recipe results: \r\n");
                search(result.Trim() , "");
            }
            Console.WriteLine($"\r\n");
        }

        static void search(String searchString, String prefix) { search(searchString , prefix , 0,10, false); }
        static void search(String searchString, String prefix, int depth, int maxDepth, bool strict) {
            depth++;
            if (depth > maxDepth) return;
            bool found = false;

            // let's check that it's not a reagent you can just get in the chemistry dispenser first
            foreach(Reagent r in chemDispenser) {
                if(strict ? r.id.Equals(searchString) : r.name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >=0 || r.id.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >=0) {
                    /*Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{prefix}{r}");
                    Console.ResetColor();*/

                    // let's not bother printing the elements if they're in the chem dispenser
                    if (r.upgradeTier <= 1)
                        return;
                    // however if they are in a higher tier, let's state that it's availble, but also provide sub recipies
                    Console.ForegroundColor = colorTree[depth];
                    Console.WriteLine($"{prefix}\t\t{Reagent.Trim(searchString)} is available in a {r.upgradeTier} tier dispenser");
                    Console.ResetColor();
                }
            }

            foreach(Recipe r in recipeList) {
                if (strict ? r.id.Equals(searchString) : r.name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >=0) {
					// matches a recipe by name
					Reagent recipe_reagent;
					if (r.results.Count < 1)
						recipe_reagent = new SS13_Chemistry.Reagent();
					else
						recipe_reagent = findReagent(r.results.First().Key , strict);
                    Console.ForegroundColor = colorTree[depth];
                    Console.WriteLine($"{prefix}{r.Short()}" + ((depth > 1 || string.IsNullOrWhiteSpace(recipe_reagent.description)) ? "" : $" || Description: {recipe_reagent.description}"));
                    searchSubResults(r, depth, maxDepth);
                    found = true;
                } else {
                    foreach (KeyValuePair<String , int> result in r.results) {
                        if (strict ? result.Key.Equals(searchString) : result.Key.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >=0) { // matches a recipe by product
                            Console.ForegroundColor = colorTree[depth];
                            Console.WriteLine($"{prefix}{r}");
                            searchSubResults(r, depth, maxDepth);
                            found = true;
                        }
                    }
                }
            }
            if(!found && depth<2) {
                Console.ForegroundColor = ConsoleColor.Red;
                searchReagents(searchString, strict); // match reagent
            }
            Console.ResetColor();
        }

        static void searchSubResults(Recipe r, int depth, int maxDepth) {
            String depthIndicator = "";
            for (int i = 0; i < depth; i++) depthIndicator += "> ";
            foreach(KeyValuePair<String, int> ingredient in r.ingredients) {
                search(ingredient.Key , depthIndicator, depth, maxDepth, true); // strict as we know the ingredient name IS a reagent id
            }
        }

        static void searchReagents(String searchString, bool strict) {
            Reagent r = findReagent(searchString , strict);
            if(r.name!="") {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{r}");
                Console.ResetColor();
            }
        }
        
        static Reagent findReagent(String searchString, bool strict) {
            Reagent returnReagent = new Reagent();
            foreach(Reagent r in reagentList) {
                if (strict ? r.id.Equals(searchString) : r.name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >=0 || r.description.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >=0 || r.id.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >=0) {
                    returnReagent = r;
                }
            }
            return returnReagent;
        }

        static void searchDescriptions(String searchString) {
            int colorFlapper = 0;
            foreach(Reagent r in reagentList) {
                if (r.description.Contains(searchString)) {
                    Console.ForegroundColor = colorTree[colorFlapper];
                    colorFlapper = colorFlapper < colorTree.Count ? colorFlapper + 1 : 0;
                    Console.WriteLine($"{r} || Description: {r.description}");
                }
            }
            Console.ResetColor();
        }
    }
}

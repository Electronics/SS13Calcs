using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SS13_Chemistry {
    class Extractors {
        static WebClient wb = new WebClient();

        public static void initSSL() {
            ServicePointManager.Expect100Continue = true;                
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public static String getFromURL(String URL) {
            Debug.WriteLine($"Downloading from {URL}...");
            Console.WriteLine($"Downloading from {URL}...");
            try {
                return wb.DownloadString(URL);
            } catch (Exception e) {
                Debug.WriteLine($"ERROR!! {e}");
                System.Environment.Exit(0);
                return null;
            }
        }

        public static List<Reagent> Reagents(String URL) {
            List<Reagent> reagentList = new List<Reagent>();

            String raw = getFromURL(URL);

            foreach(String line in raw.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)) {
                if (line.Contains("/datum/reagent/") && !line.Contains('(')) { // don't get a proc by accident
                    //Debug.WriteLine($"new reagent: {line}");
                    reagentList.Add(new Reagent());
                    // let's also set it's name and id to this incase it doesn't have a name or id (in the case of nested stuff)
                    reagentList.Last().name = reagentList.Last().id = line;
                } else if (line.Contains("glass_name")) {
                } else if (line.Contains("name = ")) {
                     reagentList.Last().name = line.Split('"')[1];
                } else if (line.Contains("id = ")) {
                    reagentList.Last().id = line.Split('"')[1];
                } else if (line.Contains("description = ") && !line.Contains("taste_description = ")) {
                    reagentList.Last().description = line.Split('"')[1];
                }

            }

            return reagentList;
        }

        public static List<Recipe> Recipes(String URL) {
            List<Recipe> recipeList = new List<Recipe>();

            String raw = getFromURL(URL);

            foreach(String line in raw.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)) {
                if (line.Contains("/datum/chemical_reaction/") && !line.Contains('(')) { // don't get a proc by accident
                    //Debug.WriteLine($"new reagent: {line}");
                    recipeList.Add(new Recipe());
                    // let's also set it's name and id to this incase it doesn't have a name or id (in the case of nested stuff)
                    recipeList.Last().name = recipeList.Last().id = line;
                } else if (line.Contains("glass_name")) {
                } else if (line.Contains("name = ")) {
                    try {
                        recipeList.Last().name = line.Split('"')[1].Trim();
                    } catch (IndexOutOfRangeException e) {
                        recipeList.Last().name = line.Split('=')[1].Trim();
                    }
                } else if (line.Contains("id = ") && !line.Contains("bid = ")) {
                    recipeList.Last().id = line.Split('=')[1].Trim();
                } else if (line.Contains("required_reagents = ")) {
                    String req = line.Split('(' , ')')[1];

                    foreach(String s in req.Split(',')) {
                        //Debug.WriteLine($"I: {s.Trim().Split('"')[1]} {int.Parse(s.Substring(s.IndexOf('=') + 1).Trim())}");
                        recipeList.Last().ingredients.Add(s.Trim().Split('=')[0].Trim(), int.Parse(s.Substring(s.IndexOf('=') + 1).Trim()));
                    }
                } else if (line.Contains("results = ")) {
                    String req = line.Split('(' , ')')[1];

                    foreach(String s in req.Split(',')) {
                        //Debug.WriteLine($"I: {s.Trim().Split('"')[1]} {int.Parse(s.Substring(s.IndexOf('=') + 1).Trim())}");
                        recipeList.Last().results.Add(s.Trim().Split('=')[0].Trim(), int.Parse(s.Substring(s.IndexOf('=') + 1).Trim()));
                    }
                } else if (line.Contains("required_temp = ")) {
                    try {
                        recipeList.Last().tempNeeded = int.Parse(line.Substring(line.IndexOf('=') + 1).Split('/')[0].Trim());
                    } catch (FormatException e) {
                        if (recipeList.Last().name.Contains("TaTP")) {
                            recipeList.Last().tempNeeded = 450;
                            Console.WriteLine("TaTP has a random temperature +-50 applied at round start");
                        } else {
                            throw e;
                        }
                    }
                }

            }

            return recipeList;
        }

        public static List<Reagent> ChemDispenser(String URL) {
            List<Reagent> reagentList = new List<Reagent>();

            String raw = getFromURL(URL);

            int tier = -1;
            foreach (String line in raw.Split(new string[] { "\r\n" , "\n" } , StringSplitOptions.RemoveEmptyEntries)) {
                if (tier >= 0) {
                    if (line.Contains(")")) {
                        if (tier > 10) {
                            break; // hit the emmaged section, so we can finish
                        }
                        tier = -1; // OLDCOMMENT:  not really needed but if we want to go on for some reason in future update - AH HAH, thanks past me
                    }
                    string new_id = line.Replace(',', ' ').Trim();
                    var existing_reagent = reagentList.FirstOrDefault(reagent => reagent.id.Contains(new_id));

                    if (existing_reagent==null) {
                        reagentList.Add(new Reagent());
                        reagentList.Last().id = new_id;
                        reagentList.Last().upgradeTier = tier;
                    } else {
                        existing_reagent.upgradeTier = tier;
                    }

                } else if (line.Contains("var/list/dispensable_reagents = list(")) {
                    tier = 1;
                } else if (line.Contains("var/list/upgrade_reagents = list(")) {
                    tier = 4;
                } else if (line.Contains("var/list/emagged_reagents = list(")) {
                    tier = 9999; // emmaged!
                }
            }
            return reagentList;
        }
    }
}

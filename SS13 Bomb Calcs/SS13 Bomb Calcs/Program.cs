using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SS13_Bomb_Calcs {
	class Program {
        static void Main(string[] args) {

            Tank o2tank = new Tank(Constants.TANK_VOLUME, Constants.MAX_TANK_PRESSURE , Constants.T0C-200 , 0, 1F , 0 , 0 , 0 , 0 , 0 , 0);

            for (float stopTemp = 300; stopTemp < 20000; stopTemp += 10) {
            //float stopTemp = 12000+Constants.T0C;

                Tank canister = new Tank(Constants.TANK_VOLUME , Constants.MAX_TANK_PRESSURE , Constants.T20C , 0.66666F , 1 - 0.66666F , 0 , 0 , 0 , 0 , 0 , 0);

                // let's start off the burn inside the cannsiter
                canister.temperature = Constants.PLASMA_MINIMUM_BURN_TEMPERATURE;

                while (true) {
                    // simulate some time
                    BombCalcs.react(canister);
                    if (canister.temperature > stopTemp) break;
                }
                //Debug.WriteLine($"Canister got to: {canister}");

                //Tank plasmaTank = new Tank(Constants.TANK_VOLUME,Constants.MAX_TANK_PRESSURE, canister.temperature, canister.gases.plasma.moles/canister.getTotalMoles(), canister.gases.oxygen.moles/canister.getTotalMoles(), 0, canister.gases.carbonDioxide.moles/canister.getTotalMoles(),0,0,0,0);


                //Debug.WriteLine($"New plasma tank: {plasmaTank}");
                Debug.WriteLine($"At {stopTemp}, Explosion range: {runBomb(o2tank , canister)}");
            }

            /*float best = 0;
            float bestPlasma = 0;
            float besto2 = 0;
            float bestco2 = 0;
            float bestn2o = 0;
            float bestfreon = 0;
            for (float co2 = 0; co2 < 0.5F; co2 += 0.01F) {
                for (float n2o = 0; n2o < 0.5F; n2o += 0.01F) {
                    //for (float freon = 0; freon < 0.3F; freon += 0.1F) {
                        for (float plasmatemp = 500; plasmatemp < 2000; plasmatemp += 1) {
                            for (float o2temp = 0; o2temp < 100; o2temp += 1) {
                                float temp = runBomb(plasmatemp , o2temp , co2 , n2o , 0);
                                if (temp > best) {
                                    best = temp;
                                    bestPlasma = plasmatemp;
                                    besto2 = o2temp;
                                    bestco2 = co2;
                                    bestn2o = n2o;
                                    //bestfreon = freon;
                                }
                            }
                        }
                        Debug.WriteLine($"co2: {co2}, n2o: {n2o}, best sofar: {best}");
                    //}
                }
            }
            Debug.WriteLine($"Best: {best} - Plasma:{bestPlasma}K O2:{besto2}K with contents co2:{bestco2}, n2o:{bestn2o}, freon:{bestfreon}");
            */

            //Debug.WriteLine(runBomb(1763 , 73.15F , 0.510F , 0 , 0));
            
		}

        static void runPureTest() {
            StringBuilder sb = new StringBuilder();

            // put a nice header accross the top
            for(float o2temp = 0; o2temp<150; o2temp+=1) {
                if (o2temp == 0) sb.AppendFormat("HEYO,");
                sb.AppendFormat("{0},",o2temp);
            }
            sb.AppendLine();

            for(float plasmatemp = 400; plasmatemp<2000; plasmatemp+=1) {
                for(float o2temp = 0; o2temp<150; o2temp+=1) {
                    if (o2temp == 0) sb.AppendFormat("{0},",plasmatemp);
                    sb.AppendFormat("{0},",runBomb(plasmatemp , o2temp));
                }
                sb.AppendLine();
            }

            System.IO.File.WriteAllText("C:\\Users\\Laurie\\Desktop\\output.csv", sb.ToString());

            //Debug.WriteLine(runBomb(100,5));
        }

        static float runBomb(float plasmaTemp, float oxygenTemp) {
            float range = 0;

            Tank tank1 = new Tank(Constants.TANK_VOLUME, Constants.MAX_TANK_PRESSURE , oxygenTemp , 0, 1F , 0 , 0 , 0 , 0 , 0 , 0);
			//Debug.WriteLine($"Tank1: {tank1.ToString()}");
			Tank tank2 = new Tank(Constants.TANK_VOLUME , Constants.MAX_TANK_PRESSURE , plasmaTemp , 1F , 0 , 0 , 0 , 0 , 0 , 0 , 0);
			//Debug.WriteLine($"Tank2: {tank2.ToString()}");

			Tank newTank = BombCalcs.merge(tank1 , tank2);

            int iteration;
            for(iteration=0; iteration<100;iteration++) {
                if(newTank.pressure > Constants.TANK_FRAGMENT_PRESSURE) {
                    //Debug.WriteLine($"Pressure is big enough to fragment tank after {iteration} iterations");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 1st Iteration, tank is now: {newTank}");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 2nd Iteration, tank is now: {newTank}");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 3rd Iteration, tank is now: {newTank}");

                    //KABOOM
                    range = BombCalcs.explode(newTank);
                    break;
                }
                if (!BombCalcs.react(newTank)) break;
                //Debug.WriteLine($"{iteration}, tank is now: {newTank}");
                iteration++;
            }

            return range;
        }

        static float runBomb(float plasmaTemp, float oxygenTemp, float extraCO2, float extraN20, float extraFreon) {
            float range = 0;

            Tank tank1 = new Tank(Constants.TANK_VOLUME, Constants.MAX_TANK_PRESSURE , oxygenTemp , 0, 1, 0 , 0, 0 , 0 , 0 , 0);
			Debug.WriteLine($"Tank1: {tank1.ToString()}");
			Tank tank2 = new Tank(Constants.TANK_VOLUME , 2418 , plasmaTemp , 1F-extraCO2-extraFreon-extraN20, 0, 0, extraCO2 , 0 , extraFreon , extraN20 , 0);
			Debug.WriteLine($"Tank2: {tank2.ToString()}");

			Tank newTank = BombCalcs.merge(tank1 , tank2);

            int iteration;
            for(iteration=0; iteration<100;iteration++) {
                if(newTank.pressure > Constants.TANK_FRAGMENT_PRESSURE) {
                    //Debug.WriteLine($"Pressure is big enough to fragment tank after {iteration} iterations");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 1st Iteration, tank is now: {newTank}");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 2nd Iteration, tank is now: {newTank}");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 3rd Iteration, tank is now: {newTank}");

                    //KABOOM
                    range = BombCalcs.explode(newTank);
                    break;
                }
                if (!BombCalcs.react(newTank)) break;
                //Debug.WriteLine($"{iteration}, tank is now: {newTank}");
                iteration++;
            }

            return range;
        }

        static float runBomb(Tank tank1, Tank tank2) {
            float range = 0;
            Tank newTank = BombCalcs.merge(tank1 , tank2);
            int iteration;
            for(iteration=0; iteration<100;iteration++) {
                if(newTank.pressure > Constants.TANK_FRAGMENT_PRESSURE) {
                    //Debug.WriteLine($"Pressure is big enough to fragment tank after {iteration} iterations");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 1st Iteration, tank is now: {newTank}");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 2nd Iteration, tank is now: {newTank}");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 3rd Iteration, tank is now: {newTank}");

                    //KABOOM
                    range = BombCalcs.explode(newTank);
                    break;
                }
                if (!BombCalcs.react(newTank)) break;
                //Debug.WriteLine($"{iteration}, tank is now: {newTank}");
                iteration++;
            }

            return range;
        }
        
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SS13_Bomb_Calcs {
	class Program {
        static void Main(string[] args) {

            Tank o2tank = new Tank(Constants.TANK_VOLUME, Constants.MAX_TANK_PRESSURE , Constants.T0C-200 , 0, 1F , 0 , 0 , 0 , 0 , 0 , 0, 0, 0, 0, 0);

            /*for (double stopTemp = 300; stopTemp < 20000; stopTemp += 10) {
            //double stopTemp = 12000+Constants.T0C;

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
            }*/

            //calculateBestPureGivenPressure(Constants.MAX_TANK_PRESSURE);


            /*double best = 0;
            double bestPlasma = 0;
            double besto2 = 0;
            double bestco2 = 0;
            double bestn2o = 0;
            double bestnob = 0;
            double besttrit = 0;
            double bestpluox = 0;
            for (double co2 = 0; co2 < 0.25; co2 += 0.05F) {
                for (double n2o = 0; n2o < 0.01; n2o += 0.05F) {
                    for (double nob = 0; nob < 0.25; nob += 0.01F) {
                        for (double pluox = 0; pluox < 0.01; pluox += 0.05F) {
                            for (double trit = 0; trit < 0.25; trit += 0.05F) {
                                for (double plasmatemp = 400; plasmatemp < 4000; plasmatemp += 20) {
                                    for (double o2temp = 73.15; o2temp < 80; o2temp += 1) {
                                        double temp = runBomb(plasmatemp,o2temp,co2,n2o,nob,0,trit,pluox,Constants.MAX_TANK_PRESSURE);
                                        if (temp > best) {
                                            best = temp;
                                            bestPlasma = plasmatemp;
                                            besto2 = o2temp;
                                            bestco2 = co2;
                                            bestn2o = n2o;
                                            bestnob = nob;
                                            bestpluox = pluox;
                                            besttrit = trit;
                                        }
                                    }
                                    Debug.WriteLine($"co2:{co2}, n2o:{n2o}, nob:{bestnob}, pluox:{pluox}, trit:{trit}, best sofar: {best} (temp: {plasmatemp} / {besto2}");
                                }
                            }
                        }  
                    }
                }
            }
            Debug.WriteLine($"Best: {best} - Plasma:{bestPlasma}K O2:{besto2}K with contents co2:{bestco2}, n2o:{bestn2o}, nob:{bestnob}, pluox:{bestpluox}, trit:{besttrit}");
            */

            //Debug.WriteLine(runBomb(100 , 12));
            /*Tank t1 = new Tank(Constants);
            t1.gases.oxygen.moles = 285.14;
            t1.pressure = 2533;
            t1.temperature = 74.83;
            Tank t2 = new Tank(Constants);
            t2.gases.plasma.moles = 27.34;
            t2.pressure = 2533;
            t2.temperature = 780.3;*/
            Tank t1 = new Tank(Constants.TANK_VOLUME , 2533 , 40000 , 0 , 0 , 0 , 0 , 0 , 1 , 0 , 0 , 0 , 0 , 0 , 0);
            Tank t2 = new Tank(Constants.TANK_VOLUME , 2533 , 50 , 0, 1, 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0);
            Debug.WriteLine(t2);
            Debug.WriteLine(runBomb(t1 , t2));

            /*for (double j = 200; j < 500; j+=5) {
                Tank burn = new Tank(Constants.TANK_VOLUME , j , 400 , 0.90 , 0.1 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0);
                for (int i = 0; i < 5000; i++) {
                    BombCalcs.react(burn);
                    burn.calculatePressure();
                }
                Debug.WriteLine(burn);
            }*/

        }

        static void runPureTest() {
            StringBuilder sb = new StringBuilder();

            // put a nice header accross the top
            for(double o2temp = 0; o2temp<150; o2temp+=1) {
                if (o2temp == 0) sb.AppendFormat("HEYO,");
                sb.AppendFormat("{0},",o2temp);
            }
            sb.AppendLine();

            for(double plasmatemp = 400; plasmatemp<2000; plasmatemp+=1) {
                for(double o2temp = 0; o2temp<150; o2temp+=1) {
                    if (o2temp == 0) sb.AppendFormat("{0},",plasmatemp);
                    sb.AppendFormat("{0},",runBomb(plasmatemp , o2temp));
                }
                sb.AppendLine();
            }

            System.IO.File.WriteAllText("C:\\Users\\Laurie\\Desktop\\output.csv", sb.ToString());

            //Debug.WriteLine(runBomb(100,5));
        }

        static void calculateBestPureGivenPressure(double pressure) {
            double best = 0;
            double bestPlasma = 0;
            double besto2 = 0;

            for (double plasmatemp = 500; plasmatemp < 50000; plasmatemp += 2F) {
                for (double o2temp = 0; o2temp < 50; o2temp += 0.5F) {
                    double temp = runBomb(plasmatemp , o2temp , 0 , 0 , 0, pressure);
                    if (temp > best) {
                        best = temp;
                        bestPlasma = plasmatemp;
                        besto2 = o2temp;
                        //bestfreon = freon;
                    }
                }
            }
            Debug.WriteLine($"Best: {best} - Plasma:{bestPlasma}K O2:{besto2}K (at {pressure}kPa)");
        }

        static double runBomb(double plasmaTemp, double oxygenTemp) { return runBomb(plasmaTemp , oxygenTemp , 0 , 0 , 0); }
        static double runBomb(double plasmaTemp , double oxygenTemp , double extraCO2 , double extraN20 , double extraNob) { return runBomb(plasmaTemp , oxygenTemp , extraCO2 , extraN20 , extraNob ,0,0,0, Constants.MAX_TANK_PRESSURE); }
        static double runBomb(double plasmaTemp , double oxygenTemp , double extraCO2 , double extraN20 , double extraNob, double pressure) { return runBomb(plasmaTemp , oxygenTemp , extraCO2 , extraN20 , extraNob ,0,0,0, pressure); }
        static double runBomb(double plasmaTemp, double oxygenTemp, double extraCO2, double extraN20, double extraNob, double extraStim, double extraTritium, double extraPluox, double pressure) {
            Tank tank1 = new Tank(Constants.TANK_VOLUME, pressure , oxygenTemp , 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); // o2
                                                                                                                      //Debug.WriteLine($"Tank1: {tank1.ToString()}");

            Tank tank2 = new Tank(Constants.TANK_VOLUME , pressure , plasmaTemp , 1 - extraCO2 - extraNob - extraN20 - extraStim - extraTritium - extraPluox, 0, 0, extraCO2 , 0 , extraNob , 0, extraN20, extraTritium, extraStim, extraPluox, 0);
            //Debug.WriteLine($"Tank2: {tank2.ToString()}");

            return runBomb(tank1 , tank2);
        }
        static double runBomb(Tank tank1, Tank tank2) {
            double range = 0;
            Tank newTank = BombCalcs.merge(tank1 , tank2);
            Debug.WriteLine($"tank is now: {newTank}");
            int iteration;
            for(iteration=0; iteration<100;iteration++) {
                if(newTank.pressure > Constants.TANK_FRAGMENT_PRESSURE) {
                    Debug.WriteLine($"Pressure is big enough to fragment tank after {iteration} iterations");
                    Debug.WriteLine($"pres:{newTank.pressure} {newTank.temperature}K moles:{newTank.getTotalMoles()}, {newTank.gases.plasma.moles} plasma, {newTank.gases.oxygen.moles} o2");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 1st Iteration react:{BombCalcs.react(newTank)}, tank is now: {newTank}");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 2nd Iteration react:{BombCalcs.react(newTank)}, tank is now: {newTank}");
                    BombCalcs.react(newTank);
                    //Debug.WriteLine($"RUPTURE: 3rd Iteration react:{BombCalcs.react(newTank)}, tank is now: {newTank}");

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

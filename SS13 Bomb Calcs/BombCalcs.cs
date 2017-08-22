using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SS13_Bomb_Calcs {
	class BombCalcs {
		public static Tank merge(Tank tank1, Tank tank2) {
			// new tank is created
			Tank newTank = new SS13_Bomb_Calcs.Tank(2 * Constants.TANK_VOLUME);

			if(Math.Abs(tank1.temperature - tank2.temperature) > Constants.MINIMUM_TEMPERATURE_DELTA_TO_CONSIDER) {
                // figure out tank1's total heatcapacity
                float tank1_hc = tank1.getHeatCapacity();
				//Debug.WriteLine($"Tank1's heat capacity: {tank1_hc}");

                // figure out tank2's total heatcapacity
                float tank2_hc = tank2.getHeatCapacity();
				//Debug.WriteLine($"Tank2's heat capacity: {tank2_hc}");

				float totalHeatCapacity = tank1_hc + tank2_hc;
				//Debug.WriteLine($"Total heat capacity: {totalHeatCapacity}");

                // assuming heat capacity >0
                if (totalHeatCapacity <= 0) return null;

				newTank.temperature = (tank1.temperature * tank1_hc + tank2.temperature * tank2_hc) / totalHeatCapacity;
				//Debug.WriteLine($"Combined temperature: {newTank.temperature}K({newTank.temperature-Constants.T0C}C)");

				// now add the gases to each other
				for(int i=0; i<tank1.gases.gaslist.Length; i++) {
					newTank.gases.gaslist[i].moles += tank1.gases.gaslist[i].moles;
					newTank.gases.gaslist[i].moles += tank2.gases.gaslist[i].moles;
				}

                newTank.calculatePressure();
				//Debug.WriteLine($"Merged tank: {newTank}");

                return newTank;
			}
			return null;
		}

        public static bool react(Tank tank) {
            // this will only deal with plasma/oxygen burnmix reactions
            if (tank.temperature < Constants.FIRE_MINIMUM_TEMPERATURE_TO_EXIST) return false;

            float oldHeatCapacity = tank.getHeatCapacity();

            float temperatureScale = 0;
            if(tank.temperature > Constants.PLASMA_UPPER_TEMPERATURE) {
                temperatureScale = 1;
            } else {
                // BYOND COMPILER DOES SOMETHING ODD AND MAKES T0C NEGATIVE
                //                                                                                                                                              V BYOND RETARDATION
                temperatureScale = (tank.temperature - (Constants.PLASMA_MINIMUM_BURN_TEMPERATURE-2*Constants.T0C)) / (Constants.PLASMA_UPPER_TEMPERATURE - Constants.PLASMA_MINIMUM_BURN_TEMPERATURE + 2*Constants.T0C);
            }
            //Debug.WriteLine($"{tank.temperature - (Constants.PLASMA_MINIMUM_BURN_TEMPERATURE-2*Constants.T0C)} {(Constants.PLASMA_UPPER_TEMPERATURE - Constants.PLASMA_MINIMUM_BURN_TEMPERATURE + 2*Constants.T0C)}");

            if(temperatureScale>0) {
                float oxygen_burn_rate = Constants.OXYGEN_BURN_BASE_RATE - temperatureScale;

                float plasma_burn_rate = 0;
                if(tank.gases.oxygen.moles > tank.gases.plasma.moles * Constants.PLASMA_OXYGEN_FULLBURN) {
                    plasma_burn_rate = tank.gases.plasma.moles * temperatureScale / Constants.PLASMA_BURN_RATE_DELTA;
                } else {
                    plasma_burn_rate = (temperatureScale * (tank.gases.oxygen.moles / Constants.PLASMA_OXYGEN_FULLBURN)) / Constants.PLASMA_BURN_RATE_DELTA;
                }

                //Debug.WriteLine($"temperatureScale={temperatureScale}, plasmaBurnRate={plasma_burn_rate}, oxygenBurnRate={oxygen_burn_rate}");

                if(plasma_burn_rate > Constants.MINIMUM_HEAT_CAPACITY) {
                    tank.gases.plasma.moles -= plasma_burn_rate;
                    tank.gases.oxygen.moles -= (plasma_burn_rate * oxygen_burn_rate);
                    tank.gases.carbonDioxide.moles += plasma_burn_rate;

                    float energy_released = Constants.FIRE_PLASMA_ENERGY_RELEASED * plasma_burn_rate;
                    
                    if(energy_released > 0) {
                        if(tank.getHeatCapacity() > Constants.MINIMUM_HEAT_CAPACITY) {
                            tank.temperature = (tank.temperature * oldHeatCapacity + energy_released) / tank.getHeatCapacity();
                        }
                    } 
                }
            }
            tank.calculatePressure();
            return true;
        }

        public static float explode(Tank tank) {
            float range = (tank.pressure - Constants.TANK_FRAGMENT_PRESSURE) / Constants.TANK_FRAGMENT_SCALE;
            //Debug.WriteLine($"Range: {range}");
            //Debug.WriteLine($"EXPLOSION: ({Math.Round(range * 0.25)},{Math.Round(range * 0.5)},{Math.Round(range)},{Math.Round(range * 1.5)})");
            return range;
        }
	}
}

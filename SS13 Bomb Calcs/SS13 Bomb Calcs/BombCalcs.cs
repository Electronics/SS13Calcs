using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SS13_Bomb_Calcs {
	class BombCalcs {
        enum Fusion_types {SUPER, HIGH, MID, LOW=0};
		public static Tank merge(Tank tank1, Tank tank2) {
			// new tank is created
			Tank newTank = new Tank(2 * Constants.TANK_VOLUME);

			if(Math.Abs(tank1.temperature - tank2.temperature) > Constants.MINIMUM_TEMPERATURE_DELTA_TO_CONSIDER) {
                // figure out tank1's total heatcapacity
                double tank1_hc = tank1.getHeatCapacity();
				//Debug.WriteLine($"Tank1's heat capacity: {tank1_hc}");

                // figure out tank2's total heatcapacity
                double tank2_hc = tank2.getHeatCapacity();
				//Debug.WriteLine($"Tank2's heat capacity: {tank2_hc}");

				double totalHeatCapacity = tank1_hc + tank2_hc;
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

        public static bool react_supress(Tank tank) {
            //if (tank.gases.noblium.moles > Constants.REACTION_OPPRESSION_THRESHOLD)Console.WriteLine($"{tank.gases.noblium.moles} STOPPED");
            if (tank.gases.noblium.moles > Constants.REACTION_OPPRESSION_THRESHOLD) {
                Debug.WriteLine($"Reacting STOP");
                return true;
            }
            return false;
        }

        public static bool react_fusion(Tank tank) {
            
            if (tank.temperature < Constants.FUSION_TEMPERATURE_THRESHOLD) return false;
            if (tank.temperature * tank.getHeatCapacity() < Constants.FUSION_ENERGY_THRESHOLD) return false; // energy check
            if (tank.gases.plasma.moles < Constants.FUSION_MOLE_THRESHOLD) return false;
            if (tank.gases.tritium.moles < Constants.FUSION_MOLE_THRESHOLD) return false;

            Debug.WriteLine($"Reacting fusion");
            double oldHeatCapacity = tank.getHeatCapacity();
            double reaction_energy = 0;

            double mediation = Constants.FUSION_MEDIATION_FACTOR * (tank.getHeatCapacity() - tank.gases.plasma.moles * Constants.PLASMA_SPECIFIC_HEAT_CAPACITY) / (tank.getTotalMoles() - tank.gases.plasma.moles); // heat capacity - plasma?

            double moles_excluding_plasma = tank.getTotalMoles() - tank.gases.plasma.moles;
            double plasma_differential = (tank.gases.plasma.moles - moles_excluding_plasma) / tank.getTotalMoles();
            double reaction_efficiency = Math.Pow(Constants.FUSION_EFFICIENCY_BASE , (Math.Pow(plasma_differential , 2) / Constants.FUSION_EFFICIENCY_DIVISOR));

            double gas_power = 0;
            foreach(Gas gas in tank.gases) {
                gas_power += reaction_efficiency * gas.moles * gas.fusionPower;
            }
            double power_ratio = gas_power / mediation;
            double gases_fused = tank.getTotalMoles();

            //radiation stuffz
            
            Fusion_types fusion_type=Fusion_types.LOW;
            double fusion_release_energy;
            double fusion_energy_divisor;
            double fusion_gas_creation_factor;
            if (power_ratio > Constants.FUSION_SUPER_TIER) {
                fusion_release_energy = Constants.FUSION_RELEASE_ENERGY_SUPER;
                fusion_energy_divisor = Constants.FUSION_ENERGY_DIVISOR_SUPER;
                fusion_gas_creation_factor = Constants.FUSION_GAS_CREATION_FACTOR_SUPER;
                fusion_type=Fusion_types.SUPER;
            } else if(power_ratio>Constants.FUSION_HIGH_TIER) {
                fusion_release_energy = Constants.FUSION_RELEASE_ENERGY_HIGH;
                fusion_energy_divisor = Constants.FUSION_ENERGY_DIVISOR_HIGH;
                fusion_gas_creation_factor = Constants.FUSION_GAS_CREATION_FACTOR_HIGH;
                fusion_type=Fusion_types.HIGH;
            } else if(power_ratio>Constants.FUSION_MID_TIER) {
                fusion_release_energy = Constants.FUSION_RELEASE_ENERGY_MID;
                fusion_energy_divisor = Constants.FUSION_ENERGY_DIVISOR_MID;
                fusion_gas_creation_factor = Constants.FUSION_GAS_CREATION_FACTOR_MID;
                fusion_type=Fusion_types.MID;
            } else {
                fusion_release_energy = Constants.FUSION_RELEASE_ENERGY_LOW;
                fusion_energy_divisor = Constants.FUSION_ENERGY_DIVISOR_LOW;
                fusion_gas_creation_factor = Constants.FUSION_GAS_CREATION_FACTOR_LOW;
                fusion_type=Fusion_types.LOW;
            }

            reaction_energy += gases_fused * Constants.FUSION_RELEASE_ENERGY_SUPER * (power_ratio / Constants.FUSION_ENERGY_DIVISOR_SUPER);
            foreach (Gas gas in tank.gases) {
                gas.moles = 0;
            }
            
            if (fusion_type==Fusion_types.SUPER) {
                tank.gases.stimulum.moles += gases_fused * Constants.FUSION_GAS_CREATION_FACTOR_SUPER;
                tank.gases.pluoxium.moles += gases_fused * Constants.FUSION_GAS_CREATION_FACTOR_SUPER;
            }
            if (fusion_type == Fusion_types.HIGH) {
                tank.gases.tritium.moles += gases_fused * Constants.FUSION_GAS_CREATION_FACTOR_HIGH;
            }
            if (fusion_type == Fusion_types.MID) {
                tank.gases.BZ.moles += gases_fused * Constants.FUSION_GAS_CREATION_FACTOR_MID;
                tank.gases.nitrousOxide.moles += gases_fused * Constants.FUSION_GAS_CREATION_FACTOR_MID;
            }
            if (fusion_type == Fusion_types.MID) {
                tank.gases.oxygen.moles += gases_fused * Constants.FUSION_GAS_CREATION_FACTOR_LOW;
                tank.gases.carbonDioxide.moles += gases_fused * Constants.FUSION_GAS_CREATION_FACTOR_LOW;
            }
                
            // radiation BOOOM!

            if(reaction_energy>0) {
                if(tank.getHeatCapacity() > Constants.MINIMUM_HEAT_CAPACITY) {
                    tank.temperature = (tank.temperature * oldHeatCapacity + reaction_energy) / tank.getHeatCapacity();
                }
                return true;
            }

            return false;
        }

        public static bool react_nitryl(Tank tank) {
            
            if (tank.gases.oxygen.moles < 20) return false;
            if (tank.gases.nitrogen.moles < 20) return false;
            if (tank.gases.nitrousOxide.moles < 5) return false;
            if (tank.temperature < Constants.FIRE_MINIMUM_TEMPERATURE_TO_EXIST*400) return false;

            Debug.WriteLine($"Reacting Nitryl");
            double oldHeatCapacity = tank.getHeatCapacity();
            double heat_efficiency = Math.Min(tank.temperature / (Constants.FIRE_MINIMUM_TEMPERATURE_TO_EXIST * 100) , Math.Min(tank.gases.oxygen.moles , tank.gases.nitrogen.moles));
            double energy_used = heat_efficiency * Constants.NITRYL_FORMATION_ENERGY;
            if (tank.gases.oxygen.moles - heat_efficiency < 0 || tank.gases.nitrogen.moles - heat_efficiency < 0) return false;
            tank.gases.oxygen.moles -= heat_efficiency;
            tank.gases.nitrogen.moles -= heat_efficiency;
            tank.gases.nitryl.moles += heat_efficiency * 2;

            if(energy_used > 0) {
                if(tank.getHeatCapacity() > Constants.MINIMUM_HEAT_CAPACITY) {
                    tank.temperature = Math.Max((tank.temperature * oldHeatCapacity - energy_used) / tank.getHeatCapacity(),Constants.TCMB);
                }
                return true;
            }
            return false;
        }

        public static bool react_bz(Tank tank) {
            
            if (tank.gases.plasma.moles < 10) return false;
            if (tank.gases.nitrousOxide.moles < 10) return false;
            Debug.WriteLine($"Reacting BZ");
            double oldHeatCapacity = tank.getHeatCapacity();

            double reaction_efficiency = Math.Min(1 / ((tank.pressure / (0.1 * Constants.ATMOSPHERE)) * (Math.Max(tank.gases.plasma.moles / tank.gases.tritium.moles , 1))) , Math.Min(tank.gases.tritium.moles , tank.gases.plasma.moles / 2));
            double energy_released = 2 * reaction_efficiency * Constants.FIRE_CARBON_ENERGY_RELEASED;
            if (tank.gases.tritium.moles - reaction_efficiency < 0 || tank.gases.plasma.moles - (2 * reaction_efficiency) < 0) return false;
            tank.gases.BZ.moles += reaction_efficiency;
            tank.gases.tritium.moles -= reaction_efficiency;
            tank.gases.plasma.moles -= 2 * reaction_efficiency;

            if(energy_released > 0) {
                if(tank.getHeatCapacity() > Constants.MINIMUM_HEAT_CAPACITY) {
                    tank.temperature = Math.Max((tank.temperature * oldHeatCapacity + energy_released) / tank.getHeatCapacity() , Constants.TCMB);
                }
                return true;
            }
            return false;
        }

        public static bool react_stim(Tank tank) {
            
            if (tank.gases.plasma.moles < 10) return false;
            if (tank.gases.tritium.moles < 30) return false;
            if (tank.gases.BZ.moles < 20) return false;
            if (tank.gases.nitryl.moles < 30) return false;
            if (tank.temperature < Constants.STIMULUM_HEAT_SCALE/2) return false;
            Debug.WriteLine($"Reacting Stim");
            double oldHeatCapacity = tank.getHeatCapacity();

            double heat_scale = Math.Min(tank.temperature / Constants.STIMULUM_HEAT_SCALE , Math.Min(Math.Min(tank.gases.tritium.moles , tank.gases.plasma.moles) , tank.gases.nitryl.moles));
            double stim_energy_change = heat_scale + Constants.STIMULUM_FIRST_RISE * Math.Pow(heat_scale , 2) - Constants.STIMULUM_FIRST_DROP * Math.Pow(heat_scale , 3) + Constants.STIMULUM_SECOND_RISE * Math.Pow(heat_scale , 4) - Constants.STIMULUM_ABSOLUTE_DROP * Math.Pow(heat_scale , 5);
            if (tank.gases.tritium.moles - heat_scale < 0 || tank.gases.plasma.moles - heat_scale < 0) return false;
            tank.gases.stimulum.moles += heat_scale / 10;
            tank.gases.tritium.moles -= heat_scale;
            tank.gases.plasma.moles -= heat_scale;
            tank.gases.nitryl.moles -= heat_scale;


            if(stim_energy_change > 0) {
                if(tank.getHeatCapacity() > Constants.MINIMUM_HEAT_CAPACITY) {
                    tank.temperature = Math.Max((tank.temperature * oldHeatCapacity + stim_energy_change) / tank.getHeatCapacity() , Constants.TCMB);
                }
                return true;
            }
            return false;
        }

        public static bool react_noblium(Tank tank) {
            
            if (tank.gases.nitrogen.moles < 10) return false;
            if (tank.gases.tritium.moles < 5) return false;
            if (tank.temperature < 5000000) return false;
            Debug.WriteLine($"Reacting Nob");
            double oldHeatCapacity = tank.getHeatCapacity();

            double nob_formed = Math.Min((tank.gases.nitrogen.moles+tank.gases.tritium.moles)/100,Math.Min(tank.gases.tritium.moles/10,tank.gases.nitrogen.moles/20));
            double energy_taken = nob_formed * (Constants.NOBLIUM_FORMATION_ENERGY / Math.Max(tank.gases.BZ.moles , 1));
            if (tank.gases.tritium.moles - 10 * nob_formed < 0 || tank.gases.nitrogen.moles - 20 * nob_formed < 0) return false;


            if(nob_formed > 0) {
                if(tank.getHeatCapacity() > Constants.MINIMUM_HEAT_CAPACITY) {
                    tank.temperature = Math.Max((tank.temperature * oldHeatCapacity - energy_taken) / tank.getHeatCapacity() , Constants.TCMB);
                }
                return true;
            }
            return false;
        }

        public static bool react_tritium(Tank tank) {
            // tritium combustion
            
            if (tank.temperature < Constants.FIRE_MINIMUM_TEMPERATURE_TO_EXIST) return false;
            if (tank.gases.tritium.moles < Constants.MINIMUM_MOLE_COUNT) return false;
            if (tank.gases.oxygen.moles < Constants.MINIMUM_MOLE_COUNT) return false;
            //Debug.WriteLine($"Reacting Tritium");
            double oldHeatCapacity = tank.getHeatCapacity();

            double burned_fuel = 0;
            if(tank.gases.oxygen.moles < tank.gases.tritium.moles) {
                burned_fuel = tank.gases.oxygen.moles / Constants.TRITIUM_BURN_OXY_FACTOR;
            } else {
                burned_fuel = tank.gases.tritium.moles * Constants.TRITIUM_BURN_TRIT_FACTOR;
                tank.gases.tritium.moles -= tank.gases.tritium.moles / Constants.TRITIUM_BURN_TRIT_FACTOR;
                tank.gases.oxygen.moles -= tank.gases.tritium.moles; // Is this gonna be less than zero?
                if(tank.gases.oxygen.moles<0) Console.WriteLine($"OH NO OXYGEN MOLES IS LESS THAN ZERO?!!!!");
            }

            double energy_released = 0;
            if(burned_fuel>0) {
                energy_released += Constants.FIRE_HYDROGEN_ENERGY_RELEASED * burned_fuel;
                // radiation happens here
                tank.gases.waterVapour.moles += burned_fuel / Constants.TRITIUM_BURN_OXY_FACTOR;
            }

            if(energy_released > 0) {
                if(tank.getHeatCapacity() > Constants.MINIMUM_HEAT_CAPACITY) {
                    tank.temperature = (tank.temperature * oldHeatCapacity + energy_released) / tank.getHeatCapacity();
                }
                return true;
            }
            return false;
        }

        public static bool react_plasma(Tank tank) {
            // plasma combustion - last reaction
            
            if (tank.temperature < Constants.FIRE_MINIMUM_TEMPERATURE_TO_EXIST) return false;
            if (tank.gases.plasma.moles < Constants.MINIMUM_MOLE_COUNT) return false;
            if (tank.gases.oxygen.moles < Constants.MINIMUM_MOLE_COUNT) return false;
            Debug.WriteLine($"Reacting plasma");
            double oldHeatCapacity = tank.getHeatCapacity();

            double temperatureScale = 0;
            if(tank.temperature > Constants.PLASMA_UPPER_TEMPERATURE) {
                temperatureScale = 1;
            } else {
                // BYOND COMPILER DOES SOMETHING ODD AND MAKES T0C NEGATIVE
                //                                                                                                                                              V BYOND RETARDATION
                temperatureScale = (tank.temperature - (Constants.PLASMA_MINIMUM_BURN_TEMPERATURE-2*Constants.T0C)) / (Constants.PLASMA_UPPER_TEMPERATURE - Constants.PLASMA_MINIMUM_BURN_TEMPERATURE + 2*Constants.T0C);
            }
            //Debug.WriteLine($"{tank.temperature - (Constants.PLASMA_MINIMUM_BURN_TEMPERATURE-2*Constants.T0C)} {(Constants.PLASMA_UPPER_TEMPERATURE - Constants.PLASMA_MINIMUM_BURN_TEMPERATURE + 2*Constants.T0C)}");
            bool super_saturation = false;
            if(tank.gases.oxygen.moles/tank.gases.plasma.moles > Constants.SUPER_SATURATION_THRESHOLD) {
                super_saturation = true;
            }

            if(temperatureScale>0) {
                double oxygen_burn_rate = Constants.OXYGEN_BURN_BASE_RATE - temperatureScale;

                double plasma_burn_rate = 0;
                if(tank.gases.oxygen.moles > tank.gases.plasma.moles * Constants.PLASMA_OXYGEN_FULLBURN) {
                    plasma_burn_rate = tank.gases.plasma.moles * temperatureScale / Constants.PLASMA_BURN_RATE_DELTA;
                } else {
                    plasma_burn_rate = (temperatureScale * (tank.gases.oxygen.moles / Constants.PLASMA_OXYGEN_FULLBURN)) / Constants.PLASMA_BURN_RATE_DELTA;
                }

                //Debug.WriteLine($"temperatureScale={temperatureScale}, plasmaBurnRate={plasma_burn_rate}, oxygenBurnRate={oxygen_burn_rate}");

                if(plasma_burn_rate > Constants.MINIMUM_HEAT_CAPACITY) {
                    tank.gases.plasma.moles -= plasma_burn_rate;
                    tank.gases.oxygen.moles -= (plasma_burn_rate * oxygen_burn_rate);

                    if(super_saturation) {
                        tank.gases.tritium.moles += plasma_burn_rate;
                    } else {
                        tank.gases.carbonDioxide.moles += plasma_burn_rate;
                    }

                    double energy_released = Constants.FIRE_PLASMA_ENERGY_RELEASED * plasma_burn_rate;
                    
                    if(energy_released > 0) {
                        if(tank.getHeatCapacity() > Constants.MINIMUM_HEAT_CAPACITY) {
                            tank.temperature = (tank.temperature * oldHeatCapacity + energy_released) / tank.getHeatCapacity();
                        }
                    } 
                }
                return (plasma_burn_rate * (1 + oxygen_burn_rate) > 0) ? true : false;
            }
            tank.calculatePressure();
            return false;
        }

        public static bool react(Tank tank) {
            if (react_supress(tank)) return false;
            react_noblium(tank); //6
            react_stim(tank); //5
            react_bz(tank); //4
            react_nitryl(tank); //3
            react_fusion(tank); //2
                                //if(!react_water(tank)) //1 ignored due to requiring open space and only making wet floor tile
            react_tritium(tank); //-1
            react_plasma(tank); //-2
            return true;
        }

        public static double explode(Tank tank) {
            double range = (tank.pressure - Constants.TANK_FRAGMENT_PRESSURE) / Constants.TANK_FRAGMENT_SCALE;
            //Debug.WriteLine($"Range: {range}");
            //Debug.WriteLine($"EXPLOSION: ({Math.Round(range * 0.25)},{Math.Round(range * 0.5)},{Math.Round(range)},{Math.Round(range * 1.5)})");
            return range;
        }
	}
}

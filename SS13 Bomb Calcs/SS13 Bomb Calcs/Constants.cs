using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS13_Bomb_Calcs {
	public class Constants {
		public static double MINIMUM_HEAT_CAPACITY = 0.0003;
        public static double MINIMUM_MOLE_COUNT = 0.01;
		public static double ATMOSPHERE = 101.325;
        public static double MOLES_GAS_VISIBLE = 0.5;
		public static double TANK_FRAGMENT_PRESSURE = 40*ATMOSPHERE;
		public static double TANK_FRAGMENT_SCALE = 6*ATMOSPHERE;
		public static double T0C = 273.15;
        public static double TCMB = 2.7;
        public static double T20C = 293.15;
		public static double PLASMA_MINIMUM_BURN_TEMPERATURE = 100+T0C;
        public static double FIRE_MINIMUM_TEMPERATURE_TO_EXIST = 100 + T0C;
		public static double PLASMA_OXYGEN_FULLBURN = 10;
        public static double PLASMA_UPPER_TEMPERATURE = 1370 + T0C;
		public static double OXYGEN_BURN_BASE_RATE = 1.4;
		//public static double FIRE_PLASMA_ENERGY_RELEASED = 3000000;
		//public static double PLASMA_BURN_RATE_DELTA = 9;
		public static double MAX_TANK_PRESSURE = 25*ATMOSPHERE; // is now 2533kPa
		public static double TANK_VOLUME = 70;
		public static double MINIMUM_TEMPERATURE_DELTA_TO_CONSIDER = 0.5;
        public static double PREFILLED_CANISTER_PRESSURE = 90 * ATMOSPHERE * 0.5;

		public static double R_IDEAL_GAS_EQUATION = 8.31;
	
		public static double O2_SPECIFIC_HEAT_CAPACITY = 20;
		public static double PLASMA_SPECIFIC_HEAT_CAPACITY = 200; // or is it 500?
		public static double N2_SPECIFIC_HEAT_CAPACITY = 20;
		public static double CO2_SPECIFIC_HEAT_CAPACITY = 30;
		public static double WATERV_SPECIFIC_HEAT_CAPACITY = 40;
		public static double NOBLIUM_SPECIFIC_HEAT_CAPACITY = 2000;
		public static double N2O_SPECIFIC_HEAT_CAPACITY = 40;
        public static double NO2_SPECIFIC_HEAT_CAPACITY = 20;
		public static double BZ_SPECIFIC_HEAT_CAPACITY = 20;
        public static double TRITIUM_SPECIFIC_HEAT_CAPACITY = 10;
        public static double STIMULUM_SPECIFIC_HEAT_CAPACITY = 5;
        public static double PLUOXIUM_SPECIFIC_HEAT_CAPACITY = 80;

        // laurie is lazy so the fusion powers are defined in the gas

        //Plasma fire properties
        public static double OXYGEN_BURN_RATE_BASE				=1.4;
        public static double PLASMA_BURN_RATE_DELTA				=9;
        public static double PLASMA_MINIMUM_OXYGEN_NEEDED		=2;
        public static double PLASMA_MINIMUM_OXYGEN_PLASMA_RATIO	=30;
        public static double FIRE_CARBON_ENERGY_RELEASED			=100000;	//Amount of heat released per mole of burnt carbon into the tile
        public static double FIRE_HYDROGEN_ENERGY_RELEASED		=280000;  //Amount of heat released per mole of burnt hydrogen and/or tritium(hydrogen isotope)
        public static double FIRE_PLASMA_ENERGY_RELEASED			=3000000;	//Amount of heat released per mole of burnt plasma into the tile
        //General assmos defines.
        public static double WATER_VAPOR_FREEZE					=200;
        public static double NITRYL_FORMATION_ENERGY				=100000;
        public static double TRITIUM_BURN_OXY_FACTOR				=100;
        public static double TRITIUM_BURN_TRIT_FACTOR			=10;
        public static double TRITIUM_BURN_RADIOACTIVITY_FACTOR	=50000; 	//The neutrons gotta go somewhere. Completely arbitrary number.
        public static double TRITIUM_MINIMUM_RADIATION_ENERGY	=0.1;  	//minimum 0.01 moles trit or 10 moles oxygen to start producing rads
        public static double SUPER_SATURATION_THRESHOLD			=96;
        public static double STIMULUM_HEAT_SCALE					=100000;
        public static double STIMULUM_FIRST_RISE					=0.65;
        public static double STIMULUM_FIRST_DROP					=0.065;
        public static double STIMULUM_SECOND_RISE				=0.0009;
        public static double STIMULUM_ABSOLUTE_DROP				=0.00000335;
        public static double REACTION_OPPRESSION_THRESHOLD		=5;
        public static double NOBLIUM_FORMATION_ENERGY			=2e9; 	//1 Mole of Noblium takes the planck energy to condense.
        //Plasma fusion properties
        public static double FUSION_ENERGY_THRESHOLD				=3e9; 	//Amount of energy it takes to start a fusion reaction
        public static double FUSION_TEMPERATURE_THRESHOLD		=1000; 	//Temperature required to start a fusion reaction
        public static double FUSION_MOLE_THRESHOLD				=250; 	//Mole count required (tritium/plasma) to start a fusion reaction
        public static double FUSION_RELEASE_ENERGY_SUPER			=3e9; 	//Amount of energy released in the fusion process, super tier
        public static double FUSION_RELEASE_ENERGY_HIGH			=1e9; 	//Amount of energy released in the fusion process, high tier
        public static double FUSION_RELEASE_ENERGY_MID			=5e8; 	//Amount of energy released in the fusion process, mid tier
        public static double FUSION_RELEASE_ENERGY_LOW			=1e8; 	//Amount of energy released in the fusion process, low tier
        public static double FUSION_MEDIATION_FACTOR				=80; 		//Arbitrary
        public static double FUSION_SUPER_TIER					=50; 		//anything above this is super tier
        public static double FUSION_HIGH_TIER					=20;		//anything above this and below 50 is high tier
        public static double FUSION_MID_TIER						=5;		//anything above this and below 20 is mid tier - below this is low tier, but that doesnt need a define
        public static double FUSION_ENERGY_DIVISOR_SUPER			=25;
        public static double FUSION_ENERGY_DIVISOR_HIGH			=20;
        public static double FUSION_ENERGY_DIVISOR_MID			=10;
        public static double FUSION_ENERGY_DIVISOR_LOW			=2;
        public static double FUSION_GAS_CREATION_FACTOR_SUPER	=0.20;	//stimulum and pluoxium - 40% in total
        public static double FUSION_GAS_CREATION_FACTOR_HIGH		=0.60; 	//trit - one gas, so its higher than the other two - 60% in total
        public static double FUSION_GAS_CREATION_FACTOR_MID		=0.45; 	//BZ and N2O - 90% in total
        public static double FUSION_GAS_CREATION_FACTOR_LOW		=0.48; 	//O2 and CO2 - 96% in total
        public static double FUSION_MID_TIER_RAD_PROB_FACTOR		=2;		//probability of radpulse is power ratio * this for whatever tier
        public static double FUSION_LOW_TIER_RAD_PROB_FACTOR		=5;
        public static double FUSION_EFFICIENCY_BASE				=60;		//used in the fusion efficiency calculations
        public static double FUSION_EFFICIENCY_DIVISOR			=0.6;		//ditto
        public static double FUSION_RADIATION_FACTOR				=15000;	//horizontal asymptote
        public static double FUSION_RADIATION_CONSTANT			=30;		//equation is form of (ax) / (x + b), where a = radiation factor and b = radiation constant (https://www.desmos.com/calculator/4i1f296phl)
        public static double FUSION_VOLUME_SUPER					=100;		//volume of the sound the fusion noises make
        public static double FUSION_VOLUME_HIGH					=50;
        public static double FUSION_VOLUME_MID					=25;
        public static double FUSION_VOLUME_LOW	                =10;
	}
}

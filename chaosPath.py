import matplotlib.pyplot as plt
from os import path
import math
import matplotlib
from matplotlib import gridspec
import copy
from matplotlib import colors as mcolors
from matplotlib import cm
from matplotlib.colors import ListedColormap, LinearSegmentedColormap

_debug = True

#plt.figure(figsize=(12,9))

def toradians(degrees): # matching byond code
	return (degrees) * 0.0174532925
def modulus(x, y): # matching byond code
	return (x) - (y) * math.floor((x) / (y))

T0C = 273.15
TOROID_VOLUME_BREAKEVEN = 1000
FUSION_MOLE_THRESHOLD = 250
FUSION_ENERGY_THRESHOLD = 3e9
FUSION_MAXIMUM_TEMPERATURE = 1e8
INSTABILITY_GAS_POWER_FACTOR  = 0.003
PLASMA_BINDING_ENERGY = 20000000
FUSION_INSTABILITY_ENDOTHERMALITY = 2
FUSION_TRITIUM_MOLES_USED = 1
FUSION_TRITIUM_CONVERSION_COEFFICIENT = 1e-10
PARTICLE_CHANCE_CONSTANT = -20000000
FUSION_RAD_COEFFICIENT = -1000
FUSION_RAD_MAX = 2000
FUSION_TEMPERATURE_THRESHOLD = 10000
MINIMUM_HEAT_CAPACITY = 0.0003
FIRE_MINIMUM_TEMPERATURE_TO_EXIST = 100+T0C
PLASMA_MINIMUM_BURN_TEMPERATURE = 100+T0C
MINIMUM_MOLE_COUNT = 0.01
PLASMA_UPPER_TEMPERATURE = 1370 + T0C
OXYGEN_BURN_RATE_BASE = 1.4
SUPER_SATURATION_THRESHOLD = 96
PLASMA_OXYGEN_FULLBURN = 10
PLASMA_BURN_RATE_DELTA = 9
FIRE_PLASMA_ENERGY_RELEASED = 3000000
MINIMUM_TRIT_OXYBURN_ENERGY = 2000000
TRITIUM_BURN_OXY_FACTOR = 100
TRITIUM_BURN_TRIT_FACTOR = 10
FIRE_HYDROGEN_ENERGY_RELEASED = 280000
N2O_DECOMPOSITION_MIN_ENERGY = 1400
N2O_DECOMPOSITION_ENERGY_RELEASED = 200000
NITRYL_FORMATION_ENERGY = 100000
ONE_ATMOSPHERE = 101.325
FIRE_CARBON_ENERGY_RELEASED = 100000
STIMULUM_HEAT_SCALE = 100000
STIMULUM_FIRST_RISE = 0.65
STIMULUM_FIRST_DROP = 0.065
STIMULUM_SECOND_RISE = 0.0009
STIMULUM_ABSOLUTE_DROP = 0.00000335
REACTION_OPPRESSION_THRESHOLD = 5
NOBLIUM_FORMATION_ENERGY = 2e9
STIM_BALL_GAS_AMOUNT = 5
R_IDEAL_GAS_EQUATION = 8.31
MINIMUM_TEMPERATURE_DELTA_TO_CONSIDER = 0.5
TANK_RUPTURE_PRESSURE = (35.*ONE_ATMOSPHERE)
TANK_FRAGMENT_PRESSURE = (40.*ONE_ATMOSPHERE)
TANK_FRAGMENT_SCALE = (ONE_ATMOSPHERE*3)
TANK_MELT_TEMPERATURE = 1000000
MOLES_GAS_VISIBLE = 0.25
WATER_VAPOR_FREEZE = 200
CANNISTER_MAXIMUM_PRESSURE = 4500

class Gas_mixture:
	volume = 1000
	temperature = 293
	plasma = 0 #gas amounts in moles
	oxygen = 0
	nitrogen = 0
	carbon = 0
	water = 0
	tritium = 0
	nitrous = 0
	hypernob = 0
	stim = 0
	pluox = 0
	miasma = 0
	bz = 0
	nitryl = 0


	def heat_capacity(self):
		c = 0
		c += self.oxygen * 20
		c += self.nitrogen * 20
		c += self.carbon * 30
		c += self.plasma * 200
		c += self.water * 40
		c += self.hypernob * 2000
		c += self.nitrous * 40
		c += self.nitryl * 20
		c += self.tritium * 10
		c += self.bz * 20
		c += self.stim * 5
		c += self.pluox * 80
		c += self.miasma * 20
		return c

	def total_moles(self):
		moles = 0
		moles += self.oxygen
		moles += self.nitrogen
		moles += self.carbon
		moles += self.plasma
		moles += self.water
		moles += self.hypernob
		moles += self.nitrous
		moles += self.nitryl
		moles += self.tritium
		moles += self.bz
		moles += self.stim
		moles += self.pluox
		moles += self.miasma
		return moles

	def thermal_energy(self):
		return self.temperature * self.heat_capacity()

	def return_pressure(self):
		if self.volume > 0:
			total_moles = self.total_moles()
			return total_moles * R_IDEAL_GAS_EQUATION * self.temperature / self.volume

	def fill(self, gastype, maximum_pressure = CANNISTER_MAXIMUM_PRESSURE):
		# fill up the rest of the tank with a gas (or from empty)
		if gastype == "plasma":
			self.plasma = (maximum_pressure - self.return_pressure()) * self.volume / (R_IDEAL_GAS_EQUATION * self.temperature)

	def __str__(self):
		return "Temperature: {:.1f} Pressure: {:.1f} Volume: {:.1f}\n O2: {:.3f} N2: {:.3f} C02: {:.2f} Plasma: {:.1f} N2O: {:.3f} Nitryl: {:.3f} Trit: {:.1f}\nBZ: {:.3f} Stim: {:.3f} Pluox: {:.3f} Hypernob: {:.3f} Water: {:.1f}".format(self.temperature, self.return_pressure(), self.volume,  self.oxygen, self.nitrogen, self.carbon, self.plasma, self.nitrous, self.nitryl, self.tritium, self.bz, self.stim, self.pluox, self.hypernob, self.water)

def quantize(input):
	return round(input, 7)

def nobsuppression(gases = Gas_mixture()):
	if gases.hypernob < REACTION_OPPRESSION_THRESHOLD:
		return gases, False
	return gases, True

def water_vapor(gases = Gas_mixture()):
	if gases.water < MOLES_GAS_VISIBLE:
		return gases, False
	if not (gases.temperature <= WATER_VAPOR_FREEZE):
		gases.water -= MOLES_GAS_VISIBLE
	return gases, True

def nitrous_decomp(gases = Gas_mixture()):

	if gases.temperature < N2O_DECOMPOSITION_MIN_ENERGY:
		return gases, False
	if gases.nitrous < MINIMUM_MOLE_COUNT:
		return gases, False

	energy_released = 0
	old_heat_capacity = gases.heat_capacity()
	burned_fuel = 0

	burned_fuel = max(0, 0.00002* (gases.temperature - (0.00001 * math.pow(gases.temperature, 2)))) * gases.nitrous
	gases.nitrous -= burned_fuel # NEGATIVE NUMBERS AHHH

	if burned_fuel > 0:
		energy_released += N2O_DECOMPOSITION_ENERGY_RELEASED * burned_fuel

		gases.oxygen += burned_fuel / 2
		gases.nitrogen += burned_fuel

		if gases.heat_capacity() > MINIMUM_HEAT_CAPACITY:
			gases.temperature = (gases.temperature * old_heat_capacity + energy_released) / gases.heat_capacity()
		return gases, True

	return gases, False

def tritfire(gases = Gas_mixture()):

	if gases.temperature < FIRE_MINIMUM_TEMPERATURE_TO_EXIST:
		return gases, False
	if gases.tritium < MINIMUM_MOLE_COUNT:
		return gases, False
	if gases.oxygen < MINIMUM_MOLE_COUNT:
		return gases, False

	energy_released = 0
	old_heat_capacity = gases.heat_capacity()
	burned_fuel = 0

	if gases.oxygen < gases.tritium or MINIMUM_TRIT_OXYBURN_ENERGY > gases.thermal_energy():
		burned_fuel = gases.oxygen / TRITIUM_BURN_OXY_FACTOR
		gases.tritium -= burned_fuel # couldn't this make it go negative?...
	else:
		burned_fuel = gases.tritium * TRITIUM_BURN_TRIT_FACTOR
		gases.tritium -= gases.tritium / TRITIUM_BURN_TRIT_FACTOR
		gases.oxygen -= gases.tritium

	if burned_fuel > 0:
		energy_released += FIRE_HYDROGEN_ENERGY_RELEASED * burned_fuel
		gases.water += burned_fuel / TRITIUM_BURN_OXY_FACTOR

	if energy_released > 0:
		if gases.heat_capacity() > MINIMUM_HEAT_CAPACITY:
			gases.temperature = (gases.temperature * old_heat_capacity + energy_released) / gases.heat_capacity()
		return gases, True

	return gases, False

def plasmafire(gases = Gas_mixture()):

	if gases.temperature < FIRE_MINIMUM_TEMPERATURE_TO_EXIST:
		return gases, False
	if gases.plasma < MINIMUM_MOLE_COUNT:
		return gases, False
	if gases.oxygen < MINIMUM_MOLE_COUNT:
		return gases, False

	energy_released = 0
	old_heat_capacity = gases.heat_capacity()
	plasma_burn_rate = 0
	oxygen_burn_rate = 0
	temperature_scale = 0
	super_saturation = False

	if gases.temperature > PLASMA_UPPER_TEMPERATURE:
		temperature_scale = 1
	else:
		temperature_scale = (gases.temperature - PLASMA_MINIMUM_BURN_TEMPERATURE) / (PLASMA_UPPER_TEMPERATURE - PLASMA_MINIMUM_BURN_TEMPERATURE)

	if temperature_scale > 0:
		oxygen_burn_rate = OXYGEN_BURN_RATE_BASE - temperature_scale
		if gases.oxygen / gases.plasma > SUPER_SATURATION_THRESHOLD:
			super_saturation = True
		if gases.oxygen > gases.plasma * PLASMA_OXYGEN_FULLBURN:
			plasma_burn_rate = gases.plasma * temperature_scale / PLASMA_BURN_RATE_DELTA
		else:
			plasma_burn_rate = (temperature_scale * (gases.oxygen / PLASMA_OXYGEN_FULLBURN)) / PLASMA_BURN_RATE_DELTA

		if plasma_burn_rate > MINIMUM_HEAT_CAPACITY:
			plasma_burn_rate = min(plasma_burn_rate, gases.plasma, gases.oxygen / oxygen_burn_rate)
			gases.plasma = quantize(gases.plasma - plasma_burn_rate)
			gases.oxygen = quantize(gases.oxygen - (plasma_burn_rate * oxygen_burn_rate))

			if super_saturation:
				gases.tritium += plasma_burn_rate
			else:
				gases.carbon += plasma_burn_rate

			energy_released += FIRE_PLASMA_ENERGY_RELEASED * plasma_burn_rate

	if energy_released > 0:
		if gases.heat_capacity() > MINIMUM_HEAT_CAPACITY:
			gases.temperature = (gases.temperature * old_heat_capacity + energy_released) / gases.heat_capacity()
		return gases, True

	return gases, False

def fusion(gases = Gas_mixture()):

	if gases.temperature < FUSION_TEMPERATURE_THRESHOLD:
		return gases, False
	if gases.tritium < FUSION_TRITIUM_MOLES_USED:
		return gases, False
	if gases.plasma < FUSION_MOLE_THRESHOLD:
		return gases, False
	if gases.carbon < FUSION_MOLE_THRESHOLD:
		return gases, False

	#headcapacity
	reaction_energy = 0


	scale_factor = gases.volume / math.pi
	toroidal_size = 2 * math.pi + math.atan((gases.volume - TOROID_VOLUME_BREAKEVEN) / TOROID_VOLUME_BREAKEVEN)
	gas_power = 0

	initial_plasma = gases.plasma
	initial_carbon = gases.carbon
	initial_heat_capacity = gases.heat_capacity()

	# only some gases appear to have fusion_power associated with them
	gas_power += gases.water * 8
	gas_power += gases.nitrous * 10
	gas_power += gases.nitryl * 16
	gas_power += gases.tritium * 1
	gas_power += gases.bz * 8
	gas_power += gases.stim * 7
	gas_power += gases.pluox * -10

	instability = modulus(math.pow(gas_power * INSTABILITY_GAS_POWER_FACTOR, 2), toroidal_size)

	plasma = (gases.plasma - FUSION_MOLE_THRESHOLD) / scale_factor
	carbon = (gases.carbon - FUSION_MOLE_THRESHOLD) / scale_factor

	plasma = modulus(plasma - (instability * math.sin(carbon)), toroidal_size)
	carbon = modulus(carbon - plasma, toroidal_size)

	gases.plasma = plasma*scale_factor + FUSION_MOLE_THRESHOLD
	gases.carbon = carbon*scale_factor + FUSION_MOLE_THRESHOLD

	delta_plasma = initial_plasma - gases.plasma

	reaction_energy += delta_plasma*PLASMA_BINDING_ENERGY # can be positive or negative
	if instability < FUSION_INSTABILITY_ENDOTHERMALITY:
		reaction_energy = max(reaction_energy, 0) # stable reaction = exothermic
		if _debug:
			print("\t\tExothermic (stable reaction)")
	elif reaction_energy < 0:
		reaction_energy *= math.pow(instability-FUSION_INSTABILITY_ENDOTHERMALITY,0.5)
		if _debug:
			print("\t\tEndothermic")

	if gases.thermal_energy() + reaction_energy < 0:
		gases.plasma = initial_plasma
		gases.carbon = initial_carbon
		return gases, False # no reaction

	gases.tritium -= FUSION_TRITIUM_MOLES_USED

	if reaction_energy > 0:
		gases.oxygen += FUSION_TRITIUM_MOLES_USED*(reaction_energy * FUSION_TRITIUM_CONVERSION_COEFFICIENT)
		gases.nitrous += FUSION_TRITIUM_MOLES_USED*(reaction_energy * FUSION_TRITIUM_CONVERSION_COEFFICIENT)
	else:
		gases.bz += FUSION_TRITIUM_MOLES_USED*(reaction_energy * (-FUSION_TRITIUM_CONVERSION_COEFFICIENT))
		gases.nitryl += FUSION_TRITIUM_MOLES_USED*(reaction_energy * (-FUSION_TRITIUM_CONVERSION_COEFFICIENT))

	if reaction_energy > 0:
		particle_chance = PARTICLE_CHANCE_CONSTANT / (reaction_energy-PARTICLE_CHANCE_CONSTANT) + 1
		rad_power = max(FUSION_RAD_COEFFICIENT/instability + FUSION_RAD_MAX, 0)

		if gases.heat_capacity() > MINIMUM_HEAT_CAPACITY and (gases.temperature <= FUSION_MAXIMUM_TEMPERATURE or reaction_energy <= 0):
			gases.temperature = (gases.temperature * initial_heat_capacity + reaction_energy) / gases.heat_capacity()
			if (gases.temperature < 2.7):
				gases.temperature = 2.7 # TCMB? I think  -- yes

		return gases, True

	return gases, True # it technically returns null in byond, but I think this is jsut ignored

def nitrylformation(gases = Gas_mixture()):
	if gases.temperature < FIRE_MINIMUM_TEMPERATURE_TO_EXIST * 60:
		return gases, False
	if gases.oxygen < 20:
		return gases, False
	if gases.nitrogen < 20:
		return gases, False
	if gases.nitrous < 5:
		return gases, False

	old_heat_capacity = gases.heat_capacity()
	heat_efficiency = min(gases.temperature/FIRE_MINIMUM_TEMPERATURE_TO_EXIST*100, gases.oxygen, gases.nitrogen)
	energy_used = heat_efficiency * NITRYL_FORMATION_ENERGY

	if gases.oxygen - heat_efficiency < 0 or gases.nitrogen - heat_efficiency < 0:
		return gases, False

	gases.oxygen -= heat_efficiency
	gases.nitrogen -= heat_efficiency
	gases.nitryl += heat_efficiency * 2

	if energy_used > 0:
		if gases.heat_capacity() > MINIMUM_HEAT_CAPACITY:
			gases.temperature = (gases.temperature * old_heat_capacity + energy_released) / gases.heat_capacity()
		return gases, True

	return gases, False

def bzformation(gases = Gas_mixture()):
	if gases.nitrous < 10:
		return gases, False
	if gases.plasma < 10:
		return gases, False

	old_heat_capacity = gases.heat_capacity()
	reaction_efficiency = min(1/((gases.return_pressure() / (0.1 * ONE_ATMOSPHERE))*max(gases.plasma / gases.nitrous, 1)), gases.nitrous, gases.plasma / 2)
	energy_released = 2 * reaction_efficiency * FIRE_CARBON_ENERGY_RELEASED
	if gases.nitrous - reaction_efficiency < 0 or gases.plasma - (2*reaction_efficiency) < 0 or energy_released <= 0:
		return gases, False

	gases.bz += reaction_efficiency
	if reaction_efficiency == gases.nitrogen:
		gases.bz -= min(gases.return_pressure(), 1)
		gases.oxygen += min(gases.return_pressure(), 1)
	gases.nitrous -= reaction_efficiency
	gases.plasma -= 2*reaction_efficiency

	if energy_released > 0:
		if gases.heat_capacity() > MINIMUM_HEAT_CAPACITY:
			gases.temperature = (gases.temperature * old_heat_capacity + energy_released) / gases.heat_capacity()
		return gases, True

	return gases, False

def stimformation(gases = Gas_mixture()):
	if gases.temperature < STIMULUM_HEAT_SCALE / 2:
		return gases, False
	if gases.tritium < 30:
		return gases, False
	if gases.plasma < 10:
		return gases, False
	if gases.bz < 20:
		return gases, False
	if gases.nitryl < 30:
		return gases, False

	old_heat_capacity = gases.heat_capacity()
	heat_scale = min(gases.temperature/STIMULUM_HEAT_SCALE,gases.tritium,gases.plasma,gases.nitryl)
	stim_energy_change = heat_scale + STIMULUM_FIRST_RISE * math.pow(heat_scale, 2) - STIMULUM_FIRST_DROP * math.pow(heat_scale, 3) + STIMULUM_SECOND_RISE * math.pow(heat_scale, 4) - STIMULUM_ABSOLUTE_DROP * math.pow(heat_scale, 5)

	if gases.tritium - heat_scale < 0:
		return gases, False

	gases.stim += heat_scale / 10
	gases.tritium -= heat_scale
	gases.plasma -= heat_scale
	gases.nitryl -= heat_scale

	if stim_energy_change:
		if gases.heat_capacity() > MINIMUM_HEAT_CAPACITY:
			gases.temperature = (gases.temperature * old_heat_capacity + stim_energy_change) / gases.heat_capacity()
		return gases, True

	return gases, False

def nobiliumformation(gases = Gas_mixture()):
	if gases.temperature < 5000000:
		return gases, False
	if gases.nitrogen < 10:
		return gases, False
	if gases.tritium < 5:
		return gases, False

	old_heat_capacity = gases.heat_capacity()
	nob_formed = min((gases.nitrogen+gases.tritium)/100,gases.tritium/10,gases.nitrogen/20)
	energy_taken = nob_formed*(NOBLIUM_FORMATION_ENERGY/(max(gases.bz,1)))

	if gases.tritium - 10 * nob_formed < 0 or gases.nitrogen - 20 * nob_formed < 0:
		return gases, False

	gases.tritium -= 10 * nob_formed
	gases.nitrogen -= 20 * nob_formed
	gases.hypernob += nob_formed

	if nob_formed:
		if gases.heat_capacity() > MINIMUM_HEAT_CAPACITY:
			gases.temperature = (gases.temperature * old_heat_capacity - energy_taken) / gases.heat_capacity()
		return gases, True

	return gases, False

def stim_ball(gases = Gas_mixture()):

	if gases.temperature < FIRE_MINIMUM_TEMPERATURE_TO_EXIST:
		return gases, False
	if gases.pluox < STIM_BALL_GAS_AMOUNT:
		return gases, False
	if gases.stim < STIM_BALL_GAS_AMOUNT:
		return gases, False
	if gases.nitryl < MINIMUM_MOLE_COUNT:
		return gases, False
	if gases.plasma < MINIMUM_MOLE_COUNT:
		return gases, False

	stim_used = min(STIM_BALL_GAS_AMOUNT / gases.plasma, gases.stim)
	pluox_used = min(STIM_BALL_GAS_AMOUNT/ gases.plasma, gases.pluox)

	energy_released = stim_used * STIMULUM_HEAT_SCALE

	gases.carbon += 4*pluox_used
	gases.nitrogen += 8*stim_used
	gases.pluox -= pluox_used
	gases.stim -=stim_used
	gases.plasma *= 0.5

	if energy_released > 0:
		if gases.heat_capacity() > MINIMUM_HEAT_CAPACITY:
			gases.temperature = (gases.temperature * old_heat_capacity + energy_released) / gases.heat_capacity()
		return gases, True

	return gases, False

def react(gases = Gas_mixture()):
	# I'm not exactly sure if gases still change even if they aren't technically reacting. Byond code is stupid
	# I think if it reacts, it moves onto the next reaction in the list other than stuff that has STOP_REACTIONS (e.g. nob surpression)
	returnGases, didReact = nobsuppression(gases) # priority inf
	returnstr = ""
	if didReact:
		if _debug:
			print("\tNob supression")
		return returnGases, 20
	returnGases, didReact = nobiliumformation(gases) # priority 6
	if didReact:
		if _debug:
			print("\tNob formation")
		gases = returnGases
		returnstr += "NobForm "
	returnGases, didReact = stimformation(gases) # priority 5
	if didReact:
		if _debug:
			print("\tStim formation")
			gases = returnGases
			returnstr += "StimForm "
	returnGases, didReact = bzformation(gases) # priority 4
	if didReact:
		if _debug:
			print("\tBZ formation")
			gases = returnGases
			returnstr += "BZForm "
	returnGases, didReact = nitrylformation(gases) # priority 3
	if didReact:
		if _debug:
			print("\tNitryl formation")
			gases = returnGases
			returnstr += "NitrylForm "
	returnGases, didReact = fusion(gases) # priority 2
	if didReact:
		if _debug:
			print("\tFusion")
			gases = returnGases
			returnstr += "Fusion "
	returnGases, didReact = nitrous_decomp(gases) # priority 0
	if didReact:
		if _debug:
			print("\tNitrous decomp")
			gases = returnGases
			returnstr += "N2ODecomp "
	returnGases, didReact = tritfire(gases) # priority -1
	if didReact:
		if _debug:
			print("\tTrit fire")
			gases = returnGases
			returnstr += "TritFire "
	returnGases, didReact = plasmafire(gases) # priority -2
	if didReact:
		if _debug:
			print("\tPlasma fire")
			gases = returnGases
			returnstr += "PlasmaFire"

	return gases, returnstr

def merge(gas1 = Gas_mixture(), gas2 = Gas_mixture()):
	# assume both inputs exist...
	returnGas = copy.deepcopy(gas1)

	if abs(gas1.temperature - gas2.temperature) > MINIMUM_TEMPERATURE_DELTA_TO_CONSIDER:
		combined_heat_capacity = gas1.heat_capacity() + gas2.heat_capacity()
		if combined_heat_capacity:
			returnGas.temperature = (gas1.temperature * gas1.heat_capacity() + gas2.temperature * gas2.heat_capacity()) / combined_heat_capacity

		# todo: re-do the gas class so I can enumerate over the gases

		returnGas.plasma += gas2.plasma
		returnGas.oxygen += gas2.oxygen
		returnGas.nitrogen += gas2.nitrogen
		returnGas.carbon += gas2.carbon
		returnGas.water += gas2.water
		returnGas.tritium += gas2.tritium
		returnGas.nitrous += gas2.nitrous
		returnGas.hypernob += gas2.hypernob
		returnGas.stim += gas2.stim
		returnGas.pluox += gas2.pluox
		returnGas.miasma += gas2.miasma
		returnGas.bz += gas2.bz
		returnGas.nitryl += gas2.nitryl

	return returnGas

cannister = Gas_mixture()
#cannister.pressure = 4500
cannister.temperature = 273
cannister.tritium = 200
cannister.plasma = 21953.7  # 2300 and 1400 seems to be spicy
cannister.carbon = 2493.3

print("Cannister pressure initial: "+str(cannister.return_pressure()))

# heat it up
cannister.temperature = 20000
# the "fusion test cannister" is 500 moles co2, 500 moles plasma, 350 trit 15000K (1000vol cannister)
# ------------------------------------MULTI SWEEP-------------------
temperatures = []
start = FUSION_MOLE_THRESHOLD
end = 2800
step = 10
highest_temp = 0
cannister_out = Gas_mixture()
for plasma in range(0,1500, 100):
	for carbon in range(start,end, step):
		cannister = Gas_mixture()
		# cannister.pressure = 4500
		cannister.temperature = 293 # to start with
		cannister.tritium = 350 # enough for 100 fusions
		cannister.carbon = carbon
		cannister.plasma = min(plasma, (4500 - cannister.return_pressure()) * cannister.volume / (R_IDEAL_GAS_EQUATION * cannister.temperature))
		#cannister.fill("plasma", CANNISTER_MAXIMUM_PRESSURE)

		cannister_orig = copy.deepcopy(cannister)
		cannister.temperature = 20000 # heat that badboi up

		temp_list = []
		for i in range(0,100):
			cannister, didReact = react(cannister)
			c_copy = copy.deepcopy(cannister)
			temp_list.append(c_copy.temperature)
			if c_copy.temperature > highest_temp:
				highest_temp = c_copy.temperature
				cannister_out = copy.deepcopy(cannister_orig)
		temperatures.append(temp_list)

fig, ax = plt.subplots(figsize=(12,9))
viridis = cm.get_cmap('viridis')
for i, t in enumerate(temperatures):
	#color = (math.floor(i / ((end-start)/step))) / ((end-start)/step) # color changes on plasma
	color = (i%((end-start)/step)) / ((end-start)/step) # color changes on carbon
	plt.plot(t, color = viridis(color))
sm = plt.cm.ScalarMappable(cmap=viridis, norm=plt.Normalize(vmin=start, vmax=end))
sm.set_array([])# fake array - https://stackoverflow.com/questions/8342549/matplotlib-add-colorbar-to-a-sequence-of-line-plots
plt.colorbar(sm)
print("Highest temp of {:.1f}".format(highest_temp))
print(cannister_out)
plt.show()

# -------------------------------------------------------------------------------------------------
exit()
# -------------SINGLETANK--------
tank1 = Gas_mixture()
tank1.volume = 70
tank1.temperature = 373
tank1.oxygen = 0.25 * 450 * 70 / (R_IDEAL_GAS_EQUATION * 373)
tank1.plasma = 0.75 * 450 * 70 / (R_IDEAL_GAS_EQUATION * 373)

tank2 = Gas_mixture()
tank2.volume = 70
tank2.temperature = 989
tank2.tritium = 1000 * 70 / (R_IDEAL_GAS_EQUATION * 989)

# print("Tanks:")
# print(tank1)
# print(tank2)

del cannister
cannister = merge(tank1, tank2)

# ignoring previous
cannister = Gas_mixture()
cannister.volume = 1000
cannister.tritium = 500
cannister.carbon = 350
cannister.temperature = 293
cannister.fill("plasma", CANNISTER_MAXIMUM_PRESSURE)
print(cannister)
# then BOOOST up the temperature!!
cannister.temperature = 20000

gas_history = []
c_copy = copy.deepcopy(cannister)
gas_history.append(c_copy)
didReact_history = []

for i in range(0,3000):
	if i==58:
		print()
	print(">>>"+str(i))
	cannister, didReact = react(cannister)
	c_copy = copy.deepcopy(cannister)
	gas_history.append(c_copy)
	didReact_history.append(didReact)

	# if cannister.return_pressure() > TANK_FRAGMENT_PRESSURE:
	# 	# KABOOM!
	# 	# react one more time
	# 	print(">>>LAST ")
	# 	cannister, didReact = react(cannister)
	# 	c_copy = copy.deepcopy(cannister)
	# 	gas_history.append(c_copy)
	# 	didReact_history.append(didReact)
	#
	# 	print("WENT KABOOM WITH RANGE: "+str((cannister.return_pressure() - TANK_FRAGMENT_PRESSURE)/TANK_FRAGMENT_SCALE))
	# 	break
	# elif cannister.return_pressure() > TANK_RUPTURE_PRESSURE or cannister.temperature > TANK_MELT_TEMPERATURE:
	# 	# boooring, dumped all gases ou
	# 	print("May have ruptured, depends on integrity")

print(cannister)

pressures = []
temperatures = []
oxygen = []
plasma = []
tritium = []
carbon = []
nitrous = []
for g in gas_history:
	pressures.append(g.return_pressure())
	temperatures.append(g.temperature)
	oxygen.append(g.oxygen)
	plasma.append(g.plasma)
	tritium.append(g.tritium)
	carbon.append(g.carbon)
	nitrous.append(g.nitrous)

fig, (ax1, ax2, ax3) = plt.subplots(3, 1, sharex=True, figsize=(12,9))
plt.grid(True)

o_line, = ax1.semilogy(oxygen)
o_line.set_label("O2")
p_line, = ax1.semilogy(plasma)
p_line.set_label("Plasma")
t_line, = ax1.semilogy(tritium)
t_line.set_label("Tritium")
c_line, = ax1.semilogy(carbon)
c_line.set_label("CO2")
n_line, = ax1.semilogy(nitrous)
n_line.set_label("N2O")

ax1.legend()
ax1.set_ylabel("Moles")

ax2.semilogy(pressures)
ax2.set_ylabel("Pressure")

ax3.plot(temperatures)
ax3.set_ylabel("Temperature")

plt.plot()
plt.show()

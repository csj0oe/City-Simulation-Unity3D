# City Simulation in Unity3D

The goal of the simulation is to create a procedural city, with buildings and houses, streets, and its population.
We want to see the population commute between low density areas (houses) to high density areas (workplaces), following roads.

## The city

The city is generated procedurally.
There are zones with buildings, and zones with houses.
It has roads, some of them must be pedestrian roads (in dense areas), while most of them are for cars.
The buildings are near the roads.  Visually, it must be clear if the building is in downtown (high density) or in a suburb (low density).
There must be different textures so that we can see if the building is lighted or not.

## The inhabitants

The simulation simulates the days in the life of the inhabitants: they go to work and go back home afterwards.

At the start of the day, the inhabitants start the day at their house. The houses are lit.
The inhabitants have to work downtown. When they leave their house, they switch off the light.
They will take the road to reach their building. When they reach their building, they increase the lighting of their window/building.
At the end of the day, the inhabitants go back home, following the roads again.
The inhabitants must look differently when they drive on roads for cars and when they walk on pedestrian road.
The number of inhabitants must be as large as possible, to create possible traffic jams, alternate routes, ...

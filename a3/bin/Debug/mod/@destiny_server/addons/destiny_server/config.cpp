class CfgPatches
{
	class destiny_server
	{
		units[] = {};
		weapons[] = {};
		requiredVersion = 0.1;
		requiredAddons[] = {"A3_Data_F"};
	};
};

class CfgFunctions
{
	class Server
	{
		class Init
		{
			file = "\destiny_server";
			class initFunctions { postInit=1; };
		};
	};
	


};
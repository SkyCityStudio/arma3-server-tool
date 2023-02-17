destiny_fnc_getObjectManipulationNum = {
	_allPlayer = count (call BIS_fnc_listPlayers);
	_allUnits = count(allUnits);
	_allCar = count(entities "Car");
	_allHelicopter = count(entities "Helicopter");
	_allMotorcycle = count(entities "Motorcycle");
	_allPlane = count(entities "Plane");
	_allShip = count(entities "Ship");
	_allStaticWeapon = count(entities "StaticWeapon");
	_allAPC = count(entities "APC");
	_allTank = count(entities "Tank");
	_allUnitsUAV = count allUnitsUAV;
	_allMissionObjects = count (allMissionObjects "All");
	_allDeadMen = count allDeadMen;
	_allGroups = count allGroups;
	_allMines = count allMines;
	_fps = diag_fps;
	_fpsMin = diag_fpsMin;
	_data = format ["%1:%2:%3:%4:%5:%6:%7:%8:%9:%10:%11:%12:%13:%14:%15:%16:%17:%18:%19","ObjectManipulationNum",destiny_var_serverUUID,_allPlayer,_allUnits,_allCar,_allHelicopter,_allMotorcycle,_allPlane,_allShip,_allStaticWeapon,_allAPC,_allTank,_allUnitsUAV,_allMissionObjects,_allDeadMen,_allGroups,_allMines,_fps,_fpsMin];
	"DestinyServerMonitoring" callExtension ["ObjectManipulationNum", [_data]];
};

destiny_fnc_getPlayerInfo = {
	_data = "";
	(call BIS_fnc_listPlayers) apply {
		_playerId = getPlayerUID _x;
		_playerName = name _x;
		_playerScores = getPlayerScores _x;
		_infantryKills = _playerScores # 0;
		_softVehicleKills = _playerScores # 1;
		_armorKills = _playerScores # 2;
		_airKills = _playerScores # 3;
		_deaths = _playerScores # 4;
		_totalScore = _playerScores # 5;
		_data = _data + format ["%1:%2:%3:%4:%5:%6:%7:%8:%9:%10|","PlayerInfo",destiny_var_serverUUID,_playerId,_playerName,_infantryKills,_softVehicleKills,_armorKills,_airKills,_deaths,_totalScore];
	};
	"DestinyServerMonitoring" callExtension ["PlayerInfo", [_data]];
};

destiny_fnc_updateOnlineInfo = {
	"DestinyServerMonitoring" callExtension ["UpdateOnlineInfo", [_this]];
};

addMissionEventHandler ["PlayerConnected",{
	params ["_id", "_uid", "_name", "_jip", "_owner", "_idstr"];
	(format["%1:%2:%3:%4","UpdateOnlineInfo",destiny_var_serverUUID,_uid,1]) spawn destiny_fnc_updateOnlineInfo;
}];

addMissionEventHandler ["PlayerDisconnected",{
	params ["_id", "_uid", "_name", "_jip", "_owner", "_idstr"];
	(format["%1:%2:%3:%4","UpdateOnlineInfo",destiny_var_serverUUID,_uid,0]) spawn destiny_fnc_updateOnlineInfo;
}];

destiny_fnc_restart = {
	_edTimeLeft = 3600 * destiny_var_restartTime;
	estimatedTimeLeft _edTimeLeft;
	destiny_module_restart_script = {
		_text = format ["<t size='1'>*****服务器即将重启*****</t><br/>%1",destiny_var_restartInfo];
		[_text,-1,-1,destiny_var_restartLastTime,1,0,789] remoteExecCall ['BIS_fnc_dynamicText',-2,FALSE];
		uiSleep destiny_var_restartLastTime;
		(call (uiNamespace getVariable 'destiny_server_command_password')) serverCommand '#restartserver';
	};
	0 spawn {
		while {true} do {
			sleep 3;
			if (serverTime > estimatedEndServerTime && estimatedEndServerTime > 1) exitWith {
				0 spawn destiny_module_restart_script;
			};
		};
	};
};
if (destiny_var_restartTime > 0) then {
	call destiny_fnc_restart;
};
if(destiny_var_enableStatistics) then {
	0 spawn {
		while {true} do {
			sleep 60;
			call destiny_fnc_getObjectManipulationNum;
		    call destiny_fnc_getPlayerInfo;
		};
	};
};
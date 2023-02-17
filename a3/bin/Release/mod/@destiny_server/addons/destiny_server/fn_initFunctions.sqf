if (isServer) then {
	destiny_var_restartTime = 20;
	destiny_var_restartInfo = '服务器重启马上就好!';
	destiny_var_restartLastTime = 260;
	uiNamespace setVariable ['destiny_server_command_password',(compileFinal "'01454680ae3e4236b098624fe8bd4fc7'")];
	destiny_var_enableStatistics = true;
	destiny_var_serverUUID = 'aab57be360784a979966ae0e0e702d8c';
	[] call compileFinal preprocessFileLineNumbers "\destiny_server\script\destiny_fnc_monitoring_service.sqf";
};

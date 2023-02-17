if (isServer) then {{
	destiny_var_restartTime = {0};
	destiny_var_restartInfo = '{1}';
	destiny_var_restartLastTime = {2};
	uiNamespace setVariable ['destiny_server_command_password',(compileFinal "'{3}'")];
	destiny_var_enableStatistics = {4};
	destiny_var_serverUUID = '{5}';
	[] call compileFinal preprocessFileLineNumbers "\destiny_server\script\destiny_fnc_monitoring_service.sqf";
}};
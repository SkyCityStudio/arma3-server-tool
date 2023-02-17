if (isServer) then {
	destiny_var_restartTime = 4;
	destiny_var_restartInfo = '服务器重启马上就好!2';
	destiny_var_restartLastTime = 66;
	uiNamespace setVariable ['destiny_server_command_password',(compileFinal "'8d098ab2562a4fb8b0bafcb032486253'")];
	destiny_var_enableStatistics = true;
	destiny_var_serverUUID = '9b1f7158b4574d548fd9647ac9cc55b3';
	[] call compileFinal preprocessFileLineNumbers "\destiny_server\script\destiny_fnc_monitoring_service.sqf";
};

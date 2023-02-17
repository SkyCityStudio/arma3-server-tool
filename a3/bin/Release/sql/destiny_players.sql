PRAGMA foreign_keys = false;
CREATE TABLE IF NOT EXISTS destiny_players (
	id integer NOT NULL PRIMARY KEY AUTOINCREMENT,
	guid varchar ( 255 ) NOT NULL,
	player_name varchar ( 255 ) NOT NULL,
	ip varchar ( 255 ) NOT NULL,
	create_date varchar ( 255 ) NOT NULL 
	);
PRAGMA foreign_keys = true;
PRAGMA foreign_keys = false;

CREATE TABLE IF NOT EXISTS "a3_servers" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "server_name" TEXT
);

CREATE TABLE IF NOT EXISTS "a3_player_info" (
  "id" INTEGER NOT NULL,
  "server_id" INTEGER,
  "data_key" TEXT,
  "player_id" text,
  "player_name" TEXT,
  "infantry_kills" integer,
  "soft_vehicle_kills" integer,
  "armor_kills" integer,
  "air_kills" integer,
  "deaths" integer,
  "total_score" integer,
  "create_time" text,
  "online" integer,
  "create_time_timestamp" integer,
  PRIMARY KEY ("id"),
  CONSTRAINT "server_id" FOREIGN KEY ("server_id") REFERENCES "a3_servers" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS "a3_object_manipulation_num" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "server_id" INTEGER NOT NULL,
  "data_key" TEXT,
  "all_player" integer,
  "all_units" integer,
  "all_car" integer,
  "all_helicopter" integer,
  "all_motorcycle" integer,
  "all_plane" integer,
  "all_ship" integer,
  "all_static_weapon" integer,
  "all_apc" integer,
  "all_tank" integer,
  "all_units_uav" integer,
  "all_mission_objects" integer,
  "all_dead_men" integer,
  "all_groups" integer,
  "all_mines" integer,
  "create_time" text,
  "fps" integer,
  "fps_min" integer,
  "create_time_timestamp" integer,
  CONSTRAINT "server_id" FOREIGN KEY ("server_id") REFERENCES "a3_servers" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);
PRAGMA foreign_keys = true;
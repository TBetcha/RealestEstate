CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS users(
 user_id UUID NOT NULL,
first_name text NOT NULL,
last_name text NOT NULL,
password text NOT NULL,
email text NOT NULL,


PRIMARY KEY (user_id)


);

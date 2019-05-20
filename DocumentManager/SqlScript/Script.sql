CREATE SCHEMA document_manager;

USE document_manager;

CREATE TABLE profile (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	name VARCHAR(64) NOT NULL,
	UNIQUE KEY profile_name_un (name)
);

INSERT INTO profile (name) VALUES ('ADMINISTRADOR');

CREATE TABLE profile_feature (
	profile_id INT NOT NULL,
	feature_id INT NOT NULL,
	PRIMARY KEY (profile_id, feature_id),
	CONSTRAINT profile_feature_profile_id_fk
		FOREIGN KEY (profile_id)
		REFERENCES profile (id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

CREATE TABLE user (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	user_name VARCHAR(64) NOT NULL,
	full_name VARCHAR(64) NOT NULL,
	password VARCHAR(100) NOT NULL,
	profile_id INT NOT NULL,
	picture_version INT NOT NULL,
	active BOOLEAN NOT NULL,
	token_low BIGINT NOT NULL,
	token_high BIGINT NOT NULL,
	UNIQUE KEY user_user_name_un (user_name),
	CONSTRAINT user_profile_id_fk
		FOREIGN KEY (profile_id)
		REFERENCES profile (id)
		ON DELETE NO ACTION
		ON UPDATE CASCADE
);

INSERT INTO user (user_name, full_name, password, profile_id, picture_version, active, token_low, token_high) VALUES ('admin', 'ADMINISTRADOR', 'k2yew1ZGIN3Qe2NHA0KS4lI2+VadNr43PdXfBVstWTEE:q6dfK7fYm5SH/86x/MfkYPaU5K34yBr8UZ52Ga6USVeh', 1, 0, 1, 0, 0);

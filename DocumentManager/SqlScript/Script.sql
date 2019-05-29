CREATE SCHEMA document_manager;

USE document_manager;

CREATE TABLE profile (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	name_en VARCHAR(64) NOT NULL,
	name_ptbr VARCHAR(64) NOT NULL,
	UNIQUE KEY profile_name_en_un (name_en),
	UNIQUE KEY profile_name_ptbr_un (name_ptbr)
);

INSERT INTO profile (name_en, name_ptbr) VALUES ('ADMINISTRATOR', 'ADMINISTRADOR');

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
	language_id INT NOT NULL,
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

INSERT INTO user (user_name, full_name, password, profile_id, language_id, picture_version, active, token_low, token_high) VALUES ('admin', 'ADMINISTRADOR', 'k2yew1ZGIN3Qe2NHA0KS4lI2+VadNr43PdXfBVstWTEE:q6dfK7fYm5SH/86x/MfkYPaU5K34yBr8UZ52Ga6USVeh', 1, 0, 0, 1, 0, 0);

CREATE TABLE course (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	name_en VARCHAR(64) NOT NULL,
	name_ptbr VARCHAR(64) NOT NULL,
	short_name_en VARCHAR(16) NOT NULL,
	short_name_ptbr VARCHAR(16) NOT NULL,
	UNIQUE KEY course_name_en_un (name_en),
	UNIQUE KEY course_name_ptbr_un (name_ptbr)
);

CREATE TABLE partition_type (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	name_en VARCHAR(64) NOT NULL,
	name_ptbr VARCHAR(64) NOT NULL,
	UNIQUE KEY partition_type_name_en_un (name_en),
	UNIQUE KEY partition_type_name_ptbr_un (name_ptbr)
);

CREATE TABLE document_type (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	name_en VARCHAR(64) NOT NULL,
	name_ptbr VARCHAR(64) NOT NULL,
	UNIQUE KEY document_type_name_en_un (name_en),
	UNIQUE KEY document_type_name_ptbr_un (name_ptbr)
);

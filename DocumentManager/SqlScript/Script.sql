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

CREATE TABLE unity (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	name_en VARCHAR(64) NOT NULL,
	name_ptbr VARCHAR(64) NOT NULL,
	short_name_en VARCHAR(16) NOT NULL,
	short_name_ptbr VARCHAR(16) NOT NULL,
	UNIQUE KEY unity_name_en_un (name_en),
	UNIQUE KEY unity_name_ptbr_un (name_ptbr)
);

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

CREATE TABLE tag (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	name_en VARCHAR(64) NOT NULL,
	name_ptbr VARCHAR(64) NOT NULL,
	UNIQUE KEY profile_name_en_un (name_en),
	UNIQUE KEY profile_name_ptbr_un (name_ptbr)
);

CREATE TABLE tag_value (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	tag_id INT NOT NULL,
	position INT NOT NULL,
	name_en VARCHAR(64) NOT NULL,
	name_ptbr VARCHAR(64) NOT NULL,
	CONSTRAINT tag_value_tag_id_fk
		FOREIGN KEY (tag_id)
		REFERENCES tag (id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

CREATE TABLE document_type_default_tags (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	document_type_id INT NOT NULL,
	tag_id INT NOT NULL,
	position INT NOT NULL,
	CONSTRAINT document_type_default_tags_document_type_id_fk
		FOREIGN KEY (document_type_id)
		REFERENCES document_type (id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT document_type_default_tags_tag_id_fk
		FOREIGN KEY (tag_id)
		REFERENCES tag (id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

CREATE TABLE user_permission_partition_type (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	user_id INT NOT NULL,
	unity_id INT NOT NULL,
	course_id INT NOT NULL,
	partition_type_id INT NOT NULL,
	feature_permission_id INT NOT NULL,
	UNIQUE KEY user_permission_partition_type_UN (user_id, unity_id, course_id, partition_type_id, feature_permission_id),
	CONSTRAINT user_permission_partition_type_partition_type_id_fk
		FOREIGN KEY (partition_type_id)
		REFERENCES partition_type (id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

CREATE TABLE user_permission_document_type (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	user_id INT NOT NULL,
	unity_id INT NOT NULL,
	course_id INT NOT NULL,
	document_type_id INT NOT NULL,
	feature_permission_id INT NOT NULL,
	UNIQUE KEY user_permission_document_type_UN (user_id, unity_id, course_id, document_type_id, feature_permission_id),
	CONSTRAINT user_permission_document_type_document_type_id_fk
		FOREIGN KEY (document_type_id)
		REFERENCES document_type (id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

CREATE TABLE document (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	name VARCHAR(128) NOT NULL,
	description VARCHAR(1000) NOT NULL,
	extension VARCHAR(10) NOT NULL,
	size INT NOT NULL,
	unity_id INT NOT NULL,
	course_id INT NOT NULL,
	partition_type_id INT NOT NULL,
	document_type_id INT NOT NULL,
	creation_user_id INT NOT NULL,
	creation_time DATETIME NOT NULL,
	CONSTRAINT document_unity_id_fk
		FOREIGN KEY (unity_id)
		REFERENCES unity (id)
		ON DELETE RESTRICT
		ON UPDATE CASCADE,
	CONSTRAINT document_course_id_fk
		FOREIGN KEY (course_id)
		REFERENCES course (id)
		ON DELETE RESTRICT
		ON UPDATE CASCADE,
	CONSTRAINT document_partition_type_id_fk
		FOREIGN KEY (partition_type_id)
		REFERENCES partition_type (id)
		ON DELETE RESTRICT
		ON UPDATE CASCADE,
	CONSTRAINT document_document_type_id_fk
		FOREIGN KEY (document_type_id)
		REFERENCES document_type (id)
		ON DELETE RESTRICT
		ON UPDATE CASCADE,
	CONSTRAINT document_creation_user_id_fk
		FOREIGN KEY (creation_user_id)
		REFERENCES user (id)
		ON DELETE NO ACTION
		ON UPDATE CASCADE
);

CREATE TABLE document_tag (
	id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	document_id INT NOT NULL,
	tag_id INT NOT NULL,
	tag_value_id INT NOT NULL,
	UNIQUE KEY document_tag_document_id_tag_id_tag_value_id_un (document_id, tag_id, tag_value_id),
	CONSTRAINT document_tag_document_id_fk
		FOREIGN KEY (document_id)
		REFERENCES document (id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT document_tag_id_fk
		FOREIGN KEY (tag_id)
		REFERENCES tag (id)
		ON DELETE RESTRICT
		ON UPDATE CASCADE,
	CONSTRAINT document_tag_value_id_fk
		FOREIGN KEY (tag_value_id)
		REFERENCES tag_value (id)
		ON DELETE RESTRICT
		ON UPDATE CASCADE
);

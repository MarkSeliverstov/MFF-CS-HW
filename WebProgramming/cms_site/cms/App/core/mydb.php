<?php

class MyDB 
{
	var $server;
	var $dblogin;
	var $dbpass;
	var $db;
	var $mysqli;
	var $query;
	var $err;
	var $result;
	var $data;
	
	function __construct() {
		require_once __DIR__ . '/../Configs/db_config.php';
		$this->server = $db_config['server'];
		$this->dblogin = $db_config['login'];
		$this->dbpass = $db_config['password'];
		$this->db = $db_config['database'];
	}
	function connect() {
		$this->mysqli = new mysqli($this->server, $this->dblogin, $this->dbpass, $this->db);
		if ($this->mysqli->connect_error) {
			$this->err = $this->mysqli->connect_error;
		}
	}

	function close() {
		$this->mysqli->close();
	}

	function run($query) {
		$this->query = $query;
		if (!$this->data = $this->mysqli->query($query)) {
			$this->err = $this->mysqli->connect_error;
		}
	}
	// function row() {
	//   $this->data = mysql_fetch_assoc($this->result);
	// }
	function fetch() {
		$this->result = $this->data->fetch_all(MYSQLI_ASSOC);
	}

	function run_one_query($query){
		$this->connect();
		$this->run($query);
		if (!is_bool($this->data)){
			$this->fetch();
		}
		$this->close();
	}
}
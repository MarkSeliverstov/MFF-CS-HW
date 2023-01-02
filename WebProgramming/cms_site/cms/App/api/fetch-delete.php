<?php

require_once __DIR__ . '/../core/mydb.php';
$id = $_GET['delId'];

$query = "DELETE FROM `cms` WHERE id=$id;";

$myDB = new MyDB();

$myDB->run_one_query($query);
if (empty($myDB->err)){
    echo json_encode(array('delete' => 'success'));
}
else{
    echo json_encode(array('delete' => 'dailed'));
}
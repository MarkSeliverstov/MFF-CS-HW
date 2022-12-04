<?php

function ViewPage(){
    $temp = __DIR__ . '/templates';
    $param = __DIR__ . '/parameters';

    $file =  $_GET['page'];
    $file = (is_dir($temp . '/' . $file)) ? 
        $file . '/index.php' : $file . '.php';

    if (file_exists($temp . '/' . $file)){
        if(file_exists($param . '/' . $file)) 
            ViewWithParam($param . '/' . $file, $temp . '/' . $file);
        else 
            require_once($temp . '/' . $file);
    }
    else ViewNotFound();
    return;
}

function ViewWithParam($paramPath, $file){
    $paramArr = require($paramPath);

    foreach($paramArr as $prm => $value){
        if (isset($_GET[$prm])){
            if (is_array($value)){
                if (in_array($_GET[$prm], $value))
                $$prm = $_GET[$prm];
                else{
                    ViewBadRequest();
                    return;
                }
            }
            else if ($value == 'int'){
                if (is_numeric($_GET[$prm]))
                $$prm = intval($_GET[$prm]);
                else{
                    ViewBadRequest();
                    return;
                }
            }
            else if ($value == 'string'){
                $$prm = $_GET[$prm];
            }
        }
        else{
            ViewServerError();
            return;
        }
    }
    require_once($file);
    return;
}

function ViewBadRequest(){
    http_response_code(400);
    ?> <h1>400 - Bad Request</h1> <?php
    return;
}

function ViewServerError(){
    http_response_code(500);
    ?> <h1>500 - Internal Server Error</h1> <?php
    return;
}

function ViewNotFound(){
    http_response_code(404);
    ?> <h1>404 - Not Found</h1> <?php
    return;
}



function ValidatePage(){
    $pattern = '/[a-zA-Z]+/';
    $arr = explode('/', $_GET['page']);

    foreach ($arr as $path) {
        if (empty($path) or !preg_match($pattern, $path)){
            return false;
        }
    }
    return true;
}

$header = __DIR__ . '/templates/_header.php';
$footer = __DIR__ . '/templates/_footer.php';
require_once($header);

if ( isset($_GET['page']) ){
    if (ValidatePage()) 
        ViewPage();
    else 
        ViewBadRequest();
}
else ViewBadRequest();

require_once($footer);

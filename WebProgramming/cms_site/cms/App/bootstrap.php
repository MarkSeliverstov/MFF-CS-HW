<?php


require_once __DIR__ . '/core/model.php';
require_once __DIR__ . '/core/view.php';
require_once __DIR__ . '/core/controller.php';

require_once __DIR__ . '/core/route.php';

try{
    $router = new Router();
    $router->dispatch();
}
catch(Exception $e){
    die("Uncaught Exception: " . $e->getMessage());
}
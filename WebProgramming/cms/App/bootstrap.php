<?php
function ErrorPage404()
{
    $host = 'http://'.$_SERVER['HTTP_HOST'].'/';
    header('HTTP/1.1 404 Not Found');
    header("Status: 404 Not Found");
    header('Location:'.$host.'404');
}

try {
    var_dump($_GET);
    echo('<br>');

    $controller_name = '';
    $action_name = '';
    $user_id = '';
    $possible_routes_count = 2;
    
    $routes = explode('/', $_GET['page']);

    if (count($routes) > $possible_routes_count) {
        throw new Exception('Too many routes');
    }

    if ( !empty($routes[0]) ) {
        $controller_name = $routes[0];
    }

    if ( !empty($routes[1]) && is_numeric($routes[1]) ) {
        $user_id = (int) $routes[1];
    }

    $controller_name = 'controller_' . $controller_name;

    echo "controller_name: $controller_name <br>";
    echo "user_id: $user_id <br>";

    $controller_file = strtolower($controller_name) . '.php';
    $controller_path = 'App/Controllers/' . $controller_file;
    if ( file_exists($controller_path) ) {
        require_once $controller_path;
    } else {
        echo $controller_path;
        // throw new Exception('Controller not found');
    }

    //$controller = new $controller_name;
    $action = 'action_' . $action_name;

    echo "controller: $controller <br>";
    echo "action: $action <br>";

    // if ( !empty($user_id) ) {
    //     $controller->$action($user_id);
    // } else {
    //     $controller->action_index();
    // }

} catch (Exception $e) {
    ErrorPage404();
} finally {
    
}
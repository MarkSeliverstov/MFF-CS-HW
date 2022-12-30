<?php

class Router{

    public function dispatch()
    {
        try {        
            $controller_name = 'main';
            $action_name = 'index';
            $user_id = null;
            
            $routes = explode('/', $_GET['page']);
            $action_and_controller = explode('-', $routes[0]);
            $possible_routes_count = 2;

            if (count($routes) > $possible_routes_count || count($action_and_controller) > 2) {
                throw new Exception('Too many routes');
            }
            
            // get controller name
            if ( !empty($action_and_controller[0]) ) {
                $controller_name = $action_and_controller[0];
            }

            // get action name
            if ( !empty($action_and_controller[1]) ) {
                $action_name = $action_and_controller[1];
            }

            // get user id
            if ( !empty($routes[1]) && is_numeric($routes[1]) ) {
                $user_id = (int) $routes[1];
            }
            
            // get names of model, controller and action
            $model_name = 'model_articles';
            $controller_name = 'controller_' . $controller_name;
            $action_name = 'action_' . $action_name;
            
            // echo "model_name: $model_name <br>";
            // echo "controller_name: $controller_name <br>";
            // echo "user_id: $user_id <br>";
            
            // Get model file
            $model_file = strtolower($model_name) . '.php';
            $model_path = 'App/Models/' . $model_file;
            if (file_exists($model_path)) {
                include($model_path);
            }

            // Get controller file
            $controller_file = strtolower($controller_name) . '.php';
            $controller_path = 'App/Controllers/' . $controller_file;
            if ( file_exists($controller_path) ) {
                include($controller_path);
            } else {
                throw new Exception('Controller not found');
            }
            
            // create controller
            $controller = new $controller_name;
            $action = $action_name;

            // call action
            if (method_exists($controller, $action)) {
                $controller->$action($user_id);
            } else {
                throw new Exception('Action not found');
            }

        } catch (Exception $e) {
            // if error, relocate to 404 page
            $this->ErrorPage404();
        }

    }
    static function ErrorPage404()
    {
        // relocate to 404 page
        $host = 'http://'.$_SERVER['HTTP_HOST'].'/';
        header('HTTP/1.1 404 Not Found');
        header("Status: 404 Not Found");
        header('Location:'.$host.'404');
    }
}
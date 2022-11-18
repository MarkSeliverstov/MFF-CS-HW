<?php
date_default_timezone_set('UTC');
require_once(__DIR__ . '/recodex_lib.php');

if ($_SERVER['REQUEST_METHOD'] == 'POST'){
    $message = 'ERROR';
    $invalidFields = [];

    // validate firstname
    if (isset($_POST['firstName']) && 
        !empty($_POST['firstName']) && 
        mb_strlen($_POST['firstName']) <= 100
        ){
        $firstName = $_POST['firstName'];
    }
    else{
        $invalidFields[] = 'firstName';
    }
    
    // validate lasttname
    if (isset($_POST['lastName']) && 
        !empty($_POST['lastName']) &&
        mb_strlen($_POST['lastName']) <= 100
        ){
        $lastName = $_POST['lastName'];
    }
    else{
        $invalidFields[] = 'lastName';
    }

    // validate email
    if (isset($_POST['email']) && 
        !empty($_POST['email']) && 
        filter_var($_POST['email'], FILTER_VALIDATE_EMAIL) &&
        mb_strlen($_POST['email']) <= 200
        ){
        $email = $_POST['email'];
    }
    else{    
        $invalidFields[] = 'email';
    }

    // validate deliveryBoy
    if (isset($_POST['deliveryBoy']) && (
        $_POST['deliveryBoy'] === 'jesus' ||
        $_POST['deliveryBoy'] === 'santa' ||
        $_POST['deliveryBoy'] === 'moroz' ||
        $_POST['deliveryBoy'] === 'hogfather' ||
        $_POST['deliveryBoy'] === 'czpost' ||
        $_POST['deliveryBoy'] === 'fedex'
    )){
        $deliveryBoy = $_POST['deliveryBoy'];
    }
    else{
        $invalidFields[] = 'deliveryBoy';
    }

    // validate unboxDay
    if (isset($_POST['unboxDay']) && 
        filter_var($_POST['unboxDay'], FILTER_VALIDATE_INT) && (
        $_POST['unboxDay'] === '24' ||
        $_POST['unboxDay'] === '25'
    )){
        $unboxDay = (int) $_POST['unboxDay'];
    }
    else{
        $invalidFields[] = 'unboxDay';
    }
    
    // validate fromTime and toTime 
    $fromTime = null;
    $toTime = null;
    $date_sec = 0;
    $pattern_time = '/^[0-2]?[0-9]:[0-5][0-9]$/';

    if (isset($_POST['fromTime'])){
        if ($_POST['fromTime'] != null){
            if (preg_match($pattern_time, $_POST['fromTime'])){
                $H = strtok($_POST['fromTime'], ':');
                if ((int)$H > 24){
                    $invalidFields[] = 'fromTime';
                }
                else{
                    $date_sec = strtotime($_POST['fromTime'], 0);
                    $fromTime = floor($date_sec/60);
                }
            }
            else{
                $invalidFields[] = 'fromTime';
            }
        }
    }
    else{
        $invalidFields[] = 'fromTime';
    }


    if (isset($_POST['toTime'])){
        if ($_POST['toTime'] != null){
            if (preg_match($pattern_time, $_POST['toTime'])){
                $H = strtok($_POST['toTime'], ':');
                if ((int)$H > 24){
                    $invalidFields[] = 'toTime';
                }
                else{
                    $date_sec = strtotime($_POST['toTime'], 0);
                    $toTime = floor($date_sec/60);
                }
            }
            else{
                $invalidFields[] = 'toTime';
            }
        }
    }
    else{
        $invalidFields[] = 'toTime';
    }


    //validate gifts
    $gifts = [];
    $giftCustom = null;
    if (isset($_POST['gifts'])){
        if (!empty($_POST['gifts'])){
            foreach ($_POST['gifts'] as $gift){
                if ($gift != 'socks' &&
                    $gift != 'points' &&
                    $gift != 'jarnik' &&
                    $gift != 'cash' &&
                    $gift != 'teddy' &&
                    $gift != 'other'
                ){
                    $invalidFields[] = 'gifts';
                    break;
                }
            }
            $gifts = $_POST['gifts'];
        }
        if (in_array('other', $_POST['gifts'])){
            if (isset($_POST['giftCustom'])){
                if (!empty($_POST['giftCustom']) && mb_strlen($_POST['giftCustom']) <= 100){
                    $giftCustom = $_POST['giftCustom'];
                }
                else{
                    $invalidFields[] = 'giftCustom';
                }
            }
            else{
                $invalidFields[] = 'giftCustom';
            }
        }
    }

    if (!empty($invalidFields))
        recodex_survey_error($message, $invalidFields);
    else{
        recodex_save_survey(
            $firstName, 
            $lastName, 
            $email, 
            $deliveryBoy,
            $unboxDay,
            $fromTime,
            $toTime,
            $gifts,
            $giftCustom,
        );
    }
    //header('Location: index.php', false, 302);
    $host  = $_SERVER['HTTP_HOST'];
    $uri   = rtrim(dirname($_SERVER['PHP_SELF']), '/\\');
    $extra = 'index.php';
    header("Location: http://$host$uri/$extra", false, 302);
}
require __DIR__ . '/form_template.html';

